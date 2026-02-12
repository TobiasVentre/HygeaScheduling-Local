FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SchedulingMS/SchedulingMS.csproj", "src/SchedulingMS/"]
COPY ["src/SchedulingMS.Application/SchedulingMS.Application.csproj", "src/SchedulingMS.Application/"]
COPY ["src/SchedulingMS.Infrastructure/SchedulingMS.Infrastructure.csproj", "src/SchedulingMS.Infrastructure/"]
COPY ["src/SchedulingMS.Domain/SchedulingMS.Domain.csproj", "src/SchedulingMS.Domain/"]
RUN dotnet restore "src/SchedulingMS/SchedulingMS.csproj"
COPY . .
RUN dotnet publish "src/SchedulingMS/SchedulingMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SchedulingMS.dll"]
