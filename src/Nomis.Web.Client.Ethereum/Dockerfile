#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Nomis.Web.Client.Ethereum/Nomis.Web.Client.Ethereum.csproj", "src/Nomis.Web.Client.Ethereum/"]
RUN dotnet restore "src/Nomis.Web.Client.Ethereum/Nomis.Web.Client.Ethereum.csproj"
COPY . .
WORKDIR "/src/src/Nomis.Web.Client.Ethereum"
RUN dotnet build "Nomis.Web.Client.Ethereum.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nomis.Web.Client.Ethereum.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nomis.Web.Client.Ethereum.dll"]