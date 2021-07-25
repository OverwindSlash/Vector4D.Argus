#!/bin/bash

name=$1

dotnet ~/RJ1400/calibration/EasyStereoCalibration/UpdateUrdf/bin/Debug/net5.0/UpdateUrdf.dll -u ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -n "$name" -f ~/.ros/easy_handeye/ur10_leftarm_eye_on_hand.yaml -o ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro