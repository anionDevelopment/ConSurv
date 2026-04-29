#!/bin/bash

export IsRunningInContainer=true

argument="--RealRun"

if [[ -n "$InitialAdminPassword" ]]; then
    argument+=" --InitialAdminPassword $InitialAdminPassword"
fi

if [[ -n "$InitialDatabaseType" ]]; then
    argument+=" --InitialDatabaseType $InitialDatabaseType"
fi

if [[ -n "$InitialDatabaseConnectionString" ]]; then
    argument+=" --InitialDatabaseConnectionString $InitialDatabaseConnectionString"
fi

if [[ -n "$InitialCameraAddresses" ]]; then
    argument+=" --InitialCameraAddresses $InitialCameraAddresses"
fi

if [[ -n "${InitialDomain}" ]]; then
    argument+=" --InitialDomain $InitialDomain"
fi

if [[ -n "${InitialEnableEndpointAvailabilityCheckValue}" ]]; then
    argument+=" --InitialEnableEndpointAvailabilityCheckValue $InitialEnableEndpointAvailabilityCheckValue"
fi

if [[ -n "${InitialEnableEndpointInitializationStateValue}" ]]; then
    argument+=" --InitialEnableEndpointInitializationStateValue $InitialEnableEndpointInitializationStateValue"
fi

if [[ -n "${InitialEnableEndpointCurrentVersionValue}" ]]; then
    argument+=" --InitialEnableEndpointCurrentVersionValue $InitialEnableEndpointCurrentVersionValue"
fi

if [[ -n "${InitialEnableEndpointShowAllEndpointsValue}" ]]; then
    argument+=" --InitialEnableEndpointShowAllEndpointsValue $InitialEnableEndpointShowAllEndpointsValue"
fi

if [[ -n "${InitialEnableEndpointHealthCheckValue}" ]]; then
    argument+=" --InitialEnableEndpointHealthCheckValue $InitialEnableEndpointHealthCheckValue"
fi

if [[ -n "${InitialEnableEndpointMetricsValue}" ]]; then
    argument+=" --InitialEnableEndpointMetricsValue $InitialEnableEndpointMetricsValue"
fi

if [[ -n "${InitialVerboseValue}" ]]; then
    argument+=" --InitialVerboseValue $InitialVerboseValue"
fi

if [[ "${EnforceVerbose}" == "true" ]]; then
    argument+=" --EnforceVerbose true"
fi

if [ -z ${DoNotHostFrontend+x} ]; then
    echo "Frontend will be started.";
else
    echo "Frontend will not be started.";
    #TODO ensure frontend will really not be started.
fi

{ cd /Workspace/Application/Backend && dotnet ./ConSurvBackend.dll $argument; } &
{ cd /Workspace/Application/Frontend && nginx -c /Workspace/Application/Frontend/nginx.conf -g "daemon off;"; } &

wait -n

pkill -P $$
