#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "take photo for lidar"

rm -rf /home/rj1300-4/RJ1400/lidar/data/photo/left
rm -rf /home/rj1300-4/RJ1400/lidar/data/photo/right

~/RJ1400/vision/src/Cpp_Save/Cpp_Save /home/rj1300-4/RJ1400/lidar/data/photo

cp $(ls /home/rj1300-4/RJ1400/lidar/data/photo/left/*.png) /home/rj1300-4/RJ1400/lidar/data/photo/$index.png