# Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj files for restore
COPY *.sln .
COPY EngAce.Api/*.csproj ./EngAce.Api/
COPY Events/*.csproj ./Events/
COPY Entities/*.csproj ./Entities/
COPY Gemini/*.csproj ./Gemini/
COPY Helper/*.csproj ./Helper/

# Restore packages
RUN dotnet restore ./EngAce.Api/EngAce.Api.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish ./EngAce.Api/EngAce.Api.csproj -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "EngAce.Api.dll"]