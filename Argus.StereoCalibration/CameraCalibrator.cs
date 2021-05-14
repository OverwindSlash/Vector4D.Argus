using System;
using System.Collections;
using Argus.StereoCalibration.config;
using Argus.StereoCalibration.yaml;
using OpenCvSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YamlDotNet.Serialization;

namespace Argus.StereoCalibration
{
    public class CameraCalibrator
    {
        private static ChessboardConfig _cbConfig;
        private static Size _patternSize;
        private static float _squareSize;
        private static Mat<Point3f> _objectCorners;
        
        private static Size _imageSize;

        public delegate void LogCallBack(string log);
        private static LogCallBack _logCallBack;

        static CameraCalibrator()
        {
            InitializeChessboardConfiguration();
            Create3DChessboardCorners();
        }

        private static void InitializeChessboardConfiguration()
        {
            _cbConfig = new ChessboardConfig();
            _cbConfig.LoadFromJson();
            
            _patternSize = new Size(_cbConfig.Width, _cbConfig.Height);
            _squareSize = _cbConfig.Size;
        }
        
        private static void Create3DChessboardCorners()
        {
            List<Point3f> spaceCorners = new List<Point3f>();

            for (int y = 0; y < _patternSize.Height; y++)
            {
                for (int x = 0; x < _patternSize.Width; x++)
                {
                    spaceCorners.Add(new Point3f(x * _squareSize, y * _squareSize, 0));
                }
            }

            _objectCorners = Mat.FromArray(spaceCorners);
        }
        
        private static (bool found, Mat<Point2f> imageCorners) CheckAndGetCorners(string imageFile)
        {
            bool found = false;
            Mat<Point2f> imageCorners = new Mat<Point2f>();

            using Mat image = new Mat(imageFile);

            // try find corners using sector based approach.
            //found = Cv2.FindChessboardCornersSB(image, _patternSize, imageCorners,
            //    ChessboardFlags.Exhaustive | ChessboardFlags.Accuracy);

            // if not found, try find corners using old approach.
            if (!found)
            {
                Mat grayImage = new Mat();
                Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

                found = Cv2.FindChessboardCorners(image, _patternSize, imageCorners,
                    ChessboardFlags.AdaptiveThresh | ChessboardFlags.FilterQuads);

                if (found)
                {
                    Point2f[] subPixCorners = Cv2.CornerSubPix(grayImage, imageCorners.ToArray(), new Size(5, 5), new Size(-1, -1),
                        new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.1));

                    imageCorners = Mat.FromArray(subPixCorners);
                }
            }

            return (found, imageCorners);
        }

        public static void SetLogCallback(LogCallBack callback)
        {
            _logCallBack = callback;
        }

        public static bool CheckAndDrawConCorners(string imageFile)
        {
            var result = CheckAndGetCorners(imageFile);

            using Mat image = new Mat(imageFile);
            Cv2.DrawChessboardCorners(image, _patternSize, result.imageCorners, result.found);
            
            image.SaveImage(imageFile);

            return result.found;
        }
        
