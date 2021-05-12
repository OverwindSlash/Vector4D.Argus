using RosSharp;
using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Media.Imaging;
using OpenCvSharp;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class HandEyeCalibrationControlViewModel : ViewModelBase
    {
        private Bitmap _leftImage;

        public Bitmap LeftImage
        {
            get => _leftImage;
            set => this.RaiseAndSetIfChanged(ref _leftImage, value);
        }

        public HandEyeCalibrationControlViewModel()
        {
            Ros.MasterUri = new Uri("http://10.10.20.18:11311");
            Ros.HostName = "10.10.20.72";
            Ros.TopicTimeout = 10000;
            Ros.XmlRpcTimeout = 10000;


            int i = 0;
            var node = Ros.InitNodeAsync("/test").Result;
            var subscriber = node.SubscriberAsync<RosSharp.sensor_msgs.Image>("/body_right/image_raw").Result;
            subscriber.Subscribe(x =>
            { 
                int columns = (int)x.width;
                int rows = (int)x.height;

                Mat image = new Mat(rows, columns, MatType.CV_8UC3, x.data.Skip(4).ToArray());
                LeftImage = new Bitmap(image.ToMemoryStream());
                image.Dispose();
            });
        }
    }
}
