# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install Node.js for Tailwind CSS build
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy package.json and install npm dependencies
COPY package.json ./
RUN npm install

# Copy everything else
COPY . .

# Build Tailwind CSS
RUN npx tailwindcss -i wwwroot/css/tailwind-input.css -o wwwroot/css/tailwind-offline.css --minify

# Publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# ============================================
# Stage 2: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install ICU for globalization support
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

EXPOSE 7777

ENV ASPNETCORE_URLS=http://+:7777
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AdvancedVotingSystem.dll"]
