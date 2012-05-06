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
        private Point? cursorLocation;
        private int lastFingerCount;
        private DateTime lastUpdate = DateTime.Now;

        private object syncRoot = new object();

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

            this.clusterDataSource = this.factory.CreateClusterDataSource(new ClusterDataSourceSettings { MaximumDepthThreshold = 900 });
            this.handDataSource = new HandDataSource(this.factory.CreateShapeDataSource(this.clusterDataSource, new Core.Shape.ShapeDataSourceSettings()));
            this.rgbImageDataSource = this.factory.CreateRGBImageDataSource();
            this.rgbImageDataSource.Start();

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
            Debug.WriteLine(string.Format("start. location: {0},{1}",x,y));
            controller.TouchDown(x, y);
        }

        private void OnTouchMove(int x, int y)
        {
            Debug.WriteLine(string.Format("move. location: {0},{1}", x, y));
            controller.TouchDrag(x,y);
        }

        private void OnTouchEnd(int x, int y)
        {
            Debug.WriteLine(string.Format("end. location: {0},{1}", x, y));
            controller.TouchUp();
        }

        private static int screenWidth = (int) SystemParameters.PrimaryScreenWidth;
        private static int screenHeight = (int) SystemParameters.PrimaryScreenHeight;

        private void handDataSource_NewDataAvailable(HandCollection data)
        {
            if (data.HandsDetected)
            {
                //TODO: get left hand only
                var hand = data.Hands.Last();

                //scale to screen resolution
                var location = MapToScreen(hand.Location);

                //color this point 

                Debug.WriteLine("timespan: {0}, fingerCount: {2}, lastFingerCount: {3}",
                    DateTime.Now - lastUpdate,
                    lastUpdate,
                    hand.FingerCount,
                    lastFingerCount);

                if (DateTime.Now > lastUpdate.AddMilliseconds(100))
                {
                    lastUpdate = DateTime.Now;

                    cursorLocation = location;

                    int x = (int)location.X,
                        y = (int)location.Y;

                    //set mouse position
                    MouseController.SendMouseInput(x, y, screenWidth, screenHeight, false);

                    var fingerCountChanged = lastFingerCount != hand.FingerCount;
                    lastFingerCount = hand.FingerCount;

                    //if (hand.FingerCount == 2 && fingerCountChanged)
                    //{
                    //    //TouchController.Touch(x, y); Debug.WriteLine("touch!");

                    //    OnTouchStart(x, y);
                    //    OnTouchEnd(x, y);
                    //}

                    if (fingerCountChanged)
                    {
                        if (hand.FingerCount ==2)
                        {
                            //start
                            OnTouchStart(x, y);
                        }
                        else if (hand.FingerCount == 0)
                        {
                            //end
                            OnTouchEnd(x, y);
                        }
                    }
                    else if (hand.FingerCount ==2)
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


                /*  find hand
                 *  map location to hand
                 *  if finger count is 1 or more apply touch
                 */

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
                this.handDataSource.Stop();
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
            Start();
            ToggleLayers();
        }



    }
}
