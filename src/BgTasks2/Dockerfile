#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/BgTasks2/BgTasks2.csproj", "src/BgTasks2/"]
RUN dotnet restore "src/BgTasks2/BgTasks2.csproj"
COPY . .
WORKDIR "/src/src/BgTasks2"
RUN dotnet build "BgTasks2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BgTasks2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true