FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300-alpine3.9 AS build
WORKDIR /procExecutor

# copy csproj and restore as distinct layers
COPY *.sln .
COPY procExecutor/*.csproj ./procExecutor/
RUN dotnet restore

# copy everything else and build app
COPY procExecutor/. ./procExecutor/
WORKDIR /app/procExecutor
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.5-alpine3.9 AS runtime
WORKDIR /procExecutor
COPY --from=build /procExecutor/procExecutor/out ./procExecutor