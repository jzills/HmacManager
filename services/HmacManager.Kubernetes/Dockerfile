FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY ["services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj", "services/HmacManager.Kubernetes/"]
COPY ["src/HmacManager.csproj", "src/"]
RUN dotnet restore "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj"
COPY . .
RUN dotnet build "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net8.0 -o /app/build

FROM build AS publish
RUN dotnet publish "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net8.0 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HmacManager.Kubernetes.dll"]
