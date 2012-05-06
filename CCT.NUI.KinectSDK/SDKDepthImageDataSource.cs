using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows;

namespace CCT.NUI.KinectSDK
{
    public class SDKDepthImageDataSource : SDKImageDataSource
    {
        private byte[] depthFrame32;
        private short[] data;

        public SDKDepthImageDataSource(IKinectSensor nuiRuntime)
            : base(nuiRuntime)
        {
            this.depthFrame32 = new byte[Width * Height * 4];
        }

        public override int Width
        {
            get { return this.Sensor.DepthStreamWidth; }
        }

        public override int Height
        {
            get { return this.Sensor.DepthStreamHeight; }
        }

        protected override void InnerStart()
        {
            this.Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(Sensor_DepthFrameReady);
        }

        protected override void InnerStop()
        {
            this.Sensor.DepthFrameReady -= new EventHandler<DepthImageFrameReadyEventArgs>(Sensor_DepthFrameReady);
        }

        void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenDepthImageFrame())
            {
                if (image != null)
                {
                    if (this.data == null)
                    {
                        this.data = new short[image.PixelDataLength];
                    }
                    image.CopyPixelDataTo(this.data);
                    this.ConvertData(this.data);
                    this.writeableBitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height), this.depthFrame32, image.Width * 4, 0);
                    this.OnNewDataAvailable();
                }
            }
        }

        private void ConvertData(short[] data)
        {
            for (int i16 = 0, i32 = 0; i16 < data.Length && i32 < this.depthFrame32.Length; i16++, i32 += 4)
            {
                int realDepth = data[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                byte intensity = (byte)(~(realDepth >> 4));

                this.depthFrame32[i32 + 2] = intensity;
                this.depthFrame32[i32 + 1] = intensity;
                this.depthFrame32[i32] = intensity;
            }
        }
    }
}
