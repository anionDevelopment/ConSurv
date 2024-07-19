FROM nginx:latest

RUN mkdir /Workspace && \
    mkdir /Workspace/Application && \
    mkdir /Workspace/Application/Backend && \
    mkdir /Workspace/Application/Frontend && \
    mkdir /Workspace/Configuration && \
    mkdir /Workspace/Logs && \
    mkdir /Workspace/Other && \
    mkdir /Workspace/Other/EntryPoint
# TODO install dotnet

ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceBackend/BuildResult_DotNet_win-x64 /Workspace/Application/Backend
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceFrontend/BuildResult_Website /Workspace/Application/Frontend
COPY ContinuousSurveillance/nginx.configuration /etc/nginx
COPY ContinuousSurveillance/EntryPoint.sh /Workspace/Other/EntryPoint

HEALTHCHECK CMD curl --insecure --fail https://localhost:443/Backend/API/Other/Maintenance/HealthCheck

WORKDIR /Workspace/Other/EntryPoint
ENTRYPOINT ["/Workspace/Other/EntryPoint/EntryPoint.sh"]
