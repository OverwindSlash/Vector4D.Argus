#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "take photo for lidar"

gnome-terminal -t "take photo" -x bash -c "cd ~/RJ1400/lidar/driver && source devel/setup.bash && roslaunch livox_ros_driver livox_lidar_msg.launch;exec bash"