FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HELMO-bilite/HELMO-bilite.csproj", "HELMO-bilite/"]
RUN dotnet restore "HELMO-bilite/HELMO-bilite.csproj"
COPY . .
WORKDIR "/src/HELMO-bilite"
RUN dotnet build "HELMO-bilite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HELMO-bilite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HELMO-bilite.dll"]
