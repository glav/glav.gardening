#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Agents/Glav.Gardening.Services.Agents/Glav.Gardening.Services.Agents.csproj", "Services/Agents/Glav.Gardening.Services.Agents/"]
RUN dotnet restore "Services/Agents/Glav.Gardening.Services.Agents/Glav.Gardening.Services.Agents.csproj"
COPY . .
WORKDIR "/src/Services/Agents/Glav.Gardening.Services.Agents"
RUN dotnet build "Glav.Gardening.Services.Agents.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Glav.Gardening.Services.Agents.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Glav.Gardening.Services.Agents.GardenOrg.dll"]