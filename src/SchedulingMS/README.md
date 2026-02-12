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
