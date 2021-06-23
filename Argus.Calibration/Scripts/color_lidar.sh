#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "color lidar"

gnome-terminal -t "color lidar" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration colorLidar.launch;exec bash"