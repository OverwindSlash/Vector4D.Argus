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
        public string ArresterToolRightStereoIp { get; set; }
        public string ArresterToolRightStereoName { get; set; }
        public string ArresterToolRightArmPositionFile { get; set; }

        public string NutInstallToolLeftStereoIp { get; set; }
        public string NutInstallToolLeftStereoName { get; set; }
        public string NutInstallToolLeftArmPositionFile { get; set; }
        public string NutInstallToolRightStereoIp { get; set; }
        public string NutInstallToolRightStereoName { get; set; }
        public string NutInstallToolRightArmPositionFile { get; set; }

        public string NutDestroyToolLeftStereoIp { get; set; }
        public string NutDestroyToolLeftStereoName { get; set; }
        public string NutDestroyToolLeftArmPositionFile { get; set; }
        public string NutDestroyToolRightStereoIp { get; set; }
        public string NutDestroyToolRightStereoName { get; set; }
        public string NutDestroyToolRightArmPositionFile { get; set; }

        public string StripWireToolLeftStereoIp { get; set; }
        public string StripWireToolLeftStereoName { get; set; }
        public string StripWireToolLeftArmPositionFile { get; set; }
        public string StripWireToolRightStereoIp { get; set; }
        public string StripWireToolRightStereoName { get; set; }
        public string StripWireToolRightArmPositionFile { get; set; }

        public string ClampWireToolLeftStereoIp { get; set; }
        public string ClampWireToolLeftStereoName { get; set; }
        public string ClampWireToolLeftArmPositionFile { get; set; }
        public string ClampWireToolRightStereoIp { get; set; }
        public string ClampWireToolRightStereoName { get; set; }
        public string ClampWireToolRightArmPositionFile { get; set; }

        public string ClawToolLeftStereoIp { get; set; }
        public string ClawToolLeftStereoName { get; set; }
        public string ClawToolLeftArmPositionFile { get; set; }
        public string ClawToolRightStereoIp { get; set; }
        public string ClawToolRightStereoName { get; set; }
        public string ClawToolRightArmPositionFile { get; set; }


        public string CutWireToolLeftStereoIp { get; set; }
        public string CutWireToolLeftStereoName { get; set; }
        public string CutWireToolLeftArmPositionFile { get; set; }
        public string CutWireToolRightStereoIp { get; set; }
        public string CutWireToolRightStereoName { get; set; }
        public string CutWireToolRightArmPositionFile { get; set; }

        // Helper
        public List<string> ArmToolsIps { get; }
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
            ArmToolsIps = new List<string>();
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

                ArmToolsIps.Clear();
                ArmToolsNames.Clear();
                ArmToolsPositionFiles.Clear();

                ArresterToolLeftStereoIp = calibConfig.ArresterToolLeftStereoIp;
                ArresterToolLeftStereoName = calibConfig.ArresterToolLeftStereoName;
                ArresterToolLeftArmPositionFile = calibConfig.ArresterToolLeftArmPositionFile;
                ArmToolsIps.Add(ArresterToolLeftStereoIp);
                ArmToolsNames.Add(ArresterToolLeftStereoName);
                ArmToolsPositionFiles.Add(ArresterToolLeftArmPositionFile);

                ArresterToolRightStereoIp = calibConfig.ArresterToolRightStereoIp;
                ArresterToolRightStereoName = calibConfig.ArresterToolRightStereoName;
                ArresterToolRightArmPositionFile = calibConfig.ArresterToolRightArmPositionFile;
                ArmToolsIps.Add(ArresterToolRightStereoIp);
                ArmToolsNames.Add(ArresterToolRightStereoName);
                ArmToolsPositionFiles.Add(ArresterToolRightArmPositionFile);

                NutInstallToolLeftStereoIp = calibConfig.NutInstallToolLeftStereoIp;
                NutInstallToolLeftStereoName = calibConfig.NutInstallToolLeftStereoName;
                NutInstallToolLeftArmPositionFile = calibConfig.NutInstallToolLeftArmPositionFile;
                ArmToolsIps.Add(NutInstallToolLeftStereoIp);
                ArmToolsNames.Add(NutInstallToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolLeftArmPositionFile);

                NutInstallToolRightStereoIp = calibConfig.NutInstallToolRightStereoIp;
                NutInstallToolRightStereoName = calibConfig.NutInstallToolRightStereoName;
                NutInstallToolRightArmPositionFile = calibConfig.NutInstallToolRightArmPositionFile;
                ArmToolsIps.Add(NutInstallToolRightStereoIp);
                ArmToolsNames.Add(NutInstallToolRightStereoName);
                ArmToolsPositionFiles.Add(NutInstallToolRightArmPositionFile);

                NutDestroyToolLeftStereoIp = calibConfig.NutDestroyToolLeftStereoIp;
                NutDestroyToolLeftStereoName = calibConfig.NutDestroyToolLeftStereoName;
                NutDestroyToolLeftArmPositionFile = calibConfig.NutDestroyToolLeftArmPositionFile;
                ArmToolsIps.Add(NutDestroyToolLeftStereoIp);
                ArmToolsNames.Add(NutDestroyToolLeftStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolLeftArmPositionFile);

                NutDestroyToolRightStereoIp = calibConfig.NutDestroyToolRightStereoIp;
                NutDestroyToolRightStereoName = calibConfig.NutDestroyToolRightStereoName;
                NutDestroyToolRightArmPositionFile = calibConfig.NutDestroyToolRightArmPositionFile;
                ArmToolsIps.Add(NutDestroyToolRightStereoIp);
                ArmToolsNames.Add(NutDestroyToolRightStereoName);
                ArmToolsPositionFiles.Add(NutDestroyToolRightArmPositionFile);

                StripWireToolLeftStereoIp = calibConfig.StripWireToolLeftStereoIp;
                StripWireToolLeftStereoName = calibConfig.StripWireToolLeftStereoName;
                StripWireToolLeftArmPositionFile = calibConfig.StripWireToolLeftArmPositionFile;
                ArmToolsIps.Add(StripWireToolLeftStereoIp);
                ArmToolsNames.Add(StripWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(StripWireToolLeftArmPositionFile);

                StripWireToolRightStereoIp = calibConfig.StripWireToolRightStereoIp;
                StripWireToolRightStereoName = calibConfig.StripWireToolRightStereoName;
                StripWireToolRightArmPositionFile = calibConfig.StripWireToolRightArmPositionFile;
                ArmToolsIps.Add(StripWireToolRightStereoIp);
                ArmToolsNames.Add(StripWireToolRightStereoName);
                ArmToolsPositionFiles.Add(StripWireToolRightArmPositionFile);

                ClampWireToolLeftStereoIp = calibConfig.ClampWireToolLeftStereoIp;
                ClampWireToolLeftStereoName = calibConfig.ClampWireToolLeftStereoName;
                ClampWireToolLeftArmPositionFile = calibConfig.ClampWireToolLeftArmPositionFile;
                ArmToolsIps.Add(ClampWireToolLeftStereoIp);
                ArmToolsNames.Add(ClampWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolLeftArmPositionFile);

                ClampWireToolRightStereoIp = calibConfig.ClampWireToolRightStereoIp;
                ClampWireToolRightStereoName = calibConfig.ClampWireToolRightStereoName;
                ClampWireToolRightArmPositionFile = calibConfig.ClampWireToolRightArmPositionFile;
                ArmToolsIps.Add(ClampWireToolRightStereoIp);
                ArmToolsNames.Add(ClampWireToolRightStereoName);
                ArmToolsPositionFiles.Add(ClampWireToolRightArmPositionFile);

                ClawToolLeftStereoIp = calibConfig.ClawToolLeftStereoIp;
                ClawToolLeftStereoName = calibConfig.ClawToolLeftStereoName;
                ClawToolLeftArmPositionFile = calibConfig.ClawToolLeftArmPositionFile;
                ArmToolsIps.Add(ClawToolLeftStereoIp);
                ArmToolsNames.Add(ClawToolLeftStereoName);
                ArmToolsPositionFiles.Add(ClawToolLeftArmPositionFile);

                ClawToolRightStereoIp = calibConfig.ClawToolRightStereoIp;
                ClawToolRightStereoName = calibConfig.ClawToolRightStereoName;
                ClawToolRightArmPositionFile = calibConfig.ClawToolRightArmPositionFile;
                ArmToolsIps.Add(ClawToolRightStereoIp);
                ArmToolsNames.Add(ClawToolRightStereoName);
                ArmToolsPositionFiles.Add(ClawToolRightArmPositionFile);

                CutWireToolLeftStereoIp = calibConfig.CutWireToolLeftStereoIp;
                CutWireToolLeftStereoName = calibConfig.CutWireToolLeftStereoName;
                CutWireToolLeftArmPositionFile = calibConfig.CutWireToolLeftArmPositionFile;
                ArmToolsIps.Add(CutWireToolLeftStereoIp);
                ArmToolsNames.Add(CutWireToolLeftStereoName);
                ArmToolsPositionFiles.Add(CutWireToolLeftArmPositionFile);

                CutWireToolRightStereoIp = calibConfig.CutWireToolRightStereoIp;
                CutWireToolRightStereoName = calibConfig.CutWireToolRightStereoName;
                CutWireToolRightArmPositionFile = calibConfig.CutWireToolRightArmPositionFile;
                ArmToolsIps.Add(CutWireToolRightStereoIp);
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