#!/bin/bash

# right arm stereo param for Argus
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/right_arm_left.yaml ～/RJ1400/vision/src/video_stream_opencv/camera_info/right_arm_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/right_arm_right.yaml ～/RJ1400/vision/src/video_stream_opencv/camera_info/right_arm_right.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/right_arm_all.xml ~/.ros/camera_info/right_arm_all.xml

# right arm stereo param for python
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/right_arm.xml ~/RJ1300-Stereo/Arrester/right_arm.xml