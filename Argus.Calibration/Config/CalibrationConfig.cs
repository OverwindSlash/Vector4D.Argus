using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Argus.Calibration.Config
{
    public class CalibrationConfig
    {
        public string StereoImagesDir { get; set; }
        public string MovementFileDir { get; set; }

        public string BodyStereoName { get; set; }
        public string BodyStereoArmPositionFile { get; set; }

        public string ArresterToolLeftStereoName { get; set; }
        public string ArresterToolLeftArmPositionFile { get; set; }
        public string ArresterToolRightStereoName { get; set; }
        public string ArresterToolRightArmPositionFile { get; set; }

        public string NutInstallToolLeftStereoName { get; set; }
        public string NutInstallToolLeftArmPositionFile { get; set; }
        public string NutInstallToolRightStereoName { get; set; }
        public string NutInstallToolRightArmPositionFile { get; set; }

        public string NutDestroyToolLeftStereoName { get; set; }
        public string NutDestroyToolLeftArmPositionFile { get; set; }
        public string NutDestroyToolRightStereoName { get; set; }
        public string NutDestroyToolRightArmPositionFile { get; set; }

        public string StripWireToolLeftStereoName { get; set; }
        public string StripWireToolLeftArmPositionFile { get; set; }
        public string StripWireToolRightStereoName { get; set; }
        public string StripWireToolRightArmPositionFile { get; set; }

        public string ClampWireToolLeftStereoName { get; set; }
        public string ClampWireToolLeftArmPositionFile { get; set; }
        public string ClampWireToolRightStereoName { get; set; }
        public string ClampWireToolRightArmPositionFile { get; set; }

        public string ClawToolLeftStereoName { get; set; }
        public string ClawToolLeftArmPositionFile { get; set; }
        public string ClawToolRightStereoName { get; set; }
        public string ClawToolRightArmPositionFile { get; set; }

        public string CutWireToolLeftStereoName { get; set; }
        public string CutWireToolLeftArmPositionFile { get; set; }
        public string CutWireToolRightStereoName { get; set; }
        public string CutWireToolRightArmPositionFile { get; set; }

        // Helper
        public List<string> ArmToolsNames { get; }
        public List<string> ArmToolsPositionFiles { get; }

        // Ros master and topic
        public string RosMasterUri { get; set; }
        public string HostName { get; set; }
        public string NodeName { get; set; }
        public string LeftStereoTopic { get; set; }
        public int TopicTimeout { get; set; }
        public int XmlRpcTimeout { get; set; }

        public string CalibrationResultDir { get; set; }

        private const string jsonFilename = "calib-config.json";

        public CalibrationConfig()
        {
            ArmToolsNames = new List<string>();
            ArmToolsPositionFiles = new List<string>();
        }

        public void LoadFromJson()
        {
            try
            {
                string jsonString = File.ReadAllText(jsonFilename);
                CalibrationConfig calibConfig = JsonSerializer.Deserialize<CalibrationConfig>(jsonString);

                StereoImagesDir = calibConfig.StereoImagesDir;
                MovementFileDir = calibConfig.MovementFileDir;
                BodyStereoName = calibConfig.BodyStereoName;
                BodyStereoArmPositionFile = calibConfig.BodyStereoArmPositionFile;

                ArmToolsNames.Clear();
                ArmToolsPositionFiles.Clear();

                ArresterToolLeftStereoName = calibConfig.ArresterToolLeftStereoName;
                ArresterToolLeftArmPositionFile = calibConfig.ArresterToolLeftArmPositionFile;
                ArmToolsNames.Add(ArresterToolLeftStereoName);
                ArmToolsPositionFiles.Add(ArresterToolLeftArmPositionFile);

                ArresterToolRightStereoName = calibConfig.ArresterToolRightStereoName;
                ArresterToolRightArmPositionFile = calibConfig.ArresterToolRightArmPositionFile;
                ArmToolsNames.Add(ArresterToolRightStereoName);
                ArmToolsPositionFiles.Add(ArresterToolRightArmPositionFile);

                NutInstallToolLeftStereoName = calibConfig.NutInstallToolLeftStereoName;
                NutInstallToolLeftArmPositionFile = calibConfig.NutInstallToolLeftArmPositionFile;
                ArmToolsNames.Add(NutInstallToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolLeftArmPositionFile);

                NutInstallToolRightStereoName = calibConfig.NutInstallToolRightStereoName;
                NutInstallToolRightArmPositionFile = calibConfig.NutInstallToolRightArmPositionFile;
                ArmToolsNames.Add(NutInstallToolRightStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolRightArmPositionFile);

                NutDestroyToolLeftStereoName = calibConfig.NutDestroyToolLeftStereoName;
                NutDestroyToolLeftArmPositionFile = calibConfig.NutDestroyToolLeftArmPositionFile;
                ArmToolsNames.Add(NutDestroyToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolLeftArmPositionFile);

                NutDestroyToolRightStereoName = calibConfig.NutDestroyToolRightStereoName;
                NutDestroyToolRightArmPositionFile = calibConfig.NutDestroyToolRightArmPositionFile;
                ArmToolsNames.Add(NutDestroyToolRightStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolRightArmPositionFile);

                StripWireToolLeftStereoName = calibConfig.StripWireToolLeftStereoName;
                StripWireToolLeftArmPositionFile = calibConfig.StripWireToolLeftArmPositionFile;
                ArmToolsNames.Add(StripWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(StripWireToolLeftArmPositionFile);

                StripWireToolRightStereoName = calibConfig.StripWireToolRightStereoName;
                StripWireToolRightArmPositionFile = calibConfig.StripWireToolRightArmPositionFile;
                ArmToolsNames.Add(StripWireToolRightStereoName);
                ArmToolsPositionFiles.Add(StripWireToolRightArmPositionFile);

                ClampWireToolLeftStereoName = calibConfig.ClampWireToolLeftStereoName;
                ClampWireToolLeftArmPositionFile = calibConfig.ClampWireToolLeftArmPositionFile;
                ArmToolsNames.Add(ClampWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolLeftArmPositionFile);

                ClampWireToolRightStereoName = calibConfig.ClampWireToolRightStereoName;
                ClampWireToolRightArmPositionFile = calibConfig.ClampWireToolRightArmPositionFile;
                ArmToolsNames.Add(ClampWireToolRightStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolRightArmPositionFile);

                ClawToolLeftStereoName = calibConfig.ClawToolLeftStereoName;
                ClawToolLeftArmPositionFile = calibConfig.ClawToolLeftArmPositionFile;
                ArmToolsNames.Add(ClawToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClawToolLeftArmPositionFile);

                ClawToolRightStereoName = calibConfig.ClawToolRightStereoName;
                ClawToolRightArmPositionFile = calibConfig.ClawToolRightArmPositionFile;
                ArmToolsNames.Add(ClawToolRightStereoName);
                ArmToolsPositionFiles.Add(ClawToolRightArmPositionFile);

                CutWireToolLeftStereoName = calibConfig.CutWireToolLeftStereoName;
                CutWireToolLeftArmPositionFile = calibConfig.CutWireToolLeftArmPositionFile;
                ArmToolsNames.Add(CutWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(CutWireToolLeftArmPositionFile);

                CutWireToolRightStereoName = calibConfig.CutWireToolRightStereoName;
                CutWireToolRightArmPositionFile = calibConfig.CutWireToolRightArmPositionFile;
                ArmToolsNames.Add(CutWireToolRightStereoName);
                ArmToolsPositionFiles.Add(CutWireToolRightArmPositionFile);

                RosMasterUri = calibConfig.RosMasterUri;
                HostName = calibConfig.HostName;
                NodeName = calibConfig.NodeName;
                LeftStereoTopic = calibConfig.LeftStereoTopic;
                TopicTimeout = calibConfig.TopicTimeout;
                XmlRpcTimeout = calibConfig.XmlRpcTimeout;

                CalibrationResultDir = calibConfig.CalibrationResultDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning! Load json configuration failed. Error message: {ex.Message}");
            }
        }
    }
}