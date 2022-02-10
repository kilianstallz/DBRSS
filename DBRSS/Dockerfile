FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DBRSS/DBRSS.csproj", "DBRSS/"]
RUN dotnet restore "DBRSS/DBRSS.csproj"
COPY . .
WORKDIR "/src/DBRSS"
RUN dotnet build "DBRSS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DBRSS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBRSS.dll"]
