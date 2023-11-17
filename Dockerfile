FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY EStoreDemo.sln ./
COPY ./src/WebAPI/*.csproj ./src/WebAPI/
COPY ./src/Application/*.csproj ./src/Application/
COPY ./src/Infrastructure/*.csproj ./src/Infrastructure/
COPY ./src/Domain/*.csproj ./src/Domain/
RUN dotnet restore "src/WebAPI/WebAPI.csproj"
RUN dotnet restore "src/Application/Application.csproj"
RUN dotnet restore "src/Infrastructure/Infrastructure.csproj"
RUN dotnet restore "src/Domain/Domain.csproj"
COPY . .
RUN dotnet build "./src/WebAPI/WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/WebAPI/WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Linux 設定環境變數或 dotnet run dll 時當作參數傳入
# Ref:https://stackoverflow.com/questions/75897574/dotnet-publish-penvironmentname-staging-not-work
ENTRYPOINT ["dotnet", "WebAPI.dll", "environment=Development"]