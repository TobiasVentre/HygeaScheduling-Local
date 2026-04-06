FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/SchedulingMS.Api/SchedulingMS.Api.csproj", "src/SchedulingMS.Api/"]
COPY ["src/SchedulingMS.Application/SchedulingMS.Application.csproj", "src/SchedulingMS.Application/"]
COPY ["src/SchedulingMS.Infrastructure/SchedulingMS.Infrastructure.csproj", "src/SchedulingMS.Infrastructure/"]
COPY ["src/SchedulingMS.Domain/SchedulingMS.Domain.csproj", "src/SchedulingMS.Domain/"]
RUN dotnet restore "src/SchedulingMS.Api/SchedulingMS.Api.csproj"
COPY . .
RUN dotnet publish "src/SchedulingMS.Api/SchedulingMS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SchedulingMS.Api.dll"]
