using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace CCT.NUI.KinectSDK
{
    public class SDKRgbImageDataSource : SDKImageDataSource
    {
        private byte[] data;

        public SDKRgbImageDataSource(IKinectSensor nuiRuntime)
            : base(nuiRuntime)
        { }

        public override int Width
        {
            get { return this.Sensor.ColorStreamWidth; }
        }

        public override int Height
        {
            get { return this.Sensor.ColorStreamHeight; }
        }

        protected override void InnerStart()
        {
            this.Sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(Sensor_ColorFrameReady);
        }

        protected override void InnerStop()
        {
            this.Sensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(Sensor_ColorFrameReady);
        }

        void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenColorImageFrame())
            {
                if (image != null)
                {
                    if (this.data == null)
                    {
                        this.data = new byte[image.PixelDataLength];
                    }
                    image.CopyPixelDataTo(this.data);

                    this.writeableBitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height), data, image.Width * image.BytesPerPixel, 0);
                    this.OnNewDataAvailable();
                }
            }
        }
    }
}
