# ---------- Stage 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files
COPY *.sln .
COPY DataAccessAPI/*.csproj ./DataAccessAPI/
COPY DataAccessLibraries/*.csproj ./DataAccessLibraries/
COPY bgbahasajerman_BusinessLogic/*.csproj ./bgbahasajerman_BusinessLogic/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish the app
RUN dotnet publish DataAccessAPI/bgbahasajerman_DataAccessAPI.csproj -c Release -o /app/publish

# ---------- Stage 2: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Set production environment variable
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published output from build stage
COPY --from=build /app/publish .

# Create wwwroot directory if it doesn't exist
RUN mkdir -p wwwroot

# Expose port
EXPOSE 80

# Set app to listen on port 80 inside container
ENV ASPNETCORE_URLS=http://+:80

# Run the app
ENTRYPOINT ["dotnet", "bgbahasajerman_DataAccessAPI.dll"]
