#!/bin/bash

robot=$1

# body stereo 2 realsense
mkdir -p ~/jhd/v3/install/share/scene/data/parameters/$1
cp -f ~/.ros/rs_calibration_result.txt ~/jhd/v3/install/share/scene/data/parameters/$1/realsense_to_stereo_vision.txt
