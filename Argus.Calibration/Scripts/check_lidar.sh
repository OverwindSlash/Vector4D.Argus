#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "check lidar"

gnome-terminal -t "lidar" -x bash -c "cd ~/RJ1400/lidar/driver && source devel/setup.bash && roslaunch livox_ros_driver livox_lidar_rviz.launch;exec bash"