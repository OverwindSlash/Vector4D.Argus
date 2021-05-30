#!/bin/bash

ns=$1
source /opt/ros/melodic/setup.bash

echo $ns
source ~/RJ1400/calibration/devel/setup.bash
roslaunch easy_handeye auto_handeye.launch namespace:=$ns

exit 1