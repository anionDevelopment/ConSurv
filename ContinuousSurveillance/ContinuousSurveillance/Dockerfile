FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN mkdir /Workspace && \
    mkdir /Workspace/Backend && \
    mkdir /Workspace/Backend/Application && \
    mkdir /Workspace/Frontend && \
    mkdir /Workspace/Frontend/Application && \
    mkdir /Workspace/Other && \
    mkdir /Workspace/Other/EntryPoint
	
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceBackend/BuildResult_DotNet_win-x64 /Workspace/Backend/Application
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceFrontend/BuildResult_Website /Workspace/Frontend/Application

#TODO set healthckeck

HEALTHCHECK CMD curl --insecure --fail https://localhost:443/API/Other/Maintenance/HealthCheck

WORKDIR /Workspace/Application
ENTRYPOINT ["dotnet", "/Workspace/Application/ContinuousSurveillanceBackend.dll"]
