using System;
using System.IO;
using System.Text.Json;

namespace Argus.Calibration.Config
{
    public class CalibrationConfig
    {
        public string StereoImagesDir { get; set; }
        public string BodyCameraName { get; set; }
        public string ArmPositionFile { get; set; }
        public string YamlFileDir { get; set; }
        public string XmlFileDir { get; set; }

        private const string jsonFilename = "calib-config.json";
        
        public void LoadFromJson()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFilename);
                CalibrationConfig calibConfig = JsonSerializer.Deserialize<CalibrationConfig>(jsonString);

                StereoImagesDir = calibConfig.StereoImagesDir;
                BodyCameraName = calibConfig.BodyCameraName;
                ArmPositionFile = calibConfig.ArmPositionFile;
                YamlFileDir = calibConfig.YamlFileDir;
                XmlFileDir = calibConfig.XmlFileDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning! Load json configuration failed. Error message: {ex.Message}");
            }
        }
    }
}