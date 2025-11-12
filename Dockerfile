# ========================
# 1️⃣ Build stage
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore project dependencies
COPY ["WebApplication1/WebApplication1.csproj", "WebApplication1/"]
RUN dotnet restore "WebApplication1/WebApplication1.csproj"

# Copy the rest of the project files
COPY . .
WORKDIR "/src/WebApplication1"

# Publish the app (Release mode)
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app/publish


# ========================
# 2️⃣ Runtime stage
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# (Optional) If you want Render health checks
ENV PORT=5000

# Expose the port Render will use
EXPOSE 5000

# Start the app
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
