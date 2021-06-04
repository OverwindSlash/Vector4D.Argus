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

        public string ArresterToolLeftStereoIp { get; set; }
        public string ArresterToolLeftStereoName { get; set; }
        public string ArresterToolLeftArmPositionFile { get; set; }
        public string ArresterToolLeftArmHandEyePresetFile { get; set; }
        public string ArresterToolRightStereoIp { get; set; }
        public string ArresterToolRightStereoName { get; set; }
        public string ArresterToolRightArmPositionFile { get; set; }
        public string ArresterToolRightArmHandEyePresetFile { get; set; }

        public string NutInstallToolLeftStereoIp { get; set; }
        public string NutInstallToolLeftStereoName { get; set; }
        public string NutInstallToolLeftArmPositionFile { get; set; }
        public string NutInstallToolLeftArmHandEyePresetFile { get; set; }
        public string NutInstallToolRightStereoIp { get; set; }
        public string NutInstallToolRightStereoName { get; set; }
        public string NutInstallToolRightArmPositionFile { get; set; }
        public string NutInstallToolRightArmHandEyePresetFile { get; set; }

        public string NutDestroyToolLeftStereoIp { get; set; }
        public string NutDestroyToolLeftStereoName { get; set; }
        public string NutDestroyToolLeftArmPositionFile { get; set; }
        public string NutDestroyToolLeftArmHandEyePresetFile { get; set; }
        public string NutDestroyToolRightStereoIp { get; set; }
        public string NutDestroyToolRightStereoName { get; set; }
        public string NutDestroyToolRightArmPositionFile { get; set; }
        public string NutDestroyToolRightArmHandEyePresetFile { get; set; }

        public string StripWireToolLeftStereoIp { get; set; }
        public string StripWireToolLeftStereoName { get; set; }
        public string StripWireToolLeftArmPositionFile { get; set; }
        public string StripWireToolLeftArmHandEyePresetFile { get; set; }
        public string StripWireToolRightStereoIp { get; set; }
        public string StripWireToolRightStereoName { get; set; }
        public string StripWireToolRightArmPositionFile { get; set; }
        public string StripWireToolRightArmHandEyePresetFile { get; set; }
        

        public string ClampWireToolLeftStereoIp { get; set; }
        public string ClampWireToolLeftStereoName { get; set; }
        public string ClampWireToolLeftArmPositionFile { get; set; }
        public string ClampWireToolLeftArmHandEyePresetFile { get; set; }
        public string ClampWireToolRightStereoIp { get; set; }
        public string ClampWireToolRightStereoName { get; set; }
        public string ClampWireToolRightArmPositionFile { get; set; }
        public string ClampWireToolRightArmHandEyePresetFile { get; set; }

        public string ClawToolLeftStereoIp { get; set; }
        public string ClawToolLeftStereoName { get; set; }
        public string ClawToolLeftArmPositionFile { get; set; }
        public string ClawToolLeftArmHandEyePresetFile { get; set; }
        public string ClawToolRightStereoIp { get; set; }
        public string ClawToolRightStereoName { get; set; }
        public string ClawToolRightArmPositionFile { get; set; }
        public string ClawToolRightArmHandEyePresetFile { get; set; }


        public string CutWireToolLeftStereoIp { get; set; }
        public string CutWireToolLeftStereoName { get; set; }
        public string CutWireToolLeftArmPositionFile { get; set; }
        public string CutWireToolLeftArmHandEyePresetFile { get; set; }
        public string CutWireToolRightStereoIp { get; set; }
        public string CutWireToolRightStereoName { get; set; }
        public string CutWireToolRightArmPositionFile { get; set; }
        public string CutWireToolRightArmHandEyePresetFile { get; set; }

        // Helper
        public List<string> ArmToolsIps { get; }
        public List<string> ArmToolsNames { get; }
        public List<string> ArmToolsPositionFiles { get; }
        public List<string> ArmToolsPresetFiles { get; }

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
            ArmToolsIps = new List<string>();
            ArmToolsNames = new List<string>();
            ArmToolsPositionFiles = new List<string>();
            ArmToolsPresetFiles = new List<string>();
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

                ArmToolsIps.Clear();
                ArmToolsNames.Clear();
                ArmToolsPositionFiles.Clear();
                ArmToolsPresetFiles.Clear();

                ArresterToolLeftStereoIp = calibConfig.ArresterToolLeftStereoIp;
                ArresterToolLeftStereoName = calibConfig.ArresterToolLeftStereoName;
                ArresterToolLeftArmPositionFile = calibConfig.ArresterToolLeftArmPositionFile;
                ArresterToolLeftArmHandEyePresetFile = calibConfig.ArresterToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(ArresterToolLeftStereoIp);
                ArmToolsNames.Add(ArresterToolLeftStereoName);
                ArmToolsPositionFiles.Add(ArresterToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(ArresterToolLeftArmHandEyePresetFile);

                ArresterToolRightStereoIp = calibConfig.ArresterToolRightStereoIp;
                ArresterToolRightStereoName = calibConfig.ArresterToolRightStereoName;
                ArresterToolRightArmPositionFile = calibConfig.ArresterToolRightArmPositionFile;
                ArresterToolRightArmHandEyePresetFile = calibConfig.ArresterToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(ArresterToolRightStereoIp);
                ArmToolsNames.Add(ArresterToolRightStereoName);
                ArmToolsPositionFiles.Add(ArresterToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(ArresterToolRightArmHandEyePresetFile);

                NutInstallToolLeftStereoIp = calibConfig.NutInstallToolLeftStereoIp;
                NutInstallToolLeftStereoName = calibConfig.NutInstallToolLeftStereoName;
                NutInstallToolLeftArmPositionFile = calibConfig.NutInstallToolLeftArmPositionFile;
                NutInstallToolLeftArmHandEyePresetFile = calibConfig.NutInstallToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(NutInstallToolLeftStereoIp);
                ArmToolsNames.Add(NutInstallToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(NutInstallToolLeftArmHandEyePresetFile);

                NutInstallToolRightStereoIp = calibConfig.NutInstallToolRightStereoIp;
                NutInstallToolRightStereoName = calibConfig.NutInstallToolRightStereoName;
                NutInstallToolRightArmPositionFile = calibConfig.NutInstallToolRightArmPositionFile;
                NutInstallToolRightArmHandEyePresetFile = calibConfig.NutInstallToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(NutInstallToolRightStereoIp);
                ArmToolsNames.Add(NutInstallToolRightStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(NutInstallToolRightArmHandEyePresetFile);

                NutDestroyToolLeftStereoIp = calibConfig.NutDestroyToolLeftStereoIp;
                NutDestroyToolLeftStereoName = calibConfig.NutDestroyToolLeftStereoName;
                NutDestroyToolLeftArmPositionFile = calibConfig.NutDestroyToolLeftArmPositionFile;
                NutDestroyToolLeftArmHandEyePresetFile = calibConfig.NutDestroyToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(NutDestroyToolLeftStereoIp);
                ArmToolsNames.Add(NutDestroyToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(NutDestroyToolLeftArmHandEyePresetFile);

                NutDestroyToolRightStereoIp = calibConfig.NutDestroyToolRightStereoIp;
                NutDestroyToolRightStereoName = calibConfig.NutDestroyToolRightStereoName;
                NutDestroyToolRightArmPositionFile = calibConfig.NutDestroyToolRightArmPositionFile;
                NutDestroyToolRightArmHandEyePresetFile = calibConfig.NutDestroyToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(NutDestroyToolRightStereoIp);
                ArmToolsNames.Add(NutDestroyToolRightStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(NutDestroyToolRightArmHandEyePresetFile);

                StripWireToolLeftStereoIp = calibConfig.StripWireToolLeftStereoIp;
                StripWireToolLeftStereoName = calibConfig.StripWireToolLeftStereoName;
                StripWireToolLeftArmPositionFile = calibConfig.StripWireToolLeftArmPositionFile;
                StripWireToolLeftArmHandEyePresetFile = calibConfig.StripWireToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(StripWireToolLeftStereoIp);
                ArmToolsNames.Add(StripWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(StripWireToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(StripWireToolLeftArmHandEyePresetFile);

                StripWireToolRightStereoIp = calibConfig.StripWireToolRightStereoIp;
                StripWireToolRightStereoName = calibConfig.StripWireToolRightStereoName;
                StripWireToolRightArmPositionFile = calibConfig.StripWireToolRightArmPositionFile;
                StripWireToolRightArmHandEyePresetFile = calibConfig.StripWireToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(StripWireToolRightStereoIp);
                ArmToolsNames.Add(StripWireToolRightStereoName);
                ArmToolsPositionFiles.Add(StripWireToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(StripWireToolRightArmHandEyePresetFile);

                ClampWireToolLeftStereoIp = calibConfig.ClampWireToolLeftStereoIp;
                ClampWireToolLeftStereoName = calibConfig.ClampWireToolLeftStereoName;
                ClampWireToolLeftArmPositionFile = calibConfig.ClampWireToolLeftArmPositionFile;
                ClampWireToolLeftArmHandEyePresetFile = calibConfig.ClampWireToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(ClampWireToolLeftStereoIp);
                ArmToolsNames.Add(ClampWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(ClampWireToolLeftArmHandEyePresetFile);

                ClampWireToolRightStereoIp = calibConfig.ClampWireToolRightStereoIp;
                ClampWireToolRightStereoName = calibConfig.ClampWireToolRightStereoName;
                ClampWireToolRightArmPositionFile = calibConfig.ClampWireToolRightArmPositionFile;
                ClampWireToolRightArmHandEyePresetFile = calibConfig.ClampWireToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(ClampWireToolRightStereoIp);
                ArmToolsNames.Add(ClampWireToolRightStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(ClampWireToolRightArmHandEyePresetFile);

                ClawToolLeftStereoIp = calibConfig.ClawToolLeftStereoIp;
                ClawToolLeftStereoName = calibConfig.ClawToolLeftStereoName;
                ClawToolLeftArmPositionFile = calibConfig.ClawToolLeftArmPositionFile;
                ClawToolLeftArmHandEyePresetFile = calibConfig.ClawToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(ClawToolLeftStereoIp);
                ArmToolsNames.Add(ClawToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClawToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(ClawToolLeftArmHandEyePresetFile);

                ClawToolRightStereoIp = calibConfig.ClawToolRightStereoIp;
                ClawToolRightStereoName = calibConfig.ClawToolRightStereoName;
                ClawToolRightArmPositionFile = calibConfig.ClawToolRightArmPositionFile;
                ClawToolRightArmHandEyePresetFile = calibConfig.ClawToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(ClawToolRightStereoIp);
                ArmToolsNames.Add(ClawToolRightStereoName);
                ArmToolsPositionFiles.Add(ClawToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(ClawToolRightArmHandEyePresetFile);

                CutWireToolLeftStereoIp = calibConfig.CutWireToolLeftStereoIp;
                CutWireToolLeftStereoName = calibConfig.CutWireToolLeftStereoName;
                CutWireToolLeftArmPositionFile = calibConfig.CutWireToolLeftArmPositionFile;
                CutWireToolLeftArmHandEyePresetFile = calibConfig.CutWireToolLeftArmHandEyePresetFile;
                ArmToolsIps.Add(CutWireToolLeftStereoIp);
                ArmToolsNames.Add(CutWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(CutWireToolLeftArmPositionFile);
                ArmToolsPresetFiles.Add(CutWireToolLeftArmHandEyePresetFile);

                CutWireToolRightStereoIp = calibConfig.CutWireToolRightStereoIp;
                CutWireToolRightStereoName = calibConfig.CutWireToolRightStereoName;
                CutWireToolRightArmPositionFile = calibConfig.CutWireToolRightArmPositionFile;
                CutWireToolRightArmHandEyePresetFile = calibConfig.CutWireToolRightArmHandEyePresetFile;
                ArmToolsIps.Add(CutWireToolRightStereoIp);
                ArmToolsNames.Add(CutWireToolRightStereoName);
                ArmToolsPositionFiles.Add(CutWireToolRightArmPositionFile);
                ArmToolsPresetFiles.Add(CutWireToolRightArmHandEyePresetFile);

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