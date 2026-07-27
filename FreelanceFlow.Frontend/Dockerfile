FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FreelanceFlow.Frontend/FreelanceFlow.Frontend.csproj", "FreelanceFlow.Frontend/"]
RUN dotnet restore "FreelanceFlow.Frontend/FreelanceFlow.Frontend.csproj"
COPY FreelanceFlow.Frontend/ FreelanceFlow.Frontend/
WORKDIR /src/FreelanceFlow.Frontend
RUN dotnet build "FreelanceFlow.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FreelanceFlow.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreelanceFlow.Frontend.dll"]
