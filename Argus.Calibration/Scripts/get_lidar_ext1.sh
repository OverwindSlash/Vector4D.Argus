#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "get lidar ext1"

gnome-terminal -t "get lidar ext1" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration getExt1.launch;exec bash"