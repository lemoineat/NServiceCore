FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ASPServiceCore/ASPServiceCore.csproj", "ASPServiceCore/"]
COPY . .
RUN dotnet restore "ASPServiceCore/ASPServiceCore.csproj"
WORKDIR "/src/ASPServiceCore"
RUN dotnet build "ASPServiceCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ASPServiceCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ASPServiceCore.dll"]