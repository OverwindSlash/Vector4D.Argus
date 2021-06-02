using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intel.RealSense;
using OpenCvSharp;

namespace Argus.MultiSensorCalibration
{
    class RealSenseCapture
    {
        private PipelineProfile profile;
        private Pipeline pipeline;
        private Colorizer colorizer;
        private Align align;

        static int i = 0;

        public RealSenseCapture()
        {
            colorizer = new Colorizer();
            pipeline = new Pipeline();
            align = new Align(Stream.Color);
            Config();
        }
        private void Config()
        {
            using (var ctx = new Context())
            {
                var devices = ctx.QueryDevices();
                var dev = devices[0];

                Console.WriteLine("\nUsing device 0, an {0}", dev.Info[CameraInfo.Name]);
                Console.WriteLine("    Serial number: {0}", dev.Info[CameraInfo.SerialNumber]);
                Console.WriteLine("    Firmware version: {0}", dev.Info[CameraInfo.FirmwareVersion]);

                var sensors = dev.QuerySensors();
                var depthSensor = sensors[0];
                var colorSensor = sensors[1];

                var depthProfile = depthSensor.StreamProfiles
                                    .Where(p => p.Stream == Stream.Depth)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();

                var colorProfile = colorSensor.StreamProfiles
                                    .Where(p => p.Stream == Stream.Color)
                                    .OrderBy(p => p.Framerate)
                                    .Select(p => p.As<VideoStreamProfile>()).First();

                var cfg = new Config();
                cfg.EnableStream(Stream.Depth, 640, 480, depthProfile.Format, depthProfile.Framerate);
                cfg.EnableStream(Stream.Color, 640, 480, colorProfile.Format, colorProfile.Framerate);

                profile = pipeline.Start(cfg);
            }
        }

        public Point2f[] CaptureImageCorners()
        {
            using (var frames = pipeline.WaitForFrames())
            using (var color = frames.ColorFrame)
            {
                VideoFrame colorFrame = color.As<VideoFrame>();
                return GetCornersFromImage(colorFrame);
            }
        }
        public Point2f[] GetCornersFromImage(VideoFrame colorFrame)
        {
            using Mat c = new Mat(colorFrame.Height, colorFrame.Width, MatType.CV_8UC3, colorFrame.Data);
            using Mat rgb = new Mat();
            Cv2.CvtColor(c, rgb, ColorConversionCodes.BGR2RGB);
            return CalibrationTool.FindChessboardCorners(rgb);
        }
        /// <summary>
        /// 外部主要调用接口
        /// </summary>
        /// <returns></returns>
        public List<Point3f> CaptureCornerPoints()
        {
            using (var frames = pipeline.WaitForFrames())
            {
                FrameSet frameSet = align.Process<FrameSet>(frames).DisposeWith(frames);
                var depth = frameSet.DepthFrame.DisposeWith(frameSet);
                var color = frameSet.ColorFrame.DisposeWith(frameSet);

                var colorIntrinsic = profile.GetStream(Stream.Color).As<VideoStreamProfile>().GetIntrinsics();

                // 1 corners
                var corners = GetCornersFromImage(color.As<VideoFrame>());
                // 2 points
                var points = GetPoints(corners, depth, colorIntrinsic);
                return points;
            }
        }
        public void Capture()
        {
            using (var frames = pipeline.WaitForFrames())
            using (var depth = frames.DepthFrame)
            using (var color = frames.ColorFrame)
            {
                VideoFrame colorFrame = color.As<VideoFrame>();
                using Mat c = new Mat(colorFrame.Height, colorFrame.Width, MatType.CV_8UC3, colorFrame.Data);
                using Mat rgb = new Mat();
                Cv2.CvtColor(c, rgb, ColorConversionCodes.BGR2RGB);
                rgb.ImWrite("color_" + i + ".png");

                VideoFrame depthFrame = depth.As<VideoFrame>();
                var colorizedDepth = colorizer.Process<VideoFrame>(depthFrame).DisposeWith(frames);
                Mat d = new Mat(colorizedDepth.Height, colorizedDepth.Width, MatType.CV_16UC1, colorizedDepth.Data);
                d.ImWrite("depth_" + i + ".png");

                i++;
            }
        }

        public void CaptureAndAlign()
        {
            using (var frames = pipeline.WaitForFrames())
            {
                FrameSet frameSet = align.Process<FrameSet>(frames).DisposeWith(frames);
                var depth = frameSet.DepthFrame.As<VideoFrame>().DisposeWith(frameSet);
                var color = frameSet.ColorFrame.As<VideoFrame>().DisposeWith(frameSet);

                using Mat c = new Mat(color.Height, color.Width, MatType.CV_8UC3, color.Data);
                using Mat rgb = new Mat();
                Cv2.CvtColor(c, rgb, ColorConversionCodes.BGR2RGB);
                rgb.ImWrite("color_" + i + ".png");

                var colorizedDepth = colorizer.Process<VideoFrame>(depth).DisposeWith(frameSet);
                Mat d = new Mat(colorizedDepth.Height, colorizedDepth.Width, MatType.CV_16UC1, colorizedDepth.Data);
                d.ImWrite("depth_" + i + ".png");
                i++;
            }

        }

        public List<Point3f> GetPoints(Point2f[] corners, DepthFrame depth, Intrinsics colorIntrinsic)
        {
            List<Point3f> points = new List<Point3f>();
            foreach (var corner in corners)
            {
                int u = (int)corner.X;
                int v = (int)corner.Y;

                var distance = depth.GetDistance(u, v);
                int count = 0;
                float d = 0.0f;
                if (distance <= 0.0)
                {
                    for (int ui = -10; ui < 10; ui++)
                    {
                        for (int vi = -10; vi < 10; vi++)
                        {
                            float dd = depth.GetDistance(ui + u, vi + v);
                            if (dd <= 0.0)
                            {
                                continue;
                            }
                            d += dd;
                            count++;
                        }
                    }
                    if (count > 200)
                    {
                        distance = d / count;
                    }
                }
                else
                {
                    //Console.WriteLine("distance is " + distance);
                    var point = DeprojectPixelToPoint(colorIntrinsic, u, v, distance);
                    Point3f p = new Point3f(point[0], point[1], point[2]);
                    points.Add(p);
                }
            }
            return points;
        }
        public float[] GetPoint(int u, int v)
        {
            using (var frames = pipeline.WaitForFrames())
            {
                FrameSet frameSet = align.Process<FrameSet>(frames).DisposeWith(frames);
                var depth = frameSet.DepthFrame.DisposeWith(frameSet);
                var color = frameSet.ColorFrame.DisposeWith(frameSet);

                // Intrinsics & Extrinsics
                //var depthIntrinsic = profile.GetStream(Stream.Depth).As<VideoStreamProfile>().GetIntrinsics();
                //Console.WriteLine(depthIntrinsic);
                var colorIntrinsic = profile.GetStream(Stream.Color).As<VideoStreamProfile>().GetIntrinsics();
                //var depthToColorExtrinsic = depth.Profile.GetExtrinsicsTo(color.Profile);

                var distance = depth.GetDistance(u, v);
                int count = 0;
                float d = 0.0f;
                if (distance <= 0.0)
                {
                    for (int ui = -10; ui < 10; ui++)
                    {
                        for (int vi = -10; vi < 10; vi++)
                        {
                            float dd = depth.GetDistance(ui + u, vi + v);
                            if (dd <= 0.0)
                            {
                                continue;
                            }
                            d += dd;
                            count++;
                        }
                    }
                    if (count > 200)
                    {
                        distance = d / count;
                    }
                }

                //Console.WriteLine("distance is " + distance);
                var point = DeprojectPixelToPoint(colorIntrinsic, u, v, distance);

                Console.WriteLine(string.Format("{0}-{1}-{2}", point[0], point[1], point[2]));
                return point;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="intrin"></param>
        /// <param name="u">pixel x</param>
        /// <param name="v">pixel y</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public float[] DeprojectPixelToPoint(Intrinsics intrin, int u, int v, float depth)
        {
            float[] point = new float[3];

            //if (intrin.model != Distortion.ModifiedBrownConrady)
            //{
            //    return point;
            //} // Cannot deproject from a forward-distorted image
            //if (intrin.model != Distortion.BrownConrady)
            //{
            //    return point;
            //}// Cannot deproject to an brown conrady model

            float x = (u - intrin.ppx) / intrin.fx;
            float y = (v - intrin.ppy) / intrin.fy;
            if (intrin.model == Distortion.InverseBrownConrady)
            {
                float r2 = x * x + y * y;
                float f = 1 + intrin.coeffs[0] * r2 + intrin.coeffs[1] * r2 * r2 + intrin.coeffs[4] * r2 * r2 * r2;
                float ux = x * f + 2 * intrin.coeffs[2] * x * y + intrin.coeffs[3] * (r2 + 2 * x * x);
                float uy = y * f + 2 * intrin.coeffs[3] * x * y + intrin.coeffs[2] * (r2 + 2 * y * y);
                x = ux;
                y = uy;
            }
            if (intrin.model == Distortion.KannalaBrandt4)
            {
                float rd = (float)Math.Sqrt((double)x * x + y * y);
                if (rd < float.Epsilon)
                {
                    rd = float.Epsilon;
                }

                float theta = rd;
                float theta2 = rd * rd;
                for (int i = 0; i < 4; i++)
                {
                    float f = theta * (1 + theta2 * (intrin.coeffs[0] + theta2 * (intrin.coeffs[1] + theta2 * (intrin.coeffs[2] + theta2 * intrin.coeffs[3])))) - rd;
                    if (Math.Abs(f) < float.Epsilon)
                    {
                        break;
                    }
                    float df = 1 + theta2 * (3 * intrin.coeffs[0] + theta2 * (5 * intrin.coeffs[1] + theta2 * (7 * intrin.coeffs[2] + 9 * theta2 * intrin.coeffs[3])));
                    theta -= f / df;
                    theta2 = theta * theta;
                }
                float r = (float)Math.Tan((double)theta);
                x *= r / rd;
                y *= r / rd;
            }
            if (intrin.model == Distortion.Ftheta)
            {
                float rd = (float)Math.Sqrt((double)x * x + y * y);
                if (rd < float.Epsilon)
                {
                    rd = float.Epsilon;
                }
                float r = (float)(Math.Tan(intrin.coeffs[0] * rd) / Math.Atan(2 * Math.Tan(intrin.coeffs[0] / 2.0)));
                x *= r / rd;
                y *= r / rd;
            }

            point[0] = depth * x;
            point[1] = depth * y;
            point[2] = depth;

            return point;
        }


    }
}
