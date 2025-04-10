FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PPSRegister.PPSUploader.csproj", "./PPSRegister.PPSUploader/"]
COPY ["../PPSRegister.Data/PPSRegister.Data.csproj", "../PPSRegister.Data/"]
COPY ["../PPSRegister.ServiceDefaults/PPSRegister.ServiceDefaults.csproj", "../PPSRegister.ServiceDefaults/"]
RUN dotnet restore "PPSRegister.PPSUploader/PPSRegister.PPSUploader.csproj"
COPY . .
WORKDIR "/src/PPSRegister.PPSUploader"
RUN dotnet build "PPSRegister.PPSUploader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PPSRegister.PPSUploader.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PPSRegister.PPSUploader.dll"]