using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace CCT.NUI.KinectSDK
{
    public interface IKinectSensor
    {
        int DepthStreamWidth { get; }

        int DepthStreamHeight { get; }

        int ColorStreamWidth { get; }

        int ColorStreamHeight { get; }

        event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady;

        event EventHandler<DepthImageFrameReadyEventArgs> DepthFrameReady;
    }
}
