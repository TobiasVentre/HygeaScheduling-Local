# SchedulingMS - Integracion

## Dependencias externas

- `DirectoryMS` expone `GET /api/v1/technician-profiles/by-provider/{providerEntityId}`.
- `SchedulingMS` consume ese endpoint para preasignacion y reasignacion de reservas.

## Configuracion local

En [appsettings.json](/abs/path/C:/Users/Usuario/Documents/Hygea/SchedulingMS/src/SchedulingMS.Api/appsettings.json) y [appsettings.Development.json](/abs/path/C:/Users/Usuario/Documents/Hygea/SchedulingMS/src/SchedulingMS.Api/appsettings.Development.json) se define:

```json
"Integrations": {
  "DirectoryMS": {
    "BaseUrl": "http://localhost:5102/",
    "TimeoutSeconds": 10
  }
}
```

## Docker

Ejemplo de bloque para `docker-compose.yml` raiz:

```yaml
  schedulingms:
    build:
      context: .
      dockerfile: src/SchedulingMS.Api/Dockerfile
    container_name: schedulingms
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
      - Integrations__DirectoryMS__BaseUrl=http://directoryms:8080/
    ports:
      - "5104:8080"
    networks:
      - hygea-network
```

Y dentro de `networks`:

```yaml
networks:
  hygea-network:
    driver: bridge
```

## Eventos

Durante esta fase HTTP, `SchedulingMS` registra los eventos de integracion en logs estructurados. El puerto `IEventPublisher` se conserva para mantener desacoplados los casos de uso del mecanismo concreto de publicacion.
