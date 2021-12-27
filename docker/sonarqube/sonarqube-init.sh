#!/bin/bash
# per https://hub.docker.com/_/sonarqube
# to be executed before launching the app

sysctl -w vm.max_map_count=262144
sysctl -w fs.file-max=65536
ulimit -n 65536
ulimit -u 4096