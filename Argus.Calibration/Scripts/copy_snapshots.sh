echo "delete folder images"
rm -r $HOME/RJ1400/temp/images/*
sleep 1
echo "stereo snapshot..."
# $HOME/RJ1400/vision/src/Cpp_Save/Cpp_Save $HOME/RJ1400/temp/images
$HOME/RJ1400/vision/src/ipc-capture/BinocularIpcCapture l=192.168.1.66/dwtype=0 r=192.168.1.66/dwtype=1 dir=$HOME/RJ1400/temp/images

sleep 1
mv $HOME/RJ1400/temp/images/left/* $HOME/RJ1400/temp/images/left/Left1.png
mv $HOME/RJ1400/temp/images/right/* $HOME/RJ1400/temp/images/right/Right1.png
