# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project files needed for restore
COPY *.sln .
COPY japanace_backend/*.csproj ./japanace_backend/
COPY Events/*.csproj ./Events/
COPY Entities/*.csproj ./Entities/
COPY Gemini/*.csproj ./Gemini/
COPY Helper/*.csproj ./Helper/
COPY japanace_setup/*.*proj ./japanace_setup/

# Restore packages for all projects
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish ./japanace_backend/japanace_backend.csproj -c Release -o /app

# Use the official .NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app .

# Set environment variables and expose ports
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "japanace_backend.dll"]