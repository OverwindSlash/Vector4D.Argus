#!/bin/bash

echo "correct left arm..."

source /opt/ros/melodic/setup.bash

source ~/RJ1400/robot/devel/setup.bash

roslaunch ur_calibration calibration_correction.launch robot_ip:=192.168.1.51

cp ~/.ros/robot_calibration.yaml  ~/RJ1400/robot/src/fmauch_universal_robot/ur_description/config/ur10_default.yaml -f
cp ~/.ros/robot_calibration.yaml  ~/RJ1400/calibration/src/easy_handeye/ur_description/config/ur10_default.yaml -f