using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using CCT.NUI.Core;
using CCT.NUI.Core.Clustering;
using System.Collections.Concurrent;

namespace CCT.NUI.KinectSDK
{
    public class SDKClusterDataSource : SensorDataSource<ClusterCollection>, IClusterDataSource
    {
        private IClusterFactory clusterFactory;
        private IDepthPointFilter<DepthImageFrame> filter;
        private ConcurrentQueue<DepthImageFrame> queue;
        private ActionRunner runner;

        public SDKClusterDataSource(IKinectSensor nuiRuntime, IClusterFactory clusterFactory, IDepthPointFilter<DepthImageFrame> filter)
            : base(nuiRuntime)
        {
            this.CurrentValue = new ClusterCollection();
            this.clusterFactory = clusterFactory;
            this.filter = filter;
            this.queue = new ConcurrentQueue<DepthImageFrame>();
            this.runner = new ActionRunner(() => Process());
        }

        public override int Width
        {
            get { return this.Sensor.DepthStreamWidth; }
        }

        public override int Height
        {
            get { return this.Sensor.DepthStreamHeight; }
        }

        protected ClusterCollection Process(DepthImageFrame image)
        {
            return this.clusterFactory.Create(this.FindPointsWithinDepthRange(image));
        }

        protected virtual IList<Point> FindPointsWithinDepthRange(DepthImageFrame image)
        {
            return this.filter.Filter(image);
        }

        protected override void InnerStart()
        {
            this.Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(nuiRuntime_DepthFrameReady);
            this.runner.Start();
        }

        protected override void InnerStop()
        {
            this.Sensor.DepthFrameReady -= new EventHandler<DepthImageFrameReadyEventArgs>(nuiRuntime_DepthFrameReady);
            this.runner.Stop();
        }

        private void Process()
        {
            DepthImageFrame frame;
            if (this.queue.TryDequeue(out frame))
            {
                this.CurrentValue = this.Process(frame);
                frame.Dispose();
                this.OnNewDataAvailable();
            }
        }

        private void nuiRuntime_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            var frame = e.OpenDepthImageFrame();
            if (frame != null)
            {
                this.queue.Enqueue(frame);
                //System.Diagnostics.Debug.WriteLine(this.queue.Count);
            }
        }
    }
}
