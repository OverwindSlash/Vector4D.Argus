#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "open lidar corner file"

gnome-terminal -t "open lidar corner file" -x bash -c "gedit ~/RJ1400/lidar/camera_calib/data/corner_lidar.txt;exec bash"