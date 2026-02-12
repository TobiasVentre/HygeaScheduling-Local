# SchedulingMS

Microservicio de agenda y reservas para Hygea bajo arquitectura por capas:

- SchedulingMS.Domain
- SchedulingMS.Application
- SchedulingMS.Infrastructure
- SchedulingMS (API)

## Endpoints

- `POST /api/v1/availability`
- `PUT /api/v1/availability/{id}`
- `DELETE /api/v1/availability/{id}`
- `GET /api/v1/availability/technician/{technicianId}?from=...&to=...`
- `POST /api/v1/reservations`
- `GET /api/v1/reservations/{reservationId}`
- `GET /api/v1/reservations/client/{clientId}`
- `GET /api/v1/reservations/technician/{technicianId}`
- `PATCH /api/v1/reservations/{reservationId}/status`

## Swagger UI

- UI: `/swagger`
- OpenAPI JSON: `/swagger/v1/swagger.json`
- Puedes deshabilitarlo con configuración:

```json
"Swagger": {
  "Enabled": false
}
```

## Deploy local (rápido)

1. Ir al proyecto API:

```bash
cd src/SchedulingMS
```

2. Ejecutar en perfil HTTP (recomendado para desarrollo local):

```bash
dotnet run --launch-profile SchedulingMS-http
```

3. Abrir en navegador:

- <http://localhost:5000/swagger>

> Importante: mantén abierta la consola donde se ejecuta `dotnet run`.

## Troubleshooting

- Si ves `ERR_CONNECTION_REFUSED`, normalmente es porque la app se detuvo o cerraste la consola.
- Este servicio usa repositorios **in-memory**, por lo que **no requiere migraciones EF** para correr Swagger y endpoints.
- Si quieres usar HTTPS local, usa:

```bash
dotnet run --launch-profile SchedulingMS-https
```

Y abre:

- <https://localhost:7001/swagger>
