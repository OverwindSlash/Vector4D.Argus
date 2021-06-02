using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argus.MultiSensorCalibration
{
    public class CalibrationTool
    {
        private static SensorConfig sensorConfig;

        static CalibrationTool()
        {
            sensorConfig = new SensorConfig();
        }

        public static Point2f[] FindChessboardCorners(Mat img,bool sub=false)
        {

            Mat<Point2f> corners = new Mat<Point2f>();
            bool find = Cv2.FindChessboardCorners(img, new Size(sensorConfig.ChessboardWidth, sensorConfig.ChessboardHeight),
                corners, ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage);
            if (sub)
            {
                using Mat grayImage = new Mat();
                Cv2.CvtColor(img, grayImage, ColorConversionCodes.BGR2GRAY);
                Point2f[] subPixCorners = Cv2.CornerSubPix(grayImage, corners.ToArray(), new Size(11, 11), new Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Count | CriteriaTypes.Eps, 30, 0.01));
                return subPixCorners;
            }
            return corners.ToArray();
        }
        public static (bool found, Point2f[] points) FindChessboardCornersByFile(string file, bool sub=false)
        {
            bool found = false;
            try
            {
                using Mat img = new Mat(file);
                var corners = FindChessboardCorners(img,sub);
                return (true, corners);
            }
            catch (Exception e)
            {
                return (found, new Point2f[0]);
                
            }
            
        }
    }
}
