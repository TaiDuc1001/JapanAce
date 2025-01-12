# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first
COPY ["JapanAce.sln", "./"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Events/Events.csproj", "Events/"]
COPY ["Gemini/Gemini.csproj", "Gemini/"]
COPY ["Helper/Helper.csproj", "Helper/"]
COPY ["japanace_backend/japanace_backend.csproj", "japanace_backend/"]
COPY ["japanace_frontend/japanace.web.esproj", "japanace_frontend/"]
COPY ["japanace_setup/japanace.setup.esproj", "japanace_setup/"]

# Restore NuGet packages
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build and publish the API project
RUN dotnet publish "japanace_backend/japanace_backend.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configure the port
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "japanace_backend.dll"]