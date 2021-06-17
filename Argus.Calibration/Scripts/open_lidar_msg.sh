#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "open lidar message"

gnome-terminal -t "lidar msg" -x bash -c "cd ~/RJ1400/lidar/driver && source devel/setup.bash && roslaunch livox_ros_driver livox_lidar_msg.launch;exec bash"