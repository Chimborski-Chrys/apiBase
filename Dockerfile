# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["BaseApi.sln", "./"]
COPY ["src/BaseApi.Domain/BaseApi.Domain.csproj", "src/BaseApi.Domain/"]
COPY ["src/BaseApi.Application/BaseApi.Application.csproj", "src/BaseApi.Application/"]
COPY ["src/BaseApi.Infra/BaseApi.Infra.csproj", "src/BaseApi.Infra/"]
COPY ["src/BaseApi.Api/BaseApi.Api.csproj", "src/BaseApi.Api/"]

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/src/BaseApi.Api"
RUN dotnet build -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Run the application
ENTRYPOINT ["dotnet", "BaseApi.Api.dll"]
