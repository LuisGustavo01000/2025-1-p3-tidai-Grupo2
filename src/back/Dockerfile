FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY CashWiseAPI.csproj ./
RUN dotnet restore CashWiseAPI.csproj
COPY . ./
RUN dotnet publish CashWiseAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5284
EXPOSE 5284
ENTRYPOINT ["dotnet", "CashWiseAPI.dll"]
