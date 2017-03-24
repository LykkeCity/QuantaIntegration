FROM microsoft/aspnetcore:1.1.0
ENTRYPOINT ["dotnet", "ApiRunner.dll"]
ARG source=.
WORKDIR /app
COPY $source .

EXPOSE 6000/tcp

ENV ASPNETCORE_URLS http://*:6000
