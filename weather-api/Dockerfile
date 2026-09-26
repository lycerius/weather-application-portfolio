# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["weather-api.csproj", "."]
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# ASP.NET Core 8+ images listen on 8080 by default (ASPNETCORE_HTTP_PORTS)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "weather-api.dll"]