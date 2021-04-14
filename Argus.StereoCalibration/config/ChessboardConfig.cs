using System;
using System.IO;
using System.Text.Json;

namespace Argus.StereoCalibration.config
{
    public class ChessboardConfig
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Size { get; set; }

        private const string jsonFilename = "cb-config.json";

        public ChessboardConfig()
        {
            Width = 11;
            Height = 8;
            Size = 30.0f;
        }

        public void LoadFromJson()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFilename);
                ChessboardConfig cbConfig = JsonSerializer.Deserialize<ChessboardConfig>(jsonString);

                Width = cbConfig.Width;
                Height = cbConfig.Height;
                Size = cbConfig.Size;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning! Load json configuration failed. Error message: {ex.Message}");
            }
        }
    }
}