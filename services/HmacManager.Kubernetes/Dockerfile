ARG DOTNET_VERSION=10

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK must match the library's highest TFM. DOTNET_VERSION controls the runtime
# image and published framework only; update this line when a new TFM is added.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG DOTNET_VERSION
WORKDIR /repo
COPY ["services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj", "services/HmacManager.Kubernetes/"]
COPY ["src/HmacManager.csproj", "src/"]
RUN dotnet restore "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj"
COPY . .
RUN dotnet build "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net${DOTNET_VERSION}.0 -o /app/build

FROM build AS publish
ARG DOTNET_VERSION
RUN dotnet publish "services/HmacManager.Kubernetes/HmacManager.Kubernetes.csproj" \
    -c Release -f net${DOTNET_VERSION}.0 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HmacManager.Kubernetes.dll"]
