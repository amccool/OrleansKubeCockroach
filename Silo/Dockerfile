#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Silo/Silo.csproj", "Silo/"]
COPY ["HelloWorldGrains/HelloWorldGrains.csproj", "HelloWorldGrains/"]
RUN dotnet restore "Silo/Silo.csproj"
COPY . .
WORKDIR "/src/Silo"
RUN dotnet build "Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Silo.dll"]
EXPOSE 8080/tcp
EXPOSE 30000/tcp
EXPOSE 11111/tcp

CMD ["/bin/bash", "-c", "update-ca-certificates && dotnet Silo.dll"]