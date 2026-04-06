# ADR-001 - SchedulingMS Architecture

## Estado

Aceptado

## Contexto

`SchedulingMS` ya seguia una separacion razonable por capas, pero mantenia dos adaptadores temporales en infraestructura:

- `InMemoryTechnicianDirectoryGateway`
- `NoOpEventPublisher`

Eso dejaba la integracion con `DirectoryMS` sin contrato real y no cumplia con la arquitectura objetivo del baseline.

## Decision

- Mantener la estructura `Domain/Application/Infrastructure/Api`.
- Conservar los casos de uso por operacion en `Application`.
- Resolver la consulta de tecnicos activos mediante un adaptador HTTP tipado hacia `DirectoryMS`.
- Mantener `IEventPublisher` como puerto estable para desacoplar el registro de eventos de los casos de uso.
- Para la fase actual basada en HTTP, publicar eventos mediante `StructuredLogEventPublisher`.

## Consecuencias

- `SchedulingMS` deja de depender de datos `InMemory` para preasignacion y reasignacion.
- La URL de `DirectoryMS` pasa a ser un requisito explicito de configuracion.
- El mecanismo de publicacion puede evolucionar sin tocar los casos de uso.
- La respuesta de errores HTTP queda normalizada mediante `ApiError` y middleware global consistente.
