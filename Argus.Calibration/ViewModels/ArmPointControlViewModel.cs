using Argus.Calibration.Config;
using Argus.Calibration.Helper;

namespace Argus.Calibration.ViewModels
{
    public class ArmPointControlViewModel : ViewModelBase
    {
        private StereoTypes _stereoType;
        private string _stereoName;
        
        private string _sourceLink;
        private string _destLink;
        
        private RobotArms _operationArm;
        
        public void SetStereoTypes(StereoTypes stereoType)
        {
            _stereoType = stereoType;
            
            _stereoName = CalibConfig.BodyStereoName;
            if  (_stereoType != StereoTypes.BodyStereo)
            {
                _stereoName = CalibConfig.ArmToolsNames[(int)_stereoType];
            }
        }

        // public void SetLinks(string sourceLink, string destLink)
        // {
        //     _sourceLink = sourceLink;
        //     _destLink = destLink;
        // }
        
        public void SetArm(RobotArms robotArms)
        {
            _operationArm = robotArms;
        }
        
        public void MoveTurntable(int x1, int y1)
        {
            string initTurntableCmd = $"init_arm_turntable_move.sh";
            initTurntableCmd.InvokeRosMasterScript();
        }

        public void PointChessboard()
        {
            CalculateSrcDestLinks();

            // TODO
        }

        private void CalculateSrcDestLinks()
        {
            // TODO
            if (_stereoType == StereoTypes.Realsense)
            {
                _sourceLink = "realsense_link";
            }
            else if (_stereoType == StereoTypes.BodyStereo)
            {
                _sourceLink = "body_link";
            }
            else
            {
                _sourceLink = $"{CalibConfig.ArmToolsNames[(int)_stereoType]}_link";
            }

            if (_operationArm == RobotArms.LeftArm)
            {
                _destLink = "left_arm_link";
            }
            else if (_operationArm == RobotArms.RightArm)
            {
                _destLink = "right_arm_link";
            }
            else
            {
                _destLink = "left_arm_link";
            }
        }
    }
}