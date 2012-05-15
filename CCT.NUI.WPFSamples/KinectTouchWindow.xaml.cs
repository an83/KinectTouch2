using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using CCT.NUI.Core;
using CCT.NUI.Core.Video;
using CCT.NUI.Visual;
using CCT.NUI.HandTracking;
using CCT.NUI.KinectSDK;
using CCT.NUI.Core.Clustering;
using Microsoft.Kinect.Samples.CursorControl;
using Point = CCT.NUI.Core.Point;
using Size = CCT.NUI.Core.Size;

namespace CCT.NUI.WPFSamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class KinectTouchWindow : Window
    {
        private IDataSourceFactory factory;
        private IHandDataSource handDataSource;
        private IClusterDataSource clusterDataSource;
        private IImageDataSource rgbImageDataSource;
        //private MouseController mouseController;
        private int lastFingerCount;
        private DateTime lastUpdate = DateTime.Now;

        public KinectTouchWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Start()
        {
            this.Cursor = Cursors.Wait;
            this.checkClusterLayer.IsEnabled = true;
            this.checkHandLayer.IsEnabled = true;
            

            this.factory = new SDKDataSourceFactory();

            this.clusterDataSource = this.factory.CreateClusterDataSource(new ClusterDataSourceSettings { MaximumDepthThreshold = 1000 });
            this.handDataSource = new HandDataSource(this.factory.CreateShapeDataSource(this.clusterDataSource, new Core.Shape.ShapeDataSourceSettings()));
            //this.rgbImageDataSource = this.factory.CreateRGBImageDataSource();
            //this.rgbImageDataSource.Start();

            var depthImageSource = this.factory.CreateDepthImageDataSource();
            depthImageSource.NewDataAvailable += new NewDataHandler<ImageSource>(MainWindow_NewDataAvailable);
            depthImageSource.Start();
            handDataSource.Start();

            //this.mouseController = new MouseController(handDataSource,true);

            this.Cursor = Cursors.Arrow;

            handDataSource.NewDataAvailable += new NewDataHandler<HandCollection>(handDataSource_NewDataAvailable);
        }

        private TouchController controller = new TouchController();

        private void OnTouchStart(int x, int y)
        {
            Debug.WriteLine("start. location: {0},{1}",x,y);
            controller.TouchDown(x, y);
        }

        private void OnTouchMove(int x, int y)
        {
            Debug.WriteLine("move. location: {0},{1}", x, y);
            controller.TouchDrag(x,y);
        }

        private void OnTouchEnd(int x, int y)
        {
            Debug.WriteLine("end. location: {0},{1}", x, y);
            controller.TouchUp();
        }

        private static readonly int ScreenWidth = (int) SystemParameters.PrimaryScreenWidth;
        private static readonly int ScreenHeight = (int) SystemParameters.PrimaryScreenHeight;

        private const int TouchGestureFingerCount = 2;
        private const int MoveGestureFingerCount = 0;

        private void handDataSource_NewDataAvailable(HandCollection data)
        {
            if (data.HandsDetected)
            {
                var hand = data.Hands.Last();

                var location = MapToScreen(hand.Location);

                //if (hand.HasPalmPoint)
                //{
                //    location = MapToScreen(hand.PalmPoint.Value);
                //}
                //else
                //{
                //    return;
                //}

                Debug.WriteLine("timespan: {0}, fingerCount: {2}, lastFingerCount: {3}, location: {4}",
                    DateTime.Now - lastUpdate,
                    lastUpdate,
                    hand.FingerCount,
                    lastFingerCount,
                    location);

                if (DateTime.Now > lastUpdate.AddMilliseconds(100))
                {
                    lastUpdate = DateTime.Now;

                    int x = (int)location.X,
                        y = (int)location.Y;

                    //set mouse position
                    MouseController.SendMouseInput(x, y, ScreenWidth, ScreenHeight, false);

                    var fingerCountChanged = lastFingerCount != hand.FingerCount;
                    lastFingerCount = hand.FingerCount;

                    if (fingerCountChanged)
                    {
                        if (hand.FingerCount == TouchGestureFingerCount)
                        {
                            //start
                            OnTouchStart(x, y);
                        }
                        else if (hand.FingerCount == MoveGestureFingerCount)
                        {
                            //end
                            OnTouchEnd(x, y);
                        }
                    }
                    else if (hand.FingerCount == TouchGestureFingerCount)
                    {
                        // update touch
                        OnTouchMove(x, y);
                    }
                }

                this.Dispatcher.Invoke(() =>
                                           {
                                               this.labelHandLocation.Content = location.ToString();
                                               this.labelFingerCount.Content = hand.FingerCount;
                                           });
            }
        }

        private Point MapToScreen(Point point)
        {
            var originalSize = new Size(this.handDataSource.Width, this.handDataSource.Height);
            return new Point(
                -50 + (float)(point.X / originalSize.Width * (SystemParameters.PrimaryScreenWidth + 100)), 
                -50 + (float)(point.Y / originalSize.Height * (SystemParameters.PrimaryScreenHeight + 100)), 
                point.Z);
        }

        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = (((((float)maxPixel) / maxSkeleton) / 2f) * position) + (maxPixel / 2);
            if (value > maxPixel)
            {
                return (float)maxPixel;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        void MainWindow_NewDataAvailable(ImageSource data)
        {
            this.videoControl.Dispatcher.Invoke(new Action(() =>
            {
                this.videoControl.ShowImageSource(data);
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new Action(() =>
            {
                if (this.handDataSource!=null)
                    this.handDataSource.Stop();

                if (this.factory != null)
                    this.factory.DisposeAll();
                
            }).BeginInvoke(null, null);
        }
        
        private void checkHandLayer_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleLayers();
        }

        private void checkClusterLayer_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleLayers();
        }

        private void ToggleLayers()
        {
            var layers = new List<IWpfLayer>();
            if (this.checkHandLayer.IsChecked.GetValueOrDefault())
            {
                layers.Add(new WpfHandLayer(this.handDataSource));
            }
            if (this.checkClusterLayer.IsChecked.GetValueOrDefault())
            {
                layers.Add(new WpfClusterLayer(this.clusterDataSource));
            }
            this.videoControl.Layers = layers;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Start();
                buttonStart.IsEnabled = false;
                
                checkClusterLayer.IsEnabled = true;
                checkHandLayer.IsEnabled = true;
                ToggleLayers();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
