#!/bin/bash
set -e

## Install Plugins
bin/elasticsearch-plugin install analysis-phonetic
bin/elasticsearch-plugin install analysis-icu

exec /usr/local/bin/docker-entrypoint.sh elasticsearch