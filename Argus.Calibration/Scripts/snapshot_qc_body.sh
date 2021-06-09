#!/bin/bash

echo "take snapshot using body stereo ..."

lip=$1
rip=$2
dir=$3

echo "body stereo ip: " $ip
echo "snapshot save dir: " $dir

~/RJ1400/vision/src/ipc-capture/BinocularIpcCapture l=$lip r=$rip dir=$dir

sleep 2