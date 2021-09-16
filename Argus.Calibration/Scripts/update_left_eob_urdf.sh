#!/bin/bash

robot=$1
preset=$2

if test $[preset] -eq 1
then
    pos=a
else
    pos=b
fi

dotnet ~/RJ1400/calibration/EasyStereoCalibration/UpdateUrdf/bin/Debug/net5.0/UpdateUrdf.dll -u ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -l ~/.ros/easy_handeye/ur10_leftarm_eye_on_base.yaml -o ~/maincontroller/install/share/ur_description/urdf/handeye_calibration_joint.xacro -b $robot -x ~/jhd/v3/install/share/scene/data/parameters/$robot/turnable/{$pos}_rs_left.txt