# SchedulingMS

Microservicio de agenda, disponibilidad, ausencias y reservas de Hygea.

## Arquitectura

- `SchedulingMS.Domain`: entidades y reglas del dominio.
- `SchedulingMS.Application`: casos de uso, puertos y DTOs.
- `SchedulingMS.Infrastructure`: EF Core, consultas, comandos e integraciones externas.
- `SchedulingMS.Api`: bootstrap HTTP, middleware, Swagger y controllers.

## Integraciones actuales

- `DirectoryMS` se consume por HTTP para resolver técnicos activos por proveedor.
- Los eventos de reservas se registran mediante `StructuredLogEventPublisher` para dejar trazabilidad local mientras la integración sigue siendo HTTP.
