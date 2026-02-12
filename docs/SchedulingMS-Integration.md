# SchedulingMS - Integración en docker-compose

Agrega el siguiente bloque al `docker-compose.yml` raíz:

```yaml
  schedulingms:
    build:
      context: .
      dockerfile: src/SchedulingMS/Dockerfile
    container_name: schedulingms
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
    ports:
      - "5080:8080"
    networks:
      - hygea-network
```

Y dentro de `networks`:

```yaml
networks:
  hygea-network:
    driver: bridge
```
