FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FreelanceFlow.Backend/FreelanceFlow.Backend.csproj", "FreelanceFlow.Backend/"]
RUN dotnet restore "FreelanceFlow.Backend/FreelanceFlow.Backend.csproj"
COPY FreelanceFlow.Backend/ FreelanceFlow.Backend/
WORKDIR /src/FreelanceFlow.Backend
RUN dotnet build "FreelanceFlow.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FreelanceFlow.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreelanceFlow.Backend.dll"]
