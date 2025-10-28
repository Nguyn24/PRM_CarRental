FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PRM_Project_Assignment.Apis/PRM_Project_Assignment.Apis.csproj", "PRM_Project_Assignment.Apis/"]
RUN dotnet restore "PRM_Project_Assignment.Apis/PRM_Project_Assignment.Apis.csproj"
COPY . .
WORKDIR "/src/PRM_Project_Assignment.Apis"
RUN dotnet build "./PRM_Project_Assignment.Apis.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PRM_Project_Assignment.Apis.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRM_Project_Assignment.Apis.dll"]
