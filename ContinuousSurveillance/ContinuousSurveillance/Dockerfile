FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN mkdir /Workspace && \
    mkdir /Workspace/Backend && \
    mkdir /Workspace/Backend/Application && \
    mkdir /Workspace/Frontend && \
    mkdir /Workspace/Frontend/Application && \
    mkdir /Workspace/Other && \
    mkdir /Workspace/Other/EntryPoint && \
	wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
	sudo dpkg -i packages-microsoft-prod.deb && \
	rm packages-microsoft-prod.deb && \
	sudo apt-get update && \
	sudo apt-get install -y dotnet-sdk-8.0 && \
	
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceBackend/BuildResult_DotNet_win-x64 /Workspace/Backend/Application
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceFrontend/BuildResult_Website /Workspace/Frontend/Application

COPY ContinuousSurveillance/EntryPoint.sh /Workspace/Other/EntryPoint

#TODO set healthckeck

HEALTHCHECK CMD curl --insecure --fail https://localhost:443/API/Other/Maintenance/HealthCheck

WORKDIR /Workspace/Application
ENTRYPOINT ["dotnet", "/Workspace/Application/ContinuousSurveillanceBackend.dll"]
