#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["tests/Issue1356/Issue1356.csproj", "tests/Issue1356/"]
COPY ["src/ImageSharp/ImageSharp.csproj", "src/ImageSharp/"]
RUN dotnet restore "tests/Issue1356/Issue1356.csproj"
COPY . .
WORKDIR "/src/tests/Issue1356"
RUN dotnet build "Issue1356.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Issue1356.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Issue1356.dll"]