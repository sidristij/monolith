FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Joined/Joined.csproj", "Joined/"]
RUN dotnet restore "Joined/Joined.csproj"
COPY . .
WORKDIR "/src/Joined"
RUN dotnet build "Joined.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Joined.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Joined.dll"]
