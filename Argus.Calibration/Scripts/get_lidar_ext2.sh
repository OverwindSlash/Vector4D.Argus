#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "get lidar ext2"

gnome-terminal -t "get lidar ext2" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration getExt2.launch;exec bash"