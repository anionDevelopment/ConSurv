#!/bin/bash
export IsRunningInContainer=true

#if [[ ! -v SomeVariable ]]; then
#    echo "SomeVariable is not set"
#elif [[ -z "$SomeVariable" ]]; then
#    echo "SomeVariable is set to the empty string"
#else
#    echo "SomeVariable has the value: $SomeVariable"
#fi

argument=""

if [[ -n "$InitialAdminPassword" ]]; then
    argument="--InitialAdminPassword $InitialAdminPassword"
fi

if [[ -n "$InitialCameraAddresses" ]]; then
    argument="$argument --InitialCameraAddresses $InitialCameraAddresses"
fi

if [[ -n "$DoNotHostFrontend" ]]; then
    #TODO start frontend only when DoNotHostFrontend is not set
fi

{ cd /Workspace/Application/Backend && dotnet ./ConSurvBackend.dll $argument; } &
{ cd /Workspace/Application/Frontend && nginx -c /Workspace/Application/Frontend/nginx.conf -g "daemon off;"; } &

wait -n

pkill -P $$
