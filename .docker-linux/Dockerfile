ARG ASPNET_IMAGE_TAG=latest
ARG ASPNET_IMAGE=netlah/aspnet-webssh:${ASPNET_IMAGE_TAG}
FROM ${ASPNET_IMAGE}
WORKDIR /app
EXPOSE 80
COPY app .
ENTRYPOINT ["/opt/startup/init_container.sh"]
CMD ["dotnet", "WebApp.dll"]
