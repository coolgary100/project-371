using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using AForge.Vision.Motion;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;


using System.Diagnostics;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
#if !__IOS__
using Emgu.CV.Cuda;
#endif
using Emgu.CV.XFeatures2D;

namespace cam_aforge1
{
    public partial class GUI : Form
    {

        private Point MouseDownLocation;
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        GUIElements myCanvas;

        MotionDetector detector = new MotionDetector(
                new SimpleBackgroundModelingDetector(),
                new MotionAreaHighlighting());
        int saveFlag=0;
        System.IO.FileStream fs;
        int tickCount = 0;
        int drawinglock = 0;
        int bloodSize = 10;
        int opaqCons = 10;
        List<int[]> pts = new List<int[]>();
        Point[] trackings ;
        int shape; //0(free) 1(line) 2(circle)
        Pen drawingPen = new Pen(Color.Black, 3);
        Pen objPen = new Pen(Color.Red, 3);
        Pen curserPen = new Pen(Color.Red, 3);

        //feature
        int[] objectside;
        int objwidth = 0;
        int objheight = 0;
        System.Drawing.Rectangle roi ;
        public BFMatcher matcher;
        public VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
        public Mat mask;
        public Mat homography;
        public Mat totHomography;
        public ORBDetector ORBCPU = new ORBDetector(1000);
        private Mat obj ;
        public Image<Bgr, Byte> objImage ;
        public UMat objDescriptors = new UMat();
        public UMat uObjImage;
        public UMat observedDescriptors = new UMat();
        public UMat uObservedImage = new UMat();
        public VectorOfKeyPoint observedKeyPoints = new VectorOfKeyPoint();
        public VectorOfKeyPoint objKeyPoints = new VectorOfKeyPoint();



        //Constructs the gui
        public GUI()
        {
            myCanvas = new GUIElements(this);
            InitializeComponent();
            this.DoubleBuffered = true;
            //Add custom events here, ie
            viewFinder.MouseDown += new MouseEventHandler(viewFinder_MouseDown);
            viewFinder.MouseMove += new MouseEventHandler(viewFinder_MouseMove);
        }

