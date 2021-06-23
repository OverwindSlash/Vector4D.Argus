#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "corner photo"

gnome-terminal -t "corner photo" -x bash -c "cd ~/RJ1400/lidar/camera_calib && source devel/setup.bash && roslaunch camera_lidar_calibration cornerPhoto.launch image_path:=$HOME/RJ1400/lidar/data/photo/$index.png;exec bash"