FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN mkdir /Workspace && \
    mkdir /Workspace/Application && \
    mkdir /Workspace/Application/Backend && \
    mkdir /Workspace/Application/Frontend && \
    mkdir /Workspace/Configuration && \
    mkdir /Workspace/Data && \
    mkdir /Workspace/Logs

ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceBackend/BuildResult_DotNet_win-x64 /Workspace/Application/Backend
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceFrontend/BuildResult_Website /Workspace/Application/Frontend

#TODO set healthckeck

HEALTHCHECK CMD curl --insecure --fail https://localhost:443/API/Other/Maintenance/HealthCheck

WORKDIR /Workspace/Application/Backend
ENTRYPOINT ["dotnet", "./ContinuousSurveillanceBackend.dll"]