        System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, 0, 0);

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.DeepSkyBlue, rec);
            //Generates the shape            
        }


        //Generally don't have to change this
        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                vidSrc.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    vidSrc.Items.Add(device.Name);
                }
                vidSrc.SelectedIndex = 0; //make dafault to first cam
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                vidSrc.Items.Add("No capture device on your system");
            }
        }

        //Generally don't have to change this
        private void rfsh_Click(object sender, EventArgs e)
        {
            getCamList();
        }

        //Generally don't have to change this
        private void start_Click(object sender, EventArgs e)
        {
            try
            {
                if (start.Text == "&Start")
                {
                    if (DeviceExist)
                    {
                        videoSource = new VideoCaptureDevice(videoDevices[vidSrc.SelectedIndex].MonikerString);
                        videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                        CloseVideoSource();
                        videoSource.DesiredFrameSize = new Size(160, 120);
                        //videoSource.DesiredFrameRate = 10;
                        videoSource.Start();
                        label2.Text = "Device running...";
                        start.Text = "&Stop";
                        timer1.Enabled = true;
                    }
                    else
                    {
                        label2.Text = "Error: No Device selected.";
                    }
                }
                else
                {
                    if (videoSource.IsRunning)
                    {
                        timer1.Enabled = false;
                        CloseVideoSource();
                        label2.Text = "Device stopped.";
                        start.Text = "&Start";
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                System.Windows.Forms.MessageBox.Show("Unable to Save File.");
            }
        }

        //Generally don't have to change this
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            
            try
            {

                Bitmap img = (Bitmap)eventArgs.Frame.Clone();

                Image<Bgr, Byte> image = new Image<Bgr, Byte>(img);
                Mat imgmodel = image.Mat;
                tickCount++;

                imgmodel = Draw(imgmodel);
                img = imgmodel.Bitmap;
                myCanvas.g = Graphics.FromImage(img);

                while (drawinglock != 0) { }
                drawinglock = 1;

                int[] mouse = getMouseCoordinates(img);
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(mouse[0], mouse[1], 5, 5);
                myCanvas.g.DrawEllipse(curserPen, rec);

               
                if (button1.Text == "Load")
                {
                    myCanvas.g.DrawRectangle(objPen, Math.Min(objectside[0], objectside[0] + objwidth), Math.Min(objectside[1], objectside[1] + objheight), Math.Abs(objwidth), Math.Abs(objheight));
                }
                else if (button1.Text == "UnLoad")
                {
                    int i;
                    for( i=0; i < trackings.Length && 0 <= trackings[i].X; i++)
                    {
                    }
                    Point[] drawtracking = new Point[i-1];
                    int j=0;
                    for ( i = 0; i < drawtracking.Length; j++)
                    {
                        drawtracking[j] = trackings[i];
                        //if (i + 2 < drawtracking.Length && 10+3*Math.Abs(trackings[i + 2].X - trackings[i].X) + 3*Math.Abs(trackings[i + 2].Y - trackings[i].Y) - Math.Abs(trackings[i + 1].X - trackings[i].X) - Math.Abs(trackings[i + 1].Y - trackings[i].Y) < 0)
                        //{
                         //   i++;
                        //}
                        i++;
                    }
                    /*for (i = 0; i<drawtracking.Length && 0 < drawtracking[i].X; i++)
                    {
                    }
                    Point[] drawtracking2 = new Point[i - 1];
                    for (i = 0; i < drawtracking2.Length; i++)
                    {
                        drawtracking2[i] = drawtracking[i];
                    }*/
                    myCanvas.g.DrawLines(objPen, drawtracking);
                }

                
                foreach (int[] point in pts)
                {
                    if (point[0] == 0)//free drawing
                    {
                        int i;
                        for (i = 1; i < point.Length && point[i] >= 0; i++)
                        {
                        }

                        Point[] curve = new Point[(i-1)/2];
                        if (curve.Length > 1)
                        {
                            for (i = 0; i < curve.Length; i++)
                            {
                                curve[i] = new Point(point[2 * i + 1], point[2 * i + 2]);
                            }
                            myCanvas.g.DrawLines(drawingPen, curve);
                        }
                    }

                    if (point[0] == 6)//free drawing
                    {
                        int i;
                        int tic;
                        for (i = 2; 2 * i + 1 < point.Length && point[2 * i + 1] >= 0; i++) 
                        {
                            tic = tickCount - point[1];
                            myCanvas.g.FillEllipse(new SolidBrush(Color.FromArgb(point[3], 131, 3, 3)), point[2 * i] - (float)(tic * .5 + point[2] / 2.0), point[2 * i + 1] - (float)(tic * .5 + point[2] / 2.0), (float)(tic * 1 + point[2]), (float)(tic * 1 + point[2]));
                        }
                    }
                    if (point[0] == 1)//draw line
                    {
                        myCanvas.g.DrawLine(drawingPen, point[1], point[2], point[3], point[4]);
                    }
                    if (point[0] == 2)//draw circle
                    {
                        myCanvas.g.DrawEllipse(drawingPen, point[1] - point[3], point[2] - point[3], 2 * point[3], 2 * point[3]);
                    }
                }


                

                // double h = detector.ProcessFrame(img);
                //System.Console.WriteLine(h);

                //myCanvas.g.TranslateTransform(img.Width / 2, img.Height / 2);
                // Rotate
                //myCanvas.g.RotateTransform(30);
                // Restore rotation point in the matrix
                //myCanvas.g.TranslateTransform(-img.Width / 2, -img.Height / 2);
                // Draw the image on the bitmap
                //myCanvas.g.DrawImage(img, new Point(0, 0));

                if (saveFlag == 1)
                {
                    saveFlag = 0;
                    img.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                }
                myCanvas.Run();
                viewFinder.Image = img;
                myCanvas.g.Dispose();
                drawinglock = 0;

            }
            catch 
            {

            }

        }

        //Generally don't have to change this
        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        //Generally don't have to change this
        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "Device running... " + videoSource.FramesReceived.ToString() + " FPS";
        }

        //Generally don't have to change this
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseVideoSource();
        }

        //Button for changing camera control
        private void ctrl_Click(object sender, EventArgs e)
        {
            CamControl.show_Controls();
        }
       
        protected void viewFinder_MouseDown(object sender,MouseEventArgs e)
        {
            try
            {
                int[] mouse = getMouseCoordinates((Bitmap)this.viewFinder.Image);
                int Xm = mouse[0];
                int Ym = mouse[1];

                while (drawinglock != 0) { }
                drawinglock = 1;

                if (e.Button == MouseButtons.Left) //can also use this one:
                //if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (shape == 0)//free drawing
                    {
                        int[] pt = new int[11];
                        for (int i=0;i<pt.Length;i++)
                        {
                            pt[i] = -1;
                        }
                        pt[0] = 0;
                        pt[1] = Xm;
                        pt[2] = Ym;
                        pts.Add(pt);
                    }
                    if (shape==1)//line
                    {
                        int[] pt = new int[] { 1, Xm, Ym, Xm, Ym };
                        pts.Add(pt);
                    }
                    else if (shape == 2)
                    {
                        int[] pt = new int[] { 2, Xm, Ym, 0 };
                        pts.Add(pt);
                    }
                    else if (shape == 5 )//object
                    {
                        objectside = new int[] { Xm, Ym};
                        objwidth = 0;
                        objheight = 0;
                    }
                    else if (shape == 6)//free drawing
                    {
                        int[] pt = new int[12];
                        for (int i = 0; i < pt.Length; i++)
                        {
                            pt[i] = -1;
                        }
                        pt[0] = 6;
                        pt[1] = tickCount;
                        pt[2] = bloodSize;
                        pt[3] = opaqCons;
                        pt[4] = Xm;
                        pt[5] = Ym;
                        pts.Add(pt);
                    }


                }
                else if (e.Button == MouseButtons.Right)
                {
                }
                MouseDownLocation = e.Location;
            }
            catch
            {

            }
            drawinglock = 0;
        }

        protected void viewFinder_MouseMove(object sender,MouseEventArgs e)
        {
            try
            {
                while (drawinglock != 0) { }
                drawinglock = 1;

                int[] mouse = getMouseCoordinates((Bitmap)this.viewFinder.Image);
                int Xm = mouse[0];
                int Ym = mouse[1];

                if (e.Button == MouseButtons.Left)
                {
                    //obj rec
                    if (shape==5)
                    {
                        objwidth = (int)(Xm - objectside[0]);
                        objheight = (int)(Ym - objectside[1]);
                        roi = new System.Drawing.Rectangle(Math.Min(objectside[0], objectside[0] + objwidth), Math.Min(objectside[1], objectside[1] + objheight), Math.Abs(objwidth), Math.Abs(objheight));
                    }
                    else if(shape==0 || shape == 1 || shape == 2|| shape == 6)
                    {
                        int[] pt = pts[pts.Count - 1];

                        if (pt[0] == 0)//free drawing
                        {
                            int i;
                            if (pt[pt.Length - 1] >= 0)//increse the size
                            {
                                int[] newpt = new int[2 * pt.Length + 1];
                                for (i = 0; i < pt.Length; i++)
                                {
                                    newpt[i] = pt[i];
                                }
                                newpt[i] = Xm;
                                newpt[i + 1] = Ym;
                                for (i = i + 2; i < newpt.Length; i++)
                                {
                                    newpt[i] = -1;
                                }
                                pts.RemoveAt(pts.Count - 1);
                                pts.Add(newpt);
                            }
                            else
                            {
                                for (i = 0; pt[i] >= 0; i++)
                                {
                                }
                                pt[i] = Xm;
                                pt[i + 1] = Ym;
                            }
                        }
                        else if (pt[0] == 1) // line
                        {
                            pt[3] = Xm;
                            pt[4] = Ym;
                        }
                        else if (pt[0] == 2)//circle
                        {
                            int radius = (int)Math.Sqrt((1.0 * Xm - pt[1]) * (1.0 * Xm - pt[1]) + (1.0 * Ym - pt[2]) * (1.0 * Ym - pt[2]));
                            pt[3] = radius;
                        }
                        else if (pt[0] == 6)//free drawing
                        {
                            int i;
                            if (pt[pt.Length - 1] >= 0)//increse the size
                            {
                                int[] newpt = new int[2 * pt.Length];
                                for (i = 0; i < pt.Length; i++)
                                {
                                    newpt[i] = pt[i];
                                }
                                newpt[i] = Xm;
                                newpt[i + 1] = Ym;
                                for (i = i + 2; i < newpt.Length; i++)
                                {
                                    newpt[i] = -1;
                                }
                                pts.RemoveAt(pts.Count - 1);
                                pts.Add(newpt);
                            }
                            else
                            {
                                for (i = 0; pt[i] >= 0; i++)
                                {
                                }
                                pt[i] = Xm;
                                pt[i + 1] = Ym;
                            }

                        }
                    }
                    
                    MouseDownLocation = e.Location;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    //TODO: deleting here
                    MouseDownLocation = e.Location;
                }
            }
            catch
            {

            }
            drawinglock = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            shape = 0;//pen
        }

        private void button2_Click(object sender, EventArgs e)
        {
            shape = 1;//line
        }

        private void button3_Click(object sender, EventArgs e)
        {
            shape = 2;//circle
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            shape = 10;
            while (drawinglock != 0) { }
            drawinglock = 1;
            try
            {
                if (pts.Count > 0)
                {
                    pts.RemoveAt(pts.Count - 1);
                }
            }
            catch (System.InvalidOperationException)
            {

            }
            drawinglock = 0;
        }

        private void FindMatch(Mat observedImage)
        {
            int k = 2;
            mask = new Mat();
            homography = null;
            matches = new VectorOfVectorOfDMatch();
            uObservedImage = observedImage.GetUMat(AccessType.ReadWrite);
               
            // extract features from the observed image
            ORBCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
            matcher = new BFMatcher(DistanceType.L2);
            matcher.Add(objDescriptors);
            if(objDescriptors.Size.Height>3 && observedDescriptors.Size.Height > 3)
            {
                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, 0.8, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    //nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(objKeyPoints, observedKeyPoints,
                     //matches, mask, 1, 2);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(objKeyPoints,
                            observedKeyPoints, matches, mask, 3);
                }
            }
        }

        private Mat Draw( Mat observedImage)
        {
            if (button1.Text == "UnLoad")
            {
                FindMatch(observedImage);
                Mat result = new Mat();

                if (matches.Size > 2)
                {

                    //Draw the matched keypoints

                    Features2DToolbox.DrawMatches(objImage, objKeyPoints, observedImage, observedKeyPoints, matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);
                    //Bgr rgb5 = new Bgr(250, 250, 250);
                    //Features2DToolbox.DrawKeypoints(observedImage, observedKeyPoints, observedImage, rgb5);
                    int i;
                    float X = 0;
                    float Y = 0;
                    int count = 0;
                    for ( i = 0; i < matches.Size; i++)
                    {
                        if (mask.GetData(i)[0] == 1)
                        {
                            X += observedKeyPoints[matches[i][0].QueryIdx].Point.X - objKeyPoints[matches[i][0].TrainIdx].Point.X;
                            Y += observedKeyPoints[matches[i][0].QueryIdx].Point.Y - objKeyPoints[matches[i][0].TrainIdx].Point.Y;
                            count++;
                        }
                    }
                    X = X / count + (float)(objwidth / 2.0);
                    Y = Y / count + (float)(objheight / 2.0);

                   
                    if (trackings[trackings.Length - 1].X >= 0)
                    {
                        Point[] newtrackings = new Point[2 * trackings.Length + 1];
                        for (i = 0; i < trackings.Length; i++)
                        {
                            newtrackings[i] = trackings[i];
                        }
                        newtrackings[i].X = (int)X;
                        newtrackings[i].Y = (int)Y;
                        for (i++; i < newtrackings.Length; i++)
                        {
                            newtrackings[i].X = -1;
                            newtrackings[i].Y = -1;
                        }
                        trackings = newtrackings;
                    }
                    else
                    {
                        for (i = 0; trackings[i].X >= 0; i++)
                        {
                        }
                        trackings[i].X = (int)X;
                        trackings[i].Y = (int)Y;
                    }

                    if (homography != null)
                    {
                        //draw a rectangle along the projected model

                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(Point.Empty, objImage.Size);
                        PointF[] pts = new PointF[]
                        {
                        new PointF(rect.Left, rect.Bottom),
                        new PointF(rect.Right, rect.Bottom),
                        new PointF(rect.Right, rect.Top),
                        new PointF(rect.Left, rect.Top)
                        };
                        pts = CvInvoke.PerspectiveTransform(pts, homography);

                        Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                        using (VectorOfPoint vp = new VectorOfPoint(points))
                        {

                            //CvInvoke.Polylines(observedImage, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                           }
                    }
                    float a = 2;
                    CvInvoke.Ellipse(result, new RotatedRect(new PointF(X, Y), new SizeF(10, 10), a), new MCvScalar(0, 250, 255, 255));

                    return result;
                }

            }
            else if (button1.Text == "Load")
            {
                uObservedImage = observedImage.GetUMat(AccessType.ReadWrite);
                // extract features from the observed image
                ORBCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);

                Bgr rgb1 = new Bgr(250, 250, 250);
                Features2DToolbox.DrawKeypoints(observedImage, observedKeyPoints, observedImage, rgb1);
            }
            return observedImage;
        }


        private int[] getMouseCoordinates(Bitmap img)
        {
            int[] wyj = new int[2];
            double imgScale = Math.Max((double)img.Height / viewFinder.Height, (double)img.Width / viewFinder.Width);
            int mouseX = System.Windows.Forms.Cursor.Position.X - this.Left - 10 - (int)((viewFinder.Width - img.Width / imgScale) / 2.0);
            int mouseY = System.Windows.Forms.Cursor.Position.Y - this.Top - 35 - (int)((viewFinder.Height - img.Height / imgScale) / 2.0);
            int minX = viewFinder.Left;//(int)(( viewFinder.Left / 2.0 + viewFinder.Right / 2.0 - viewFinder.Width / 2.0 ) * imgScale);
            int minY = viewFinder.Top;//(int)(( viewFinder.Bottom / 2.0 + viewFinder.Top / 2.0 - viewFinder.Height / 2.0) * imgScale);
            int maxX = viewFinder.Right;//(int)(( viewFinder.Left / 2.0 + viewFinder.Right / 2.0 + viewFinder.Width / 2.0 ) * imgScale);
            int maxY = viewFinder.Bottom;//(int)(( viewFinder.Bottom / 2.0 + viewFinder.Top / 2.0 + viewFinder.Height / 2.0) * imgScale);

            if (mouseX < minX)
            {
                mouseX = 0;
            }
            else if (mouseX > maxX)
            {
                mouseX = (int)((maxX - minX) * imgScale);
            }
            else
            {
                mouseX = (int)((mouseX - minX) * imgScale);
            }

            if (mouseY < minY)
            {
                mouseY = 0;
            }
            else if (mouseY > maxY)
            {
                mouseY = (int)((maxY - minY) * imgScale);
            }
            else
            {
                mouseY = (int)((mouseY - minY) * imgScale);
            }
            wyj[0] = mouseX;
            wyj[1] = mouseY;
            return wyj;
        }

        private void GUI_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (button1.Text == "Load") 
            {
                trackings = new Point[11];
                for (int i = 0; i < trackings.Length; i++)
                {
                    trackings[i].X= -1;
                    trackings[i].Y = -1;
                }

                button1.Text = "UnLoad";
                ORBCPU = new ORBDetector(400);
                objImage = uObservedImage.ToImage<Bgr, Byte>();
                objImage.ROI = roi;
                uObjImage = objImage.Mat.GetUMat(AccessType.ReadWrite);
                ORBCPU.DetectAndCompute(uObjImage, null, objKeyPoints, objDescriptors, false);
                shape = 20;
                ORBCPU = new ORBDetector(8000);
            }
            else if (button1.Text == "UnLoad" )
            {
                trackings = new Point[11];
                for (int i = 0; i < trackings.Length; i++)
                {
                    trackings[i].X = -1;
                    trackings[i].Y = -1;
                }
                objKeyPoints = new VectorOfKeyPoint();
                objDescriptors = new UMat();
                objheight = 0;
                objwidth = 0;
                shape = 20;
                button1.Text = "Object";
            }
            else if(button1.Text == "Object")
            {
                button1.Text = "Load";
                shape = 5;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            tickCount = 0;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            opaqCons = trackbar1.Value;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            shape = 6;//blood
        }

        private void countLabel_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            bloodSize = trackBar2.Value;
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                drawingPen.Color = colorDialog1.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                try
                {
                    saveFlag = 1;
                    fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    //Do whatever with the new path
                }
                catch (System.IO.IOException)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to Save File.");
                }
                return;
            }
        }
    }
}