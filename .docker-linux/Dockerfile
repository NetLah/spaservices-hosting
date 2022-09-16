ARG ASPNET_IMAGE_TAG=6.0-alpine
ARG ASPNET_IMAGE=mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}
FROM ${ASPNET_IMAGE}
WORKDIR /app
EXPOSE 80
RUN apk update && apk add --no-cache ca-certificates tzdata
ENV TZ=UTC
COPY app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
