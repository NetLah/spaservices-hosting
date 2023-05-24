#ARG ASPNET_IMAGE_TAG=6.0
#ARG ASPNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}
ARG ASPNET_IMAGE_TAG=latest
ARG ASPNET_IMAGE=netlah/aspnet-webssh:${ASPNET_IMAGE_TAG}
FROM ${ASPNET_IMAGE} AS base
WORKDIR /app
EXPOSE 80

ARG ASPNET_IMAGE_TAG=6.0
ARG SDK_IMAGE_TAG=${ASPNET_IMAGE_TAG}
ARG SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:${SDK_IMAGE_TAG}
FROM mcr.microsoft.com/dotnet/sdk:${ASPNET_IMAGE_TAG} AS restore
WORKDIR /src
COPY ["Directory.Build.props", "Directory.Build.targets", "NetLah.SpaServices.sln", "./"]
COPY ["src/Hosting/Hosting.csproj", "src/Hosting/Directory.Build.targets", "src/Hosting/"]
COPY ["src/WebApp/WebApp.csproj", "src/WebApp/"]
COPY ["test/WebApp.Test/WebApp.Test.csproj", "test/WebApp.Test/"]
RUN dotnet restore

FROM restore AS build
COPY . .
ARG VERSION=1.0.0-dev
ENV MinVerVersionOverride=$VERSION
ARG CONFIGURATION=Debug
RUN dotnet build -c ${CONFIGURATION} --no-restore

FROM build AS publish
ARG CONFIGURATION=Debug
RUN dotnet publish src/WebApp -c ${CONFIGURATION} --no-build -o /publish/webApp

FROM base AS final
COPY --from=publish /publish/webApp .
ENTRYPOINT ["/opt/startup/init_container.sh", "exec", "dotnet", "WebApp.dll"]
