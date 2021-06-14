FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/query-plant.csproj", "./"]
RUN dotnet restore "./query-plant.csproj"
COPY . ./
WORKDIR "/src"
RUN dotnet build "query-plant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "query-plant.csproj" -c Release -o /app/publish
#RUN dotnet publish "query-plant.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "query-plant.dll"]
