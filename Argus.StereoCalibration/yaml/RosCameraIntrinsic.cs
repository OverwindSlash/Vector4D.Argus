namespace Argus.StereoCalibration.yaml
{
    public class RosCameraIntrinsic
    {
        
        public int Image_Width { get; set; }
        public int Image_Height { get; set; }
        public string Camera_Name { get; set; }
        public CameraMatrix Camera_Matrix { get; set; }
        public string Distortion_Model { get; set; }
        public CameraMatrix Distortion_Coefficients { get; set; }
        public CameraMatrix Rectification_Matrix { get; set; }
        public CameraMatrix Projection_Matrix { get; set; }

        public RosCameraIntrinsic()
        {
            Image_Width = 1920;
            Image_Height = 1080;
            Camera_Name = "body_left";

            Camera_Matrix = new CameraMatrix(3, 3, new double[9]);

            Distortion_Model = "plumb_bob";
            Distortion_Coefficients = new CameraMatrix(1, 5, new double[5]);

            Rectification_Matrix = new CameraMatrix(3, 3, new double[9]);

            Projection_Matrix = new CameraMatrix(3, 4, new double[12]);
        }
    }
}
