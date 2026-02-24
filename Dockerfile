FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app
COPY . .
RUN dotnet restore && dotnet publish -c Release -o out

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

WORKDIR /app/out
ENTRYPOINT ["dotnet", "LokiGrafana.dll"]