# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Install MeCab dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends mecab libmecab-dev mecab-ipadic-utf8

# Set environment variables for NMeCab
ENV MECAB_PATH=/usr/lib/libmecab.so
ENV MECAB_DIC_DIR=/usr/lib/x86_64-linux-gnu/mecab/dic/ipadic
ENV LD_LIBRARY_PATH=/usr/lib/x86_64-linux-gnu:${LD_LIBRARY_PATH}

# Copy only the project files needed for restore
COPY *.sln .
COPY EngAce.Api/*.csproj ./EngAce.Api/
COPY Events/*.csproj ./Events/
COPY Entities/*.csproj ./Entities/
COPY Gemini/*.csproj ./Gemini/
COPY Helper/*.csproj ./Helper/
COPY EngAce.Web/*.*proj ./EngAce.Web/

# Clear NuGet cache and restore packages
RUN dotnet nuget locals all --clear
RUN dotnet restore Helper/Helper.csproj
RUN dotnet restore EngAce.Api/EngAce.Api.csproj

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish ./EngAce.Api/EngAce.Api.csproj -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install MeCab and its dependencies in the runtime image
RUN apt-get update && \
    apt-get install -y --no-install-recommends mecab libmecab-dev mecab-ipadic-utf8 && \
    rm -rf /var/lib/apt/lists/*

# Set environment variables for NMeCab
ENV MECAB_PATH=/usr/lib/libmecab.so
ENV MECAB_DIC_DIR=/usr/lib/x86_64-linux-gnu/mecab/dic/ipadic
ENV LD_LIBRARY_PATH=/usr/lib/x86_64-linux-gnu:${LD_LIBRARY_PATH}

# Copy the published application
COPY --from=build /app .

# Set environment variables and expose ports
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EngAce.Api.dll"]