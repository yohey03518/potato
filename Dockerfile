FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Potato.Trading.Cli/Potato.Trading.Cli.csproj", "src/Potato.Trading.Cli/"]
COPY ["src/Potato.Trading.Core/Potato.Trading.Core.csproj", "src/Potato.Trading.Core/"]
COPY ["src/Potato.Trading.Infrastructure/Potato.Trading.Infrastructure.csproj", "src/Potato.Trading.Infrastructure/"]
RUN dotnet restore "src/Potato.Trading.Cli/Potato.Trading.Cli.csproj"
COPY . .
WORKDIR "/src/src/Potato.Trading.Cli"
RUN dotnet build "Potato.Trading.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Potato.Trading.Cli.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Potato.Trading.Cli.dll"]
