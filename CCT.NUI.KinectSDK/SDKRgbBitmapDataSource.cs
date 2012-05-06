using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Imaging;

namespace CCT.NUI.KinectSDK
{
    public class SDKRgbBitmapDataSource : SDKBitmapDataSource
    {
        public SDKRgbBitmapDataSource(IKinectSensor sensor)
            : base(sensor)
        { }

        public override int Width
        {
            get { return this.Sensor.ColorStreamWidth; }
        }

        public override int Height
        {
            get { return this.Sensor.ColorStreamHeight; }
        }

        protected override void  InnerStart()
        {
            this.Sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
        }

        protected override void InnerStop()
        {
            this.Sensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
        }

        protected void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    this.ProcessFrame(frame);
                }
            }
        }

        protected unsafe void ProcessFrame(ColorImageFrame frame)
        {
            var bytes = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(bytes);

            BitmapData bitmapData = this.CurrentValue.LockBits(new System.Drawing.Rectangle(0, 0, this.Width, this.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte* pDest = (byte*)bitmapData.Scan0.ToPointer();
            int pointer = 0;

            var maxIndex = this.Width * this.Height;
            for (int index = 0; index < maxIndex; index++)
            {
                pDest[0] = bytes[pointer];
                pDest[1] = bytes[pointer + 1];
                pDest[2] = bytes[pointer + 2];
                pDest += 3;
                pointer += 4;
            }
            this.CurrentValue.UnlockBits(bitmapData);
            this.OnNewDataAvailable();
        }
    }
}
