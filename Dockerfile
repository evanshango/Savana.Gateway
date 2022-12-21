FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Savana.Gateway.csproj", "Savana.Gateway/"]
RUN dotnet restore "Savana.Gateway/Savana.Gateway.csproj"
WORKDIR "/src/Savana.Gateway"
COPY . .
RUN dotnet build "Savana.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Savana.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savana.Gateway.dll"]
