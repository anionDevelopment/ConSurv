#!/bin/bash
export IsRunningInContainer=true

echo "Running ConSurv with arguments: $@"

{ cd /Workspace/Application/Backend && dotnet ./ConSurvBackend.dll "$@"; } &
{ cd /Workspace/Application/Frontend && nginx -c /Workspace/Application/Frontend/nginx.conf -g "daemon off;"; } &

wait -n

pkill -P $$
