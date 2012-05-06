using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Imaging;

namespace CCT.NUI.KinectSDK
{
    public class SDKDepthBitmapDataSource : SDKBitmapDataSource
    {
        public SDKDepthBitmapDataSource(IKinectSensor nuiRuntime)
            : base(nuiRuntime)
        { }

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
            this.Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
        }

        protected override void InnerStop()
        {
            this.Sensor.DepthFrameReady -= new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
        }

        unsafe void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenDepthImageFrame())
            {
                if (image != null)
                {
                    var data = new short[image.PixelDataLength];
                    image.CopyPixelDataTo(data);
                    BitmapData bitmapData = this.CurrentValue.LockBits(new System.Drawing.Rectangle(0, 0, this.Width, this.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    int pointer = 0;
                    int width = this.Width;
                    int height = this.Height;
                    for (int y = 0; y < height; y++)
                    {
                        byte* pDest = (byte*)bitmapData.Scan0.ToPointer() + y * bitmapData.Stride ;
                        for (int x = 0; x < width; x++, pointer++, pDest += 3)
                        {
                            int realDepth = data[pointer] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                            byte intensity = (byte)(~(realDepth >> 4));
                            pDest[0] = intensity;
                            pDest[1] = intensity;
                            pDest[2] = intensity;
                        }
                    }
                    this.CurrentValue.UnlockBits(bitmapData);
                    this.OnNewDataAvailable();
                }
            }
        }
    }
}
