using System.Collections.Generic;
using OpenCvSharp;

namespace Argus.StereoCalibration
{
    public class CalibrationResult
    {
        public List<string> LeftImages { get; set; }
        public List<string> RightImages { get; set; }
        
        public Size ImageSize { get; set; }
        public int ImageCount { get; set; }

        public double LeftRms { get; set; }
        public Mat<double> LeftCameraMatrix { get; set; }
        public Mat<double> LeftDistCoeffs { get; set; }
        
        public double RightRms { get; set; }
        public Mat<double> RightCameraMatrix { get; set; }
        public Mat<double> RightDistCoeffs { get; set; }
        
        
        public double StereoRms { get; set; }
        public Mat<double> Rotation { get; set; }
        public Mat<double> Translation { get; set; }
        public Mat<double> Essential { get; set; }
        public Mat<double> Fundamental { get; set; }

        public Mat<double> R1 { get; set; }
        public Mat<double> R2 { get; set; }
        public Mat<double> P1 { get; set; }
        public Mat<double> P2 { get; set; }
        public Mat<double> Q { get; set; }
        
        public Mat<float> MapLeftX { get; set; }
        public Mat<float> MapLeftY { get; set; }
        
        public Mat<float> MapRightX { get; set; }
        public Mat<float> MapRightY { get; set; }
    }
}