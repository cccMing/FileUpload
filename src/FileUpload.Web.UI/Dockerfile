#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
ENV ASPNETCORE_ENVIRONMENT=Docker
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["FileUpload.Web.UI.csproj", "src/"]
RUN dotnet restore "FileUpload.Web.UI.csproj"
COPY . .
RUN dotnet build "FileUpload.Web.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileUpload.Web.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileUpload.Web.UI.dll"]