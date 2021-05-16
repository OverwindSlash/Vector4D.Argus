#!/bin/bash
pos=$1

source /opt/ros/melodic/setup.bash
echo "move left arm to position ..."

OLD_IFS=$IFS
IFS=" "
array=($1)
IFS=$OLD_IFS
echo "x :" ${array[0]}
echo "y :" ${array[1]}
echo "z :" ${array[2]}
echo "q2u_rpy_r:" ${array[3]}
echo "q2u_rpy_p:" ${array[4]}
echo "q2u_rpy_y:" ${array[5]}

rostopic pub -1 /leftarm_radar_chatter armpoint_msg/armpoint "header:
  seq: 0
  stamp:
    secs: 0
    nsecs: 0
  frame_id: ''
x: ${array[0]}
y: ${array[1]}
z: ${array[2]}
ox: 0
oy: 0
oz: 0
ow: 0
q2u_rpy_r: ${array[3]}
q2u_rpy_p: ${array[4]}
q2u_rpy_y: ${array[5]}"

sleep 2