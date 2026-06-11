FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first to restore dependencies
COPY ["OverLoad.API/OverLoad.API.csproj", "OverLoad.API/"]
COPY ["OverLoad.Services/OverLoad.Services.csproj", "OverLoad.Services/"]
COPY ["OverLoad.Repositories/OverLoad.Repositories.csproj", "OverLoad.Repositories/"]
COPY ["OverLoad.Domain/OverLoad.Domain.csproj", "OverLoad.Domain/"]

# Restore dependencies
RUN dotnet restore "OverLoad.API/OverLoad.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the API project
WORKDIR "/src/OverLoad.API"
RUN dotnet build "OverLoad.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OverLoad.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OverLoad.API.dll"]