        public static (Mat<double> cameraMatrix, Mat<double> distCoeffs, double rms) CalibrateCameraIntrinsic(List<string> imageFiles)
        {
            List<Mat<Point3f>>  allObjectCorners = new List<Mat<Point3f>>();
            List<Mat<Point2f>>  allImageCorners = new List<Mat<Point2f>>();            

            foreach (var imageFile in imageFiles)
            {
                var result = CheckAndGetCorners(imageFile);

                FileInfo fi = new FileInfo(imageFile);
                if (result.found)
                {
                    //Trace.WriteLine($"Found corners in file: {fi.Name}");
                    _logCallBack($"发现角点：{fi.Name}");

                    allObjectCorners.Add(_objectCorners);
                    allImageCorners.Add(result.imageCorners);
                }
                else
                {
                    //Trace.WriteLine($"WARNING! Can not find corners in file: {fi.Name}");
                    _logCallBack($"***未发现角点：{fi.Name}");
                }
            }

            Mat<double> cameraMatrix = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));
            Mat<double> distCoeffs = new Mat<double>();

            double rms = Cv2.CalibrateCamera(allObjectCorners, allImageCorners, _imageSize, cameraMatrix, distCoeffs,
                out var rotationVectors, out var translationVectors, CalibrationFlags.FixK4 | CalibrationFlags.FixK5 | CalibrationFlags.FixK6);

            return (cameraMatrix, distCoeffs, rms);
        }

        public static CalibrationResult CalibrateStereoCamera(List<string> leftImages, List<string> rightImages)
        {
            if ((leftImages.Count == 0) || (leftImages.Count != rightImages.Count))
            {
                return null;
            }
            
            _imageSize = GetImageSize(leftImages[0]);
            
            // calibrate left camera.
            var leftCameraParams = CalibrateCameraIntrinsic(leftImages);
            Mat<double> leftCameraMatrix = leftCameraParams.cameraMatrix;
            Mat<double> leftDistCoeffs = leftCameraParams.distCoeffs;
            double leftRms = leftCameraParams.rms;

            // calibrate right camera.
            var rightCameraParams = CalibrateCameraIntrinsic(rightImages);
            Mat<double> rightCameraMatrix = rightCameraParams.cameraMatrix;
            Mat<double> rightDistCoeffs = rightCameraParams.distCoeffs;
            double rightRms = rightCameraParams.rms;
            
            // calibrate stereo params.
            var stereoParams = CalibrateStereoParams(leftImages, leftCameraMatrix, leftDistCoeffs, 
                rightImages, rightCameraMatrix, rightDistCoeffs);
            Mat<double> rotation = stereoParams.R;
            Mat<double> translation = stereoParams.T;
            Mat<double> essential = stereoParams.E;
            Mat<double> fundamental = stereoParams.F;
            double stereoRms = stereoParams.rms;
            
            // rectify stereo.
            var rectifyParams = RectifyStereo(leftCameraMatrix, leftDistCoeffs, rightCameraMatrix, rightDistCoeffs, rotation, translation);
            Mat<double> r1 = rectifyParams.R1;
            Mat<double> r2 = rectifyParams.R2;
            Mat<double> p1 = rectifyParams.P1;
            Mat<double> p2 = rectifyParams.P2;
            Mat<double> q = rectifyParams.Q;
            
            // undistort stereo.
            var undistortParams = UndistortStereo(leftCameraMatrix, leftDistCoeffs, rightCameraMatrix, rightDistCoeffs, r1, r2, p1, p2);
            Mat<float> mapLeftX = undistortParams.mapLeftX;
            Mat<float> mapLeftY = undistortParams.mapLeftY;
            Mat<float> mapRightX = undistortParams.mapRightX;
            Mat<float> mapRightY = undistortParams.mapRightY;

            // Show result.
            ShowStereoCalibrationResult(leftImages, rightImages, mapLeftX, mapLeftY, mapRightX, mapRightY);

            CalibrationResult result = new CalibrationResult()
            {
                LeftImages = leftImages,
                RightImages = rightImages,
                ImageSize = _imageSize,
                ImageCount = leftImages.Count,
                LeftRms = leftRms,
                LeftCameraMatrix = leftCameraMatrix,
                LeftDistCoeffs = leftDistCoeffs,
                RightRms = rightRms,
                RightCameraMatrix = rightCameraMatrix,
                RightDistCoeffs = rightDistCoeffs,
                StereoRms = stereoRms,
                Rotation = rotation,
                Translation = translation,
                Essential = essential,
                Fundamental = fundamental,
                R1 = r1,
                R2 = r2,
                P1 = p1,
                P2 = p2,
                Q = q,
                MapLeftX = mapLeftX,
                MapLeftY = mapLeftY,
                MapRightX = mapRightX,
                MapRightY = mapRightY
            };

            return result;
        }
        
        private static Size GetImageSize(string filename)
        {
            Mat image = new Mat(filename);
            Size imageSize = image.Size();
            return imageSize;
        }
        
        private static (Mat<double> R, Mat<double> T, Mat<double> E, Mat<double> F, double rms) 
            CalibrateStereoParams(List<string> leftImages, Mat<double> leftCameraMatrix, Mat<double> leftDistCoeffs,
            List<string> rightImages, Mat<double> rightCameraMatrix, Mat<double> rightDistCoeffs)
        {
            List<Mat<Point3f>> allObjectCorners = new List<Mat<Point3f>>();
            List<Mat<Point2f>> leftImageCorners = new List<Mat<Point2f>>();
            List<Mat<Point2f>> rightImageCorners = new List<Mat<Point2f>>();

            for (int imageIndex = 0; imageIndex < leftImages.Count; imageIndex++)
            {
                string leftImageFile = leftImages[imageIndex];
                string rightImageFile = rightImages[imageIndex];
                
                var leftCornersResult = CheckAndGetCorners(leftImageFile);
                var rightCornersResult = CheckAndGetCorners(rightImageFile);

                FileInfo leftImageFi = new FileInfo(leftImageFile);
                FileInfo rightImageFi = new FileInfo(rightImageFile);

                if (leftCornersResult.found && rightCornersResult.found)
                {
                    //Trace.WriteLine($"Found corners in both left and right file: " + $"{leftImageFi.Name} | {rightImageFi.Name}");
                    _logCallBack($"左右图像中发现角点：" + $"{leftImageFi.Name} | {rightImageFi.Name}");

                    allObjectCorners.Add(_objectCorners);
                    leftImageCorners.Add(leftCornersResult.imageCorners);
                    rightImageCorners.Add(rightCornersResult.imageCorners);
                }
                else
                {
                    _logCallBack($"***未能在左右图像中发现角点：" + $"{leftImageFi.Name} | {rightImageFi.Name}");
                }
            }

            List<List<Point3f>> objectPoints = new List<List<Point3f>>();
            foreach (var mat in allObjectCorners)
            {
                objectPoints.Add(new List<Point3f>(mat.ToArray()));
            }

            List<List<Point2f>> leftImagePoints = new List<List<Point2f>>();
            foreach (var mat in leftImageCorners)
            {
                leftImagePoints.Add(new List<Point2f>(mat.ToArray()));
            }

            List<List<Point2f>> rightImagePoints = new List<List<Point2f>>();
            foreach (var mat in rightImageCorners)
            {
                rightImagePoints.Add(new List<Point2f>(mat.ToArray()));
            }
            
            Mat<double> r = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));
            Mat<double> t = new Mat<double>(Mat.Eye(3, 1, MatType.CV_64FC1));
            Mat<double> e = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));
            Mat<double> f = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));

            double rms = Cv2.StereoCalibrate(objectPoints, leftImagePoints, rightImagePoints, leftCameraMatrix.ToRectangularArray(), leftDistCoeffs.ToArray(),
                rightCameraMatrix.ToRectangularArray(), rightCameraMatrix.ToArray(), _imageSize, r, t, e, f);

            return (r, t, e, f, rms);
        }
        
        private static (Mat<double> R1, Mat<double> R2, Mat<double> P1, Mat<double> P2, Mat<double> Q) 
            RectifyStereo(Mat<double> leftCameraMatrix, Mat<double> leftDistCoeffs, Mat<double> rightCameraMatrix, Mat<double> rightDistCoeffs,
            Mat<double> r, Mat<double> t)
        {
            Mat<double> r1 = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));
            Mat<double> r2 = new Mat<double>(Mat.Eye(3, 3, MatType.CV_64FC1));
            Mat<double> p1 = new Mat<double>(Mat.Eye(3, 4, MatType.CV_64FC1));
            Mat<double> p2 = new Mat<double>(Mat.Eye(3, 4, MatType.CV_64FC1));
            Mat<double> q = new Mat<double>(Mat.Eye(4, 4, MatType.CV_64FC1));

            Cv2.StereoRectify(leftCameraMatrix, leftDistCoeffs, rightCameraMatrix, rightDistCoeffs, 
                _imageSize, r, t, r1, r2, p1, p2, q, StereoRectificationFlags.ZeroDisparity, -1, _imageSize);
            
            return (r1, r2, p1, p2, q);
        }
        
        private static (Mat<float> mapLeftX, Mat<float> mapLeftY, Mat<float> mapRightX, Mat<float> mapRightY) 
            UndistortStereo(Mat<double> leftCameraMatrix, Mat<double> leftDistCoeffs, Mat<double> rightCameraMatrix, Mat<double> rightDistCoeffs, 
                Mat<double> R1, Mat<double> R2, Mat<double> P1, Mat<double> P2)
        {
            Mat<float> mapLeftX = new Mat<float>(Mat.Eye(_imageSize.Height, _imageSize.Width, MatType.CV_32FC1));
            Mat<float> mapLeftY = new Mat<float>(Mat.Eye(_imageSize.Height, _imageSize.Width, MatType.CV_32FC1));
            Cv2.InitUndistortRectifyMap(leftCameraMatrix, leftDistCoeffs, R1, P1, _imageSize, MatType.CV_32FC1, mapLeftX, mapLeftY);

            Mat<float> mapRightX = new Mat<float>(Mat.Eye(_imageSize.Height, _imageSize.Width, MatType.CV_32FC1));
            Mat<float> mapRightY = new Mat<float>(Mat.Eye(_imageSize.Height, _imageSize.Width, MatType.CV_32FC1));
            Cv2.InitUndistortRectifyMap(rightCameraMatrix, rightDistCoeffs, R2, P2, _imageSize, MatType.CV_32FC1, mapRightX, mapRightY);

            return (mapLeftX, mapLeftY, mapRightX, mapRightY);
        }

        private static void ShowStereoCalibrationResult(List<string> leftImageFiles, List<string> rightImageFiles,
            Mat<float> mapLeftX, Mat<float> mapLeftY, Mat<float> mapRightX, Mat<float> mapRightY)
        {
            Mat undistLeftImg = new Mat();
            Mat undistRightImg = new Mat();

            int[] randomIndex = GenerateNonRepeatedRandom(3, 0, leftImageFiles.Count);

            foreach (var imageIndex in randomIndex)
            {
                using Mat leftImage = new Mat(leftImageFiles[imageIndex]);
                using Mat rightImage = new Mat(rightImageFiles[imageIndex]);

                Cv2.Remap(leftImage, undistLeftImg, mapLeftX, mapLeftY);
                Cv2.Remap(rightImage, undistRightImg, mapRightX, mapRightY);

                ShowRectifiedStereo(undistLeftImg, undistRightImg);
            }
        }

        static int[] GenerateNonRepeatedRandom(int length, int minValue, int maxValue)
        {
            Hashtable hashtable = new Hashtable();
            int seed = Guid.NewGuid().GetHashCode();

            Random random = new Random(seed);
            for (int i = 0; hashtable.Count < length; i++)
            {
                int nValue = random.Next(minValue, maxValue);
                if (!hashtable.ContainsValue(nValue) && nValue != 0)
                {
                    hashtable.Add(i, nValue);
                }
            }

            int[] array = new int[hashtable.Count];
            hashtable.Values.CopyTo(array, 0);
            return array;
        }

        private static void ShowRectifiedStereo(Mat undistLeftImg, Mat undistRightImg)
        {
            int width, height;
            double scale = 800.0 / Math.Max(_imageSize.Width, _imageSize.Height);
            width = (int)Math.Round(_imageSize.Width * scale);
            height = (int)Math.Round(_imageSize.Height * scale);
            Mat canvas = new Mat(height, width * 2, MatType.CV_8UC3);

            Mat leftCanvasPart = new Mat(canvas, new Rect(0, 0, width, height));
            Cv2.Resize(undistLeftImg, leftCanvasPart, leftCanvasPart.Size());

            Mat rightCanvasPart = new Mat(canvas, new Rect(width, 0, width, height));
            Cv2.Resize(undistRightImg, rightCanvasPart, rightCanvasPart.Size());

            for (int i = 0; i < canvas.Rows; i += 16)
                Cv2.Line(canvas, new Point(0, i), new Point(canvas.Cols, i), Scalar.Aqua);

            Window.ShowImages(canvas);
        }

        public static string GenerateYamlFile(string yamlDir, string cameraName, Size imageSize, Mat<double> cameraMatrix, Mat<double> cameraDistCoeff, Mat<double> rectMatrix, Mat<double> projMatrix)
        {
            var yamlSerializer = new SerializerBuilder()
                .WithNamingConvention(OnlyLowerCaseNamingConvention.Instance)
                .WithEventEmitter(next => new FlowStyleDoubleSequences(next))
                .Build();

            RosCameraIntrinsic cameraIntrinsic = new RosCameraIntrinsic();
            cameraIntrinsic.Camera_Name = cameraName;
            cameraIntrinsic.Image_Width = imageSize.Width;
            cameraIntrinsic.Image_Height = imageSize.Height;
            cameraIntrinsic.Camera_Matrix = new CameraMatrix(3, 3, cameraMatrix.ToArray());
            cameraIntrinsic.Distortion_Coefficients = new CameraMatrix(1, 5, cameraDistCoeff.ToArray());
            cameraIntrinsic.Rectification_Matrix = new CameraMatrix(3, 3, rectMatrix.ToArray());
            cameraIntrinsic.Projection_Matrix = new CameraMatrix(3, 4, projMatrix.ToArray());

            var yaml = yamlSerializer.Serialize(cameraIntrinsic);

            string yamlFilename = $"{cameraName}.yaml";
            if (Directory.Exists(yamlDir))
            {
                yamlFilename = Path.Combine(yamlDir, $"{cameraName}.yaml");
            }

            using (StreamWriter writer = File.CreateText(yamlFilename))
            {
                writer.WriteLine(yaml);
                return yamlFilename;
            }
        }
        
        public static string GenerateXmlFile(string xmlDir, string cameraName, Mat<double> leftCameraMatrix, Mat<double> leftDistCoeffs,
            Mat<double> rightCameraMatrix, Mat<double> rightDistCoeffs, Mat<double> R, Mat<double> T, Mat<double> E, Mat<double> F)
        {
            string xmlFilename = $"{cameraName}.xml";
            if (Directory.Exists(xmlDir))
            {
                xmlFilename = Path.Combine(xmlDir, $"{cameraName}.xml");
            }

            using (var fs = new FileStorage(xmlFilename, FileStorage.Modes.Write))
            {
                fs.Write("M1", leftCameraMatrix);
                fs.Write("D1", leftDistCoeffs);
                fs.Write("M2", rightCameraMatrix);
                fs.Write("D2", rightDistCoeffs);
                fs.Write("R", R);
                fs.Write("T", T);
                fs.Write("E", E);
                fs.Write("F", F);

                return xmlFilename;
            }
        }
    }
}