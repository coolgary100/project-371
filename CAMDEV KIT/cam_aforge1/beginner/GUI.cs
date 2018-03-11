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
        int shape; //0(circle) 1(line)
        Pen drawingPen = new Pen(Color.Black, 3);
        Pen curserPen = new Pen(Color.Red, 3);

        //Constructs the gui
        public GUI()
        {
            myCanvas = new GUIElements(this);
            InitializeComponent();
            this.DoubleBuffered = true;
            //Add custom events here, ie
            //viewFinder.MouseDown += new MouseEventHandler(viewFinder_MouseDown);
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
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, 5, 5);
            myCanvas.g.DrawEllipse(curserPen, rec);

            while (drawinglock != 0) { }
            drawinglock = 1;

            foreach (int[] point in pts)
            {
                if (point[0] == 0)//draw circle
                {
                    myCanvas.g.DrawEllipse(drawingPen, point[1] - point[3] / 2, point[2] - point[3] / 2, point[3], point[3]);
                }
                if (point[0] == 1)//draw line
                {
                    myCanvas.g.DrawLine(drawingPen, point[1], point[2], point[3], point[4]);
                }

            }
            drawinglock = 0;
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
        private void button3_Click(object sender, EventArgs e)
        {
            //Step 7: Let's activate this button. Uncomment lines 143-144 to add functionality
            //to the button.
            tickCount++;
            countDisp.Text = tickCount.ToString();

            //Step 8: The button_Click method can be used to call a method that you wrote
            //on the GUIElements Class. Uncomment line 149 to call the ButtonWasClicked method
            //from GUIElements.
            //myCanvas.Run();


            //Note that the syntax for calling methods in the GUIElements class is always in the form of
            //`myCanvas.NameOfMethod();`.
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            while (drawinglock!=0) { }
            drawinglock = 1;
            if (e.Button == MouseButtons.Left)
            //can also use this one:
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int Xm = System.Windows.Forms.Cursor.Position.X;
                int Ym = System.Windows.Forms.Cursor.Position.Y;
                int[] pt = new int[] { 1, Xm, Ym, Xm, Ym };
                pts.Add(pt);
                MouseDownLocation = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                int Xm = System.Windows.Forms.Cursor.Position.X;
                int Ym = System.Windows.Forms.Cursor.Position.Y;
                int[] pt = new int[] { 0, Xm, Ym, 1 };
                pts.Add(pt);
                MouseDownLocation = e.Location;
            }
            drawinglock = 0;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            while (drawinglock != 0) { }
            drawinglock = 1;
            int Xm = System.Windows.Forms.Cursor.Position.X;
            int Ym = System.Windows.Forms.Cursor.Position.Y;
            if (e.Button == MouseButtons.Left)
            {
                int[] pt = pts[pts.Count - 1];
                pt[3] = Xm;
                pt[4] = Ym;
                pts.Add(pt);
                MouseDownLocation = e.Location; 
            }
            else if (e.Button == MouseButtons.Right)
            {
                int[] pt = pts[pts.Count - 1];
                int size = (int) Math.Sqrt((Xm - pt[1] - pt[3] / 2) * (Xm - pt[1] - pt[3] / 2) + (Ym - pt[2] - pt[3] / 2) * (Ym - pt[2] - pt[3] / 2));
                pts.RemoveAt(pts.Count - 1);
                pt[3] = size;
                pts.Add(pt);
                MouseDownLocation = e.Location;
            }
            drawinglock = 0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void viewFinder_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            shape = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            shape = 1;
        }
    }
}