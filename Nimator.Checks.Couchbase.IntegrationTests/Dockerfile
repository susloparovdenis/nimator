FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY Nimator.sln ./
COPY Nimator.Checks.Couchbase.IntegrationTests/Nimator.Checks.Couchbase.IntegrationTests.csproj Nimator.Checks.Couchbase.IntegrationTests/
COPY Nimator.Checks.Couchbase/Nimator.Checks.Couchbase.csproj Nimator.Checks.Couchbase/
COPY Nimator/Nimator.csproj Nimator/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Nimator.Checks.Couchbase.IntegrationTests
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Nimator.Checks.Couchbase.IntegrationTests.dll"]