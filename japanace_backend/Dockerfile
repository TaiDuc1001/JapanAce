# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files first
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Events/Events.csproj", "Events/"]
COPY ["Gemini/Gemini.csproj", "Gemini/"]
COPY ["Helper/Helper.csproj", "Helper/"]
COPY ["japanace_backend/japanace_backend.csproj", "japanace_backend/"]
COPY ["japanace_frontend/japanace.web.esproj", "japanace_frontend/"]
COPY ["japanace_setup/japanace.setup.esproj", "japanace_setup/"]

# Copy the source code
COPY ["Entities/", "Entities/"]
COPY ["Events/", "Events/"]
COPY ["Gemini/", "Gemini/"]
COPY ["Helper/", "Helper/"]
COPY ["japanace_backend/", "japanace_backend/"]

# Build and publish the API project directly
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