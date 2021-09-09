#!/bin/bash

camera=$1

sleep 5

# right arm stereo param for Argus
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_left.yaml ~/RJ1400/vision/src/video_stream_opencv/camera_info/left_arm_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_right.yaml ~/RJ1400/vision/src/video_stream_opencv/camera_info/left_arm_right.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_all.xml ~/.ros/camera_info/left_arm_all.xml

# right arm stereo param for python
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm.xml ~/RJ1300_Stereo/Arrester/{$camera}.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm.xml ~/RJ1300_Stereo/Cable/{$camera}.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm.xml ~/turntable_install/chessboard/{$camera}.xml