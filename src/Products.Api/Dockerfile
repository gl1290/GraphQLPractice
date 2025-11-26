# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/Products.Api/Products.Api.csproj", "src/Products.Api/"]
RUN dotnet restore "src/Products.Api/Products.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Products.Api"
RUN dotnet build "Products.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Products.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 5001
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.Api.dll"]
