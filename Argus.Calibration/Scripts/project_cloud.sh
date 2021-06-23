#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "project cloud"

gnome-terminal -t "project cloud" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration projectCloud.launch;exec bash"