#!/bin/bash

robot=$1

$index=1
while read LINE
do
    sed '1c $LINE' ~/jhd/v3/install/share/scene/data/parameters/$robot/calibration_data.txt
    ((index++))
done <  ~/jhd/v3/install/share/scene/data/parameters/$robot/turnable/turnable.txt