# UTNGolCoin.API

Backend del **Servicio UTNGolCoin** — Proyecto Integrador UTN GolMundial 2026 (Persona B).

Tecnología: **ASP.NET Core Web API (.NET 10)** + **Entity Framework Core** + **MySQL 8 (Pomelo)**.

## Ejecución nativa

Este flujo se ejecuta directamente en la máquina, sin Docker y contra servicios reales.

Requisitos:

- .NET SDK 10
- MySQL Server 8
- Servicio de Estadísticas (Guacales) en ejecución

Crear la base una sola vez:

```sql
CREATE DATABASE IF NOT EXISTS utngolcoin_db;
```

La aplicación aplica automáticamente las migraciones EF al iniciar. Si MySQL todavía está arrancando, reintenta la conexión durante aproximadamente 30 segundos antes de terminar con error.

## Configuración

`appsettings.json` conserva la configuración que Mayra ya utilizaba:

- MySQL local en `localhost:3306`.
- Base `utngolcoin_db`.
- Usuario `root` y contraseña `rocio123`.

En la red del equipo solo debe reemplazar la IP de Andrea en
`ServicioEstadisticas:BaseUrl`. En Linux también puede sobrescribirla por entorno:

```bash
export ServicioEstadisticas__BaseUrl='http://IP_DE_ANDREA:8080/demo/api/v1/'
```

La API escucha en `0.0.0.0:5001`; los demás la consumen con
`http://IP_DE_MAYRA:5001/api/`.

## Iniciar

```bash
dotnet restore
dotnet run --no-launch-profile --urls http://0.0.0.0:5001
```

Verificaciones:

```bash
curl http://127.0.0.1:5001/api/salud
curl http://127.0.0.1:5001/swagger/index.html
```

Swagger se habilita con `ASPNETCORE_ENVIRONMENT=Development`.

### Windows con Visual Studio 2022

1. Inicia el servicio MySQL de Windows.
2. Abre `appsettings.json` y cambia únicamente `172.20.132.124` por la IP actual
   de Andrea. Si su contraseña root cambió, actualiza `password=rocio123`.
3. Abre `UTNGolCoinApi.slnx` y ejecuta con **F5**. El perfil escucha en
   `0.0.0.0:5001`. Si Windows pregunta por el firewall, permite redes privadas.

## Flujo real de integración

Usa un `partidoId` real, existente y todavía programado en Guacales:

```bash
# 1. Crear la billetera del usuario real
curl -X POST http://localhost:5001/api/billeteras \
  -H 'Content-Type: application/json' \
  -d '{"usuarioId":7}'

# 2. Registrar una predicción; consulta partido y cuotas reales en Guacales
curl -X POST http://localhost:5001/api/predicciones \
  -H 'Content-Type: application/json' \
  -d '{"usuarioId":7,"partidoId":1,"resultadoPronosticado":"LOCAL","monto":2}'

# 3. Liquidar cuando Guacales registre el resultado oficial
curl -X POST http://localhost:5001/api/liquidaciones/1 \
  -H 'Content-Type: application/json' \
  -d '{"golesLocal":2,"golesVisitante":1}'

# 4. Comprobar saldo y movimientos reales
curl http://localhost:5001/api/billeteras/7
curl http://localhost:5001/api/billeteras/7/transacciones
```

El endpoint académico de simulación se conserva, pero no forma parte de este procedimiento de integración.

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/salud` | Salud de UTNGolCoin. |
| POST | `/api/billeteras` | Crea billetera y bono de bienvenida. |
| GET | `/api/billeteras/{usuarioId}` | Saldo actual. |
| GET | `/api/billeteras/{usuarioId}/transacciones` | Historial completo. |
| POST | `/api/predicciones` | Valida el partido real en Guacales y registra una predicción 1X2. |
| GET | `/api/predicciones/usuario/{usuarioId}` | Historial de predicciones. |
| POST | `/api/liquidaciones/{partidoId}` | Liquida las predicciones con el resultado oficial recibido. |
| GET | `/api/ranking` | Usuarios ordenados por saldo. |

Guacales debe llamar `POST /api/billeteras` al registrar un usuario y `POST /api/liquidaciones/{partidoId}` al guardar el resultado oficial. UTNGolCoin consulta `GET /partidos/{id}` usando `ServicioEstadisticas__BaseUrl`.

Si Guacales no responde, devuelve un error distinto de 404 o entrega una respuesta inválida, los flujos que requieren sus datos responden HTTP 503. Un partido que Guacales confirma como inexistente conserva HTTP 404.
