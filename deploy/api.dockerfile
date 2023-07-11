FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5032

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY api/SimpleAuth/SimpleAuth.csproj /src/api/SimpleAuth/
RUN dotnet restore ./api/SimpleAuth/SimpleAuth.csproj
WORKDIR /src
COPY ./api /src
RUN dotnet build .api/SimpleAuth/SimpleAuth.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish SimpleAuth.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleAuth.dll"]
