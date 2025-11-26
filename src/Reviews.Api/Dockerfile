# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/Reviews.Api/Reviews.Api.csproj", "src/Reviews.Api/"]
RUN dotnet restore "src/Reviews.Api/Reviews.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Reviews.Api"
RUN dotnet build "Reviews.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Reviews.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 5002
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reviews.Api.dll"]
