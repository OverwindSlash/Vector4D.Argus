using System.Collections.Generic;
using System.IO;
using Argus.StereoCalibration.config;
using OpenCvSharp;

namespace Argus.StereoCalibration
{
    public class StereoCalibrator
    {
        private static ChessboardConfig cbConfig;
        private static Size patternSize;
        private static float squareSize;
        private static Size imageSize;
        private static Mat<Point3f> objectCorners;

        static StereoCalibrator()
        {
            InitializeChessboardConfiguration();
        }
        
        private static void InitializeChessboardConfiguration()
        {
            cbConfig = new ChessboardConfig();
            cbConfig.LoadFromJson();
            patternSize = new Size(cbConfig.Width, cbConfig.Height);
            squareSize = cbConfig.Size;
        }

        public static bool CheckAndDrawCorners(string imageFile)
        {
            bool found = false;
            Mat<Point2f> imageCorners = new Mat<Point2f>();

            using Mat image = new Mat(imageFile);

            // try find corners using sector based approach.
            //found = Cv2.FindChessboardCornersSB(image, patternSize, imageCorners,
            //    ChessboardFlags.Exhaustive | ChessboardFlags.Accuracy);

            // if not found, try find corners using old approach.
            if (!found)
            {
                Mat grayImage = new Mat();
                Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

                found = Cv2.FindChessboardCorners(image, patternSize, imageCorners,
                    ChessboardFlags.AdaptiveThresh | ChessboardFlags.FilterQuads);

                Point2f[] subPixCorners = Cv2.CornerSubPix(grayImage, imageCorners.ToArray(), new Size(5, 5), new Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.1));

                imageCorners = Mat<Point2f>.FromArray(subPixCorners);
                
                Cv2.DrawChessboardCorners(image, patternSize, imageCorners, found);
                image.SaveImage(imageFile);
            }

            return found;
        }
        
        public static List<FileInfo> GetImageFilesInFolder(string path)
        {
            ISet<string> extensions = new HashSet<string> { ".jpg", ".bmp", ".png" };

            DirectoryInfo imgDir = new DirectoryInfo(path);
            FileInfo[] files = imgDir.GetFiles();

            List<FileInfo> imageFiles = new List<FileInfo>();
            foreach (FileInfo file in files)
            {
                if (extensions.Contains(file.Extension.ToLower()))
                {
                    imageFiles.Add(file);
                }
            }

            imageFiles.Sort((x, y) => x.Name.CompareTo(y.Name));

            return imageFiles;
        }
    }
}