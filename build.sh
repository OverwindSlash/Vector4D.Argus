#!/bin/bash

dotnet build Argus.Calibration/Argus.Calibration.csproj

find . -name "*.cs" -type f -delete
find . -name "*.axaml" -type f -delete
find . -name "*.csproj" -type f -delete

rm -rf obj
rm -rf Argus.RosSharp/obj
rm -rf Argus.StereoCalibration/obj