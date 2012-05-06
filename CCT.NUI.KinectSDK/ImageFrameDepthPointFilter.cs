using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using CCT.NUI.Core;
using CCT.NUI.Core.Clustering;

namespace CCT.NUI.KinectSDK
{
    public class ImageFrameDepthPointFilter : IDepthPointFilter<DepthImageFrame>
    {
        private IKinectSensor sensor;
        private IntSize size;
        private int minimumDepthThreshold;
        private int maximumDepthThreshold;
        private int lowerBorder;

        private short[] data;

        public ImageFrameDepthPointFilter(IKinectSensor sensor, IntSize size, int minimumDepthThreshold, int maximumDepthThreshold, int lowerBorder)
        {
            this.sensor = sensor;
            this.size = size;
            this.minimumDepthThreshold = minimumDepthThreshold;
            this.maximumDepthThreshold = maximumDepthThreshold;
            this.lowerBorder = lowerBorder;
        }

        public IList<Point> Filter(DepthImageFrame source)
        {
            var result = new List<Point>();

            var localHeight = this.size.Height; //5ms faster when it's a local variable
            var localWidth = this.size.Width;
            var maxY = localHeight - this.lowerBorder;
            var minDepth = this.minimumDepthThreshold;
            var maxDepth = this.maximumDepthThreshold;
            if (this.data == null)
            {
                this.data = new short[source.PixelDataLength];
            }
            source.CopyPixelDataTo(this.data);
            var pointer = 0;

            for (int y = 0; y < localHeight; y++)
            {
                for (int x = 0; x < localWidth; x++)
                {
                    int realDepth = data[pointer] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                    if (realDepth <= maxDepth && realDepth >= minDepth && y < maxY) //Should not be put in a seperate method for performance reasons
                    {
                        result.Add(new Point(x, y, realDepth));
                    }
                    pointer++;
                }
            }
            return result;
        }
    }
}
