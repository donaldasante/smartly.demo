# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

RUN curl -sL https://deb.nodesource.com/setup_16.x | bash - 
RUN apt-get install -y nodejs

# copy csproj and restore as distinct layers
COPY ./*.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /build
RUN dotnet publish SmartlyDemo.RiotSPA.csproj -c Release -o published --no-cache

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /build/published ./
ENTRYPOINT ["dotnet", "SmartlyDemo.RiotSPA.dll"]