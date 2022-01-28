#!/bin/bash

sudo mkdir -pv /mnt/efs-{master,develop}
sudo mount -t efs fs-076d94d6dcf153360 /mnt/efs-master
sudo mount -t efs fs-0aff87e0b7aa5d986 /mnt/efs-develop