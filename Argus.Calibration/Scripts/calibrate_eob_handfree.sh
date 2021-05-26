#!/bin/bash

ns=$1
source /opt/ros/melodic/setup.bash
echo "Warning! please make sure easy_handeye is started!!!"
echo "auto calibration start ..."
sleep 3

echo $ns
gnome-terminal -t "handfree calib" -x bash -c "cd ~/RJ1400/calibration/ && source devel/setup.bash && roslaunch easy_handeye auto_handeye.launch namespace:=$ns;exec bash"