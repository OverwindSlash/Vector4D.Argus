#!/bin/bash

dotnet ~/RJ1400/calibration/EasyStereoCalibration/UpdateUrdf/bin/Debug/net5.0/UpdateUrdf.dll -u ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -s ~/.ros/rs_calibration_result.yaml -o ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro