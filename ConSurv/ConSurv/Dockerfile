ARG image_debian
FROM ${image_debian}
ARG CodeUnitVersion
ARG DotNetRuntime
LABEL version=${CodeUnitVersion}
ENV ISRUNNINGINCONTAINER="true"
#TODO make it possible to have an own apt-server for apt-packages to get rid of rate limits
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
    apt-get install -y curl nginx ffmpeg wget libfontconfig1 libgdiplus uuid-runtime libuuid1 libicu-dev libssl-dev && \
    wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y aspnetcore-runtime-10.0 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /Workspace/Other/EntryPoint

ADD Other/Resources/DependentCodeUnits/ConSurvBackend/BuildResult_DotNet_${DotNetRuntime} /Workspace/Application/Backend
ADD Other/Resources/DependentCodeUnits/ConSurvFrontend/BuildResult_WebApplication /Workspace/Application/Frontend/WebApplicationFiles
COPY Other/Resources/DevelopmentCertificate/ConSurvDevelopmentCertificate.crt /Workspace/Other/Certificates
COPY Other/Resources/DevelopmentCertificate/ConSurvDevelopmentCertificate.key /Workspace/Other/Certificates
COPY ConSurv/nginx.conf /Workspace/Application/Frontend
COPY ConSurv/nginx-api-only.conf /Workspace/Application/Frontend
COPY ConSurv/nginx.loadings-settings.conf /Workspace/Application/Frontend
COPY ConSurv/nginx.loadings-settings-api-only.conf /Workspace/Application/Frontend

COPY ConSurv/EntryPoint.sh /Workspace/Other/EntryPoint
COPY ConSurv/HealthCheck.sh /Workspace/Other/EntryPoint
RUN chmod +x /Workspace/Other/EntryPoint/EntryPoint.sh && \
    chmod +x /Workspace/Other/EntryPoint/HealthCheck.sh

HEALTHCHECK CMD ./HealthCheck.sh

ENTRYPOINT ["./EntryPoint.sh"]
