FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5599

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files (relative to solution root)
COPY Src/Timesheet.API/Timesheet.API.csproj Timesheet.API/
COPY Src/Timesheet.Infra/Timesheet.Infra.csproj Timesheet.Infra/
COPY Src/Timesheet.App/Timesheet.App.csproj Timesheet.App/
COPY Src/Timesheet.Core/Timesheet.Core.csproj Timesheet.Core/
COPY Src/Timesheet.Shared/Timesheet.Shared.csproj Timesheet.Shared/
COPY Src/Timesheet.UI/Timesheet.UI.csproj Timesheet.UI/
COPY Tests/Timesheet.Tests/Timesheet.Tests.csproj Timesheet.Tests/

RUN dotnet restore Timesheet.API/Timesheet.API.csproj

# Copy all remaining source code
COPY . .

# Run tests
WORKDIR /src/Tests/Timesheet.Tests
RUN dotnet test --verbosity normal

# Publish API (includes Blazor output)
WORKDIR /src/Src/Timesheet.API
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Timesheet.API.dll"]
