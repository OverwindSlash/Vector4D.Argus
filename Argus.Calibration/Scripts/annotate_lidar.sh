#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "anno lidar"

gnome-terminal -t "anno lidar" -x bash -c "pcl_viewer -use_point_picking ~/RJ1400/lidar/camera_calib/data/pcdFiles/$index.pcd;exec bash"