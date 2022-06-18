FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["First/First.csproj", "First/"]
RUN dotnet restore "First/First.csproj"
COPY . .
WORKDIR "/src/First"
RUN dotnet build "First.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "First.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "First.dll"]
