#!/bin/bash

robot=$1

mkdir -p ~/jhd/v3/install/share/scene/data/parameters/$robot
dotnet ~/RJ1400/calibration/EasyStereoCalibration/UpdateUrdf/bin/Debug/net5.0/UpdateUrdf.dll -u ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -t ~/.ros/turntable_0_0_calibration.yaml -o ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -b $robot -x ~/jhd/v3/install/share/scene/data/parameters/$robot/turnable/turnable.txt