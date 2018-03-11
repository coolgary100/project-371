using System;
using System.Collections.Generic;
using System.Text;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;

namespace cam_aforge1
{
    class CamControl
    {
        public static void show_Controls()
        {
            VideoCaptureDevice Cam1;
            FilterInfoCollection VideoCaptureDevices;

            VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            Cam1 = new VideoCaptureDevice(VideoCaptureDevices[0].MonikerString);
            Cam1.DisplayPropertyPage(IntPtr.Zero); //This will display a form with camera controls
        }
    }
}
