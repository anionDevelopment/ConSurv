#!/bin/bash
export IsRunningInContainer=true

{ cd /Workspace/Application/Backend && dotnet ./ConSurvBackend.dll; } &
{ cd /Workspace/Application/Frontend && nginx -c /Workspace/Application/Frontend/nginx.configuration -g "daemon off;"; } &

wait -n

pkill -P $$
