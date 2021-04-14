namespace Argus.StereoCalibration.yaml
{
    public class CameraMatrix
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public double[] Data { get; set; }

        public CameraMatrix(int rows, int cols, double[] data)
        {
            Rows = rows;
            Cols = cols;
            Data = data;
        }
    }
}
