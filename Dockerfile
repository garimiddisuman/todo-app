# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy backend code (including wwwroot)
COPY backend ./backend
WORKDIR /app/backend/TodoApp

# Restore and publish to known folder
RUN dotnet restore
RUN dotnet publish -c Release -o /publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output (including static frontend in wwwroot)
COPY --from=build /publish .

# Expose app port
EXPOSE 8000

# Environment: listen on all interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:8000

# Start the app
ENTRYPOINT ["dotnet", "TodoApp.dll"]
