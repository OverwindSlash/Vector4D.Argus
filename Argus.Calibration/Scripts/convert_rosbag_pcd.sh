#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "convert pcd"

gnome-terminal -t "convert pcd" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration pcdTransfer.launch;exec bash"