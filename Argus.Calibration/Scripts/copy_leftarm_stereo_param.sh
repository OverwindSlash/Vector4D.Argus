#!/bin/bash

sleep 5

# right arm stereo param for Argus
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_left.yaml ~/RJ1400/vision/src/video_stream_opencv/camera_info/left_arm_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_right.yaml ~/RJ1400/vision/src/video_stream_opencv/camera_info/left_arm_right.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm_all.xml ~/.ros/camera_info/left_arm_all.xml

# right arm stereo param for python
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm.xml ~/RJ1300_Stereo/Arrester/left_arm.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/left_arm.xml ~/RJ1300_Stereo/Cable/left_arm.xml
