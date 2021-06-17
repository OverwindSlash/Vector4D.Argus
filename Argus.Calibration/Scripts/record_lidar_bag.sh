#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "record lidar message"

gnome-terminal -t "record lidar" -x bash -c "rosbag record --duration=10 /livox/lidar -O ~/RJ1400/lidar/data/lidar/$index.bag;exec bash"