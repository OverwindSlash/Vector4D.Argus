#!/bin/bash

sleep 5

# body stereo param for Argus
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_left.yaml ~/.ros/camera_info/body_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_right.yaml ~/.ros/camera_info/body_right.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_all.xml ~/.ros/camera_info/body_all.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_left.yaml ~/RJ1400/vision/src/arena_camera/camera_info/body_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_right.yaml ~/RJ1400/vision/src/arena_camera/camera_info/body_right.yaml

# body stereo param for python
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body.xml ~/RJ1300_Stereo/Arrester/camera_Link.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body.xml ~/RJ1300_Stereo/Cable/camera_Link.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body.xml ~/turntable_install/camera_Link.xml
