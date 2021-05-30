#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "open lucid body stereo ..."

source ~/RJ1400/vision/devel/setup.bash && 
roslaunch arena_camera body_2cameras.launch
