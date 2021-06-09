#!/bin/bash

echo "take snapshot using arm stereo ..."

ip=$1
dir=$2

echo "arm stereo ip: " $ip
echo "snapshot save dir: " $dir

~/RJ1400/vision/src/ipc-capture/BinocularIpcCapture l=$ip/dwtype=0 r=$ip/dwtype=1 dir=$dir

sleep 2