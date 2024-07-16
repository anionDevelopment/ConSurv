FROM nginx:latest

RUN mkdir /Workspace && \
    mkdir /Workspace/Backend && \
    mkdir /Workspace/Backend/Application && \
    mkdir /Workspace/Frontend && \
    mkdir /Workspace/Frontend/Application && \
    mkdir /Workspace/Other && \
    mkdir /Workspace/Other/EntryPoint
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceBackend/BuildResult_DotNet_win-x64 /Workspace/Backend/Application
ADD Other/Resources/DependentCodeUnits/ContinuousSurveillanceFrontend/BuildResult_Website /Workspace/Frontend/Application
COPY ContinuousSurveillance/nginx.configuration /etc/nginx
COPY ContinuousSurveillance/EntryPoint.sh /Workspace/Other/EntryPoint
#TODO set healthckeck
WORKDIR /usr/src/app
ENTRYPOINT ["/Workspace/Other/EntryPoint/EntryPoint.sh"]
