#!/bin/bash

source /opt/ros/melodic/setup.bash
echo "lucid body stereo to left arm calibration start ..."

gnome-terminal -t "arm calib" -x bash -c "cd ~/RJ1400/robot/ && source devel/setup.bash && roslaunch ur_calibration calibration_correction.launch robot_ip:=192.168.1.51;exec bash"
sleep 3

gnome-terminal -t "copy arm calib info" -x bash -c "cp ~/.ros/robot_calibration.yaml  ~/RJ1400/robot/src/fmauch_universal_robot/ur_description/config/ur10_default.yaml -f;exec bash"
gnome-terminal -t "copy arm calib info" -x bash -c "cp ~/.ros/robot_calibration.yaml  ~/RJ1400/calibration/src/easy_handeye/ur_description/config/ur10_default.yaml -f;exec bash"

gnome-terminal -t "bring up" -x bash -c "cd ~/RJ1400/robot/ && source devel/setup.bash && roslaunch ur_modern_driver ur10_bringup.launch limited:=true robot_ip:=192.168.1.51;exec bash"
sleep 3

gnome-terminal -t "moveit" -x bash -c "cd ~/RJ1400/robot/ && source devel/setup.bash && roslaunch ur10_moveit_config ur10_moveit_planning_execution.launch limited:=true;exec bash"
sleep 3

gnome-terminal -t "stereo left" -x bash -c "cd ~/RJ1400/vision/ && source devel/setup.bash && roslaunch arena_camera body_left.launch;exec bash"
sleep 3

gnome-terminal -t "rectify left" -x bash -c "cd ~/RJ1400/vision/ && source devel/setup.bash  && ROS_NAMESPACE=body_left rosrun image_proc image_proc;exec bash"

gnome-terminal -t "tracking" -x bash -c "cd ~/RJ1400/vision/ && source devel/setup.bash && roslaunch aruco_ros body_left.launch;exec bash"

#gnome-terminal -t "rqt" -x bash -c "rqt_image_view;exec bash"

gnome-terminal -t "easy_hand_eye" -x bash -c "cd ~/RJ1400/calibration/ && source devel/setup.bash && roslaunch easy_handeye easy_hand_server_only.launch namespace_prefix:=/ur10_leftarm;exec bash"