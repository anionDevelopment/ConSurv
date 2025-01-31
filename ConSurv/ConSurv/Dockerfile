FROM debian:stable-slim

RUN mkdir /Workspace && \
    mkdir /Workspace/Application && \
    mkdir /Workspace/Application/Backend && \
    mkdir /Workspace/Application/Frontend && \
    mkdir /Workspace/Application/Frontend/WebApplicationFiles && \
    mkdir /Workspace/Configuration && \
    mkdir /Workspace/Logs && \
    mkdir /Workspace/Other && \
    mkdir /Workspace/Other/Certificates && \
    mkdir /Workspace/Other/EntryPoint && \
    apt-get update && \
    apt-get install -y wget iputils-ping curl procps nginx

WORKDIR /Workspace/Other/EntryPoint

#Install .NET and ffmpeg
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y aspnetcore-runtime-9.0 ffmpeg

ADD Other/Resources/DependentCodeUnits/ConSurvBackend/BuildResult_DotNet_linux-x64 /Workspace/Application/Backend
ADD Other/Resources/DependentCodeUnits/ConSurvFrontend/BuildResult_WebApplication /Workspace/Application/Frontend/WebApplicationFiles
COPY Other/Resources/DevelopmentCertificate/ConSurvDevelopmentCertificate.crt /Workspace/Other/Certificates
COPY Other/Resources/DevelopmentCertificate/ConSurvDevelopmentCertificate.key /Workspace/Other/Certificates
COPY ConSurv/nginx.configuration /Workspace/Application/Frontend

COPY ConSurv/EntryPoint.sh /Workspace/Other/EntryPoint
RUN chmod +x /Workspace/Other/EntryPoint/EntryPoint.sh

HEALTHCHECK CMD curl --fail http://localhost:80/API/Other/Maintenance/HealthCheck
#TODO add frontend to healthcheck

ENTRYPOINT ["./EntryPoint.sh"]
