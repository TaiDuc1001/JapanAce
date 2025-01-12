# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project files needed for restore
COPY *.sln .
COPY EngAce.Api/*.csproj ./EngAce.Api/
COPY Events/*.csproj ./Events/
COPY Entities/*.csproj ./Entities/
COPY Gemini/*.csproj ./Gemini/
COPY Helper/*.csproj ./Helper/
COPY EngAce.Web/*.*proj ./EngAce.Web/

# Restore packages for all projects
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish ./EngAce.Api/EngAce.Api.csproj -c Release -o /app

# Use the official .NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app .

# Set environment variables and expose ports
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EngAce.Api.dll"]