using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;

namespace cam_aforge1
{
    public partial class GUI : Form
    {

        private Point MouseDownLocation;
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        GUIElements myCanvas;

        int tickCount = 0;
        int drawinglock = 0;
        List<int[]> pts = new List<int[]>();
        int shape; //0(free) 1(line) 2(circle)
        Pen drawingPen = new Pen(Color.Black, 3);
        Pen curserPen = new Pen(Color.Red, 3);

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

        //Generally don't have to change this
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            myCanvas.g = Graphics.FromImage(img);
            try
            {
                int[] mouse = getMouseCoordinates(img);
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(mouse[0], mouse[1], 5, 5);
                myCanvas.g.DrawEllipse(curserPen, rec);

                while (drawinglock != 0) { }
                drawinglock = 1;

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
                    if (point[0] == 1)//draw line
                    {
                        myCanvas.g.DrawLine(drawingPen, point[1], point[2], point[3], point[4]);
                    }
                    if (point[0] == 2)//draw circle
                    {
                        myCanvas.g.DrawEllipse(drawingPen, point[1] - point[3], point[2] - point[3], 2 * point[3], 2 * point[3]);
                    }
                }
                drawinglock = 0;
            }
            catch
            {

            }
            
            myCanvas.Run();
            viewFinder.Image = img;
            myCanvas.g.Dispose();
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
                    }else if (shape == 2)
                    {
                        int[] pt = new int[] { 2, Xm, Ym, 0 };
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
                    int[] pt = pts[pts.Count - 1];

                    if (pt[0] == 0)//free drawing
                    {
                        int i;
                        if (pt[pt.Length - 1] >= 0)//increse the size
                        {
                            int[] newpt = new int[2 * pt.Length + 1];
                            for(i = 0; i < pt.Length; i++)
                            {
                                newpt[i] =pt[i];
                            }
                            newpt[i] = Xm;
                            newpt[i + 1] = Ym;
                            for (i=i+2; i < newpt.Length; i++)
                            {
                                newpt[i] = -1;
                            }
                            pts.RemoveAt(pts.Count - 1);
                            pts.Add(newpt);
                        }
                        else
                        {
                            for (i=0; pt[i] >= 0; i++)
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
            //Step 7: Let's activate this button. Uncomment lines 143-144 to add functionality
            //to the button.
            //tickCount++;
            //countDisp.Text = tickCount.ToString();

            //Step 8: The button_Click method can be used to call a method that you wrote
            //on the GUIElements Class. Uncomment line 149 to call the ButtonWasClicked method
            //from GUIElements.
            //myCanvas.Run();


            //Note that the syntax for calling methods in the GUIElements class is always in the form of
            //`myCanvas.NameOfMethod();`.
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            shape = 10;
            pts.RemoveAt(pts.Count - 1);
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

    }
}