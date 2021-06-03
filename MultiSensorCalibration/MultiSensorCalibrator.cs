using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace Argus.MultiSensorCalibration
{
    public class MultiSensorCalibrator
    {
        private static SensorConfig sensorConfig;
        private static Size patternSize;
        private static Size stereoImageSize;
        public delegate void LogCallBack(string log);
        private static LogCallBack logCallBack;

        static MultiSensorCalibrator()
        {
            sensorConfig = new SensorConfig();
            patternSize = new Size(sensorConfig.ChessboardWidth,sensorConfig.ChessboardHeight);
            stereoImageSize = new Size(sensorConfig.StereoImageWidth, sensorConfig.StereoImageHeight);
        }
        public static void SetLogCallback(LogCallBack callback)
        {
            logCallBack = callback;
        }

        public static List<Point3f> GetPointCloudFromRealSense()
        {
            return new RealSenseCapture().CaptureCornerPoints();
        }

        public static List<Point3f> GetPointCloudFromStereo(string leftImageFile, string rightImageFile, string parameterFile)
        {
            // 1 undistortion
            using Mat leftImage = new Mat(leftImageFile);
            using Mat rightImage = new Mat(rightImageFile);

            using var fs = new FileStorage(parameterFile, FileStorage.Modes.Read);
            
            using Mat M1 = fs["M1"].ReadMat();
            using Mat M2 = fs["M2"].ReadMat();
            using Mat D1 = fs["D1"].ReadMat();
            using Mat D2 = fs["D2"].ReadMat();
            using Mat R1 = fs["R1"].ReadMat();
            using Mat R2 = fs["R2"].ReadMat();
            using Mat P1 = fs["Q1"].ReadMat();
            using Mat P2 = fs["Q2"].ReadMat();

            using Mat<float> mapLeftX = new Mat<float>(Mat.Eye(stereoImageSize.Height, stereoImageSize.Width, MatType.CV_32FC3));
            using Mat<float> mapLeftY = new Mat<float>(Mat.Eye(stereoImageSize.Height, stereoImageSize.Width, MatType.CV_32FC3));
            Cv2.InitUndistortRectifyMap(M1, D1, R1, P1, stereoImageSize, MatType.CV_32FC3, mapLeftX, mapLeftY);

            using Mat<float> mapRightX = new Mat<float>(Mat.Eye(stereoImageSize.Height, stereoImageSize.Width, MatType.CV_32FC3));
            using Mat<float> mapRightY = new Mat<float>(Mat.Eye(stereoImageSize.Height, stereoImageSize.Width, MatType.CV_32FC3));
            Cv2.InitUndistortRectifyMap(M2, D2, R2, P2, stereoImageSize, MatType.CV_32FC3, mapRightX, mapRightY);

            using Mat undistLeftImg = new Mat();
            using Mat undistRightImg = new Mat();

            Cv2.Remap(leftImage, undistLeftImg, mapLeftX, mapLeftY);
            Cv2.Remap(rightImage, undistRightImg, mapRightX, mapRightY);

            var points = FindCorner3dPoints(undistLeftImg, undistRightImg, P1, P2);

            return points;
        }

        //public static (Mat<float> Q, Mat<float> T) CalibrateRealSenseAndStereo()
        //{
        //    //var rsPoints = GetPointCloudFromRealSense();
        //    //var stereoPoints = GetPointCloudFromStereo();
        //    return CalibrateTwoPointCloud(rsPoints, stereoPoints);
        //}


        static List<Point3f> FindCorner3dPoints(Mat leftImg, Mat rightImg, Mat P1, Mat P2)
        {
            List<Point3f> points = new List<Point3f>();
            using Mat leftGrayImage = new Mat();
            Cv2.CvtColor(leftImg, leftGrayImage, ColorConversionCodes.BGR2GRAY);
            using Mat rightGrayImage = new Mat();
            Cv2.CvtColor(rightImg, rightGrayImage, ColorConversionCodes.BGR2GRAY);

            Mat<Point2f> corners_l = new Mat<Point2f>();
            Mat<Point2f> corners_r = new Mat<Point2f>();

            bool found_l = Cv2.FindChessboardCorners(leftImg, patternSize, corners_l, ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage);
            bool found_r = Cv2.FindChessboardCorners(rightImg, patternSize, corners_r, ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage);

            if (found_l && found_r)
            {

                Point2f[] leftSubPixCorners = Cv2.CornerSubPix(leftGrayImage, corners_l.ToArray(), new Size(11, 11), new Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Count | CriteriaTypes.Eps, 30, 0.01));
                Point2f[] rightSubPixCorners = Cv2.CornerSubPix(rightGrayImage, corners_r.ToArray(), new Size(11, 11), new Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Count | CriteriaTypes.Eps, 30, 0.01));


                for (int i = 0; i < leftSubPixCorners.Length; i++)
                {
                    Point3f point = Uv2Xyz(leftSubPixCorners[i], rightSubPixCorners[i], P1, P2);
                    points.Add(point);
                }
            }
            return points;
        }
        /// <summary>
        /// pointcloud calibration using icp
        /// </summary>
        /// <param name="pc1"></param>
        /// <param name="pc2"></param>
        /// <returns></returns>
        public static (Mat<float> Q, Mat<float> T) CalibrateTwoPointCloud(List<Point3f> pc1, List<Point3f> pc2)
        {
            var Q = new Mat<float>(4, 1);
            var T = new Mat<float>(Mat.Zeros(3, 1, MatType.CV_32FC1));

            //NativeMethods.aruco_detectCharucoDiamond
            if (pc1.Count <= 0 || pc2.Count <= 0)
            {
                // log
                return (Q, T);
            }
            //var points1 = ConvertPointToDataPoint(pc1);
            //var points2 = ConvertPointToDataPoint(pc2);
            //EuclideanTransform initialTransform = EuclideanTransform.Identity;
            //ICP icp = new ICP();
            ////icp.ReadingDataPointsFilters = new RandomSamplingDataPointsFilter(prob: 0.1f);
            ////icp.ReferenceDataPointsFilters = new SamplingSurfaceNormalDataPointsFilter(SamplingMethod.RandomSampling, ratio: 0.2f);
            ////icp.OutlierFilter = new TrimmedDistOutlierFilter(ratio: 0.5f);
            //EuclideanTransform transform = icp.Compute(points1, points2, initialTransform);
            //var r = transform.rotation;
            //var t = transform.translation;
            //Q.At<float>(0) = r.W;
            //Q.At<float>(1) = r.X;
            //Q.At<float>(2) = r.Y;
            //Q.At<float>(3) = r.Z;
            //T.At<float>(0) = t.X;
            //T.At<float>(1) = t.Y;
            //T.At<float>(2) = t.Z;

            return (Q, T);
        }

        static Point3f Uv2Xyz(Point2f uv_l, Point2f uv_r, Mat P1, Mat P2)
        {
            Mat A = new Mat(4, 3, MatType.CV_64FC1);

            A.At<double>(0, 0) = uv_l.X * P1.At<double>(2, 0) - P1.At<double>(0, 0);
            A.At<double>(0, 1) = uv_l.X * P1.At<double>(2, 1) - P1.At<double>(0, 1);
            A.At<double>(0, 2) = uv_l.X * P1.At<double>(2, 2) - P1.At<double>(0, 2);

            A.At<double>(1, 0) = uv_l.Y * P1.At<double>(2, 0) - P1.At<double>(1, 0);
            A.At<double>(1, 1) = uv_l.Y * P1.At<double>(2, 1) - P1.At<double>(1, 1);
            A.At<double>(1, 2) = uv_l.Y * P1.At<double>(2, 2) - P1.At<double>(1, 2);

            A.At<double>(2, 0) = uv_r.X * P2.At<double>(2, 0) - P2.At<double>(0, 0);
            A.At<double>(2, 1) = uv_r.X * P2.At<double>(2, 1) - P2.At<double>(0, 1);
            A.At<double>(2, 2) = uv_r.X * P2.At<double>(2, 2) - P2.At<double>(0, 2);

            A.At<double>(3, 0) = uv_r.Y * P2.At<double>(2, 0) - P2.At<double>(1, 0);
            A.At<double>(3, 1) = uv_r.Y * P2.At<double>(2, 1) - P2.At<double>(1, 1);
            A.At<double>(3, 2) = uv_r.Y * P2.At<double>(2, 2) - P2.At<double>(1, 2);

            Mat B = new Mat(4, 1, MatType.CV_64FC1);
            B.At<double>(0, 0) = P1.At<double>(0, 3) - uv_l.X * P1.At<double>(2, 3);
            B.At<double>(1, 0) = P1.At<double>(1, 3) - uv_l.Y * P1.At<double>(2, 3);
            B.At<double>(2, 0) = P2.At<double>(0, 3) - uv_r.X * P2.At<double>(2, 3);
            B.At<double>(3, 0) = P2.At<double>(1, 3) - uv_r.Y * P2.At<double>(2, 3);

            Mat XYZ = new Mat(3, 1, MatType.CV_64FC1);

            Cv2.Solve(A, B, XYZ, DecompTypes.SVD);

            Point3f world;
            world.X = (float)XYZ.At<double>(0, 0);
            world.Y = (float)XYZ.At<double>(1, 0);
            world.Z = (float)XYZ.At<double>(2, 0);

            return world;
        }
        //static DataPoints ConvertPointToDataPoint(List<Point3f> points)
        //{
        //    DataPoints dataPoints = new DataPoints();
        //    if (points.Count > 0)
        //    {
        //        dataPoints.points = points.Select(p => new DataPoint { point = new System.Numerics.Vector3 { X = p.X, Y = p.Y, Z = p.Z } }).ToArray();
        //    }
        //    return dataPoints;
        //}
    }
}
