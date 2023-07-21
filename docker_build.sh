#!/usr/bin/env bash
if [ $# -eq 0 ]
  then
    tag='erebus'
  else
    tag=$1
fi

docker build --no-cache -t sbu:$tag .
