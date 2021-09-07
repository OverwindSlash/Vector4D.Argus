#!/bin/bash

robot=$1

# body stereo 2 realsense
mkdir -p ~/jhd/v3/install/share/scene/data/parameters/$1
cp -f ~/.ros/turntable_0_0_calibration.txt ~/jhd/v3/install/share/scene/data/parameters/$1/calibration_data.txt
