using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Argus.MultiSensorCalibration
{
    public class SensorConfig
    {
        public int RealSenseWidth { get; set; }
        public int RealSenseHeight { get; set; }
        public int ChessboardWidth { get; set; }
        public int ChessboardHeight { get; set; }
        public float ChessboardSize { get; set; }
        public int StereoImageWidth { get; set; }
        public int StereoImageHeight { get; set; }
        private const string jsonFileName = "config.json";
        public SensorConfig()
        {
            RealSenseWidth = 640;
            RealSenseHeight = 480;
            ChessboardWidth = 11;
            ChessboardHeight = 9;
            ChessboardSize = 30.0f;
            StereoImageWidth = 1920;
            StereoImageHeight = 1080;
            LoadFromJson();
        }
        public void LoadFromJson()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFileName);
                SensorConfig config = JsonSerializer.Deserialize<SensorConfig>(jsonString);

                RealSenseWidth = config.RealSenseWidth;
                RealSenseHeight = config.RealSenseHeight;
                ChessboardWidth = config.ChessboardWidth;
                ChessboardHeight = config.ChessboardHeight;
                ChessboardSize = config.ChessboardSize;
                StereoImageWidth = config.StereoImageWidth;
                StereoImageHeight = config.StereoImageHeight;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning! Load json configuration failed. Error message: {ex.Message}");
            }
        }

    }
}
