ARG NET_VERSION=10

FROM mcr.microsoft.com/dotnet/aspnet:${NET_VERSION}.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK version must match the library's highest TFM (currently net10.0).
# NET_VERSION controls only the runtime image and the published framework.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG NET_VERSION
WORKDIR /repo
COPY ["services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj", "services/HmacManager.Kubernetes/"]
COPY ["src/HmacManager.csproj", "src/"]
RUN dotnet restore "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj"
COPY . .
RUN dotnet build "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net${NET_VERSION}.0 -o /app/build

FROM build AS publish
ARG NET_VERSION
RUN dotnet publish "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net${NET_VERSION}.0 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HmacManager.Kubernetes.dll"]
