using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace CCT.NUI.KinectSDK
{
    public class KinectSensorAdapter : IKinectSensor, IDisposable
    {
        private KinectSensor sensor;

        public KinectSensorAdapter(KinectSensor sensor)
        {
            this.sensor = sensor;
            //this.sensor.DepthStream.Range = DepthRange.Near;
            this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            this.sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
        }

        public void Dispose()
        {
            this.sensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            this.sensor.DepthFrameReady -= new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
        }

        public int DepthStreamWidth
        {
            get { return this.sensor.DepthStream.FrameWidth; }
        }

        public int DepthStreamHeight
        {
            get { return this.sensor.DepthStream.FrameHeight; }
        }

        public int ColorStreamWidth
        {
            get { return this.sensor.ColorStream.FrameWidth; }
        }

        public int ColorStreamHeight
        {
            get { return this.sensor.ColorStream.FrameHeight; }
        }

        public event EventHandler<DepthImageFrameReadyEventArgs> DepthFrameReady;

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (this.DepthFrameReady != null)
            {
                this.DepthFrameReady(this, e);
            }
        }

        public event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady;

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (this.ColorFrameReady != null)
            {
                this.ColorFrameReady(this, e);
            }
        }
    }
}
