#!/bin/bash

# body stereo param for Argus
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_left.yaml ~/.ros/camera_info/body_left.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_right.yaml ~/.ros/camera_info/body_right.yaml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body_all.xml ~/.ros/camera_info/body_all.xml

# body stereo param for python
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body.xml ~/RJ1300_Stereo/Arrester/body.xml
cp -f ~/Vector4D.Argus/Argus.Calibration/bin/Debug/net5.0/CalibrationResults/body.xml ~/RJ1300_Stereo/Cable/body.xml
