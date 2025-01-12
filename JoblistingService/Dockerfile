# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9002

# Build stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

# Copy the .csproj file explicitly from the current directory
COPY JoblistingService.csproj ./

# Restore the project’s dependencies
RUN dotnet restore -a $TARGETARCH "JoblistingService.csproj"

# Copy the rest of the application files to the container
COPY . .

# Build the project
RUN dotnet build -a $TARGETARCH "JoblistingService.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
ARG TARGETARCH
RUN dotnet publish -a $TARGETARCH "JoblistingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:9002
ENTRYPOINT ["dotnet", "JoblistingService.dll"]