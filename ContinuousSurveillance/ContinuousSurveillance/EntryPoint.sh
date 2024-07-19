#!/bin/bash
{ cd /Workspace/Application/Backend && dotnet ContinuousSurveillanceBackend.dll; } &
{ cd /Workspace/Application/Frontend && ping -c 8 8.8.8.8; } & # TODO run frontend using nginx instead
wait -n
pkill -P $$
