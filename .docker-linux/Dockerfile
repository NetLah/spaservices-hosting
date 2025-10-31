ARG ASPNET_IMAGE_TAG=6.0-alpine
ARG ASPNET_IMAGE_REPO=netlah/aspnet-webssh
ARG ASPNET_IMAGE=${ASPNET_IMAGE_REPO}:${ASPNET_IMAGE_TAG}
FROM ${ASPNET_IMAGE}
WORKDIR /app
EXPOSE 80
COPY app .
ENTRYPOINT ["/opt/startup/init_container.sh", "exec", "dotnet", "WebApp.dll"]
