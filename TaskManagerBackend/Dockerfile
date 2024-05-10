FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-prep
WORKDIR /app
COPY . ./src
RUN mkdir ./proj && cd ./src && \
  find . -type f -a \( -iname "*.sln" -o -iname "*.csproj" \) \
    -exec cp --parents "{}" ../proj/ \;

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
WORKDIR /app
COPY --from=dotnet-prep /app/proj ./src
RUN dotnet restore ./src
COPY --from=dotnet-prep /app/src ./src
RUN dotnet publish ./src -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as dotnet-runtime
WORKDIR /app
COPY --from=dotnet-build /app/out .
ENTRYPOINT ["dotnet", "TaskManagerBackend.dll"]