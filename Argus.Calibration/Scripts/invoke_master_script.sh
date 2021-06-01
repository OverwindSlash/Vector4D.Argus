#!/bin/bash

param=$1

source /opt/ros/melodic/setup.bash

rostopic pub -1 /shell_exec_service std_msgs/String "$param"