# Use the official .NET SDK image to build and publish the backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy backend source
COPY backend ./backend
WORKDIR /app/backend/TodoApp

# Restore and publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish -r linux-arm64 --self-contained false

# Use the official .NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published backend
COPY --from=build /app/publish .

# Copy frontend static files to the published output (served by .NET)
COPY frontend ./wwwroot

# Expose the port the app runs on
EXPOSE 8000

# Set ASP.NET Core to listen on all interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:8000

# Start the application
ENTRYPOINT ["dotnet", "TodoApp.dll"]
