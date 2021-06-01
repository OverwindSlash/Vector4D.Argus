#!/bin/bash
cmd=$1

source /opt/ros/melodic/setup.bash
echo "invoke script on ROS master..."
echo $cmd

rostopic pub -1 /shell_exec_service std_msgs/String "$cmd"