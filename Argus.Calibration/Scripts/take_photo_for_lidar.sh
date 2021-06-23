#!/bin/bash

index=$1

source /opt/ros/melodic/setup.bash
echo "take photo for lidar"

rm -rf ~/RJ1400/lidar/data/photo/left
rm -rf ~/RJ1400/lidar/data/photo/right

~/RJ1400/vision/src/Cpp_Save/Cpp_Save ~/RJ1400/lidar/data/photo

cp $(ls ~/RJ1400/lidar/data/photo/left/*.png) ~/RJ1400/lidar/data/photo/$index.png