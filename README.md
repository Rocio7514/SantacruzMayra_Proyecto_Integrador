# UTNGolCoin.API

Backend del **Servicio UTNGolCoin** — Proyecto Integrador UTN GolMundial 2026 (Persona B).

Tecnología: **ASP.NET Core Web API (.NET 10)** + **Entity Framework Core** + **MySQL 8 (Pomelo)**.

## Ejecución nativa

Este flujo se ejecuta directamente en la máquina, sin Docker y contra servicios reales.

Requisitos:

- .NET SDK 10
- MySQL Server 8
- Servicio de Estadísticas (Guacales) en ejecución
- `make` (opcional; debajo también se muestra el comando `dotnet`)

Crear la base una sola vez:

```sql
CREATE DATABASE IF NOT EXISTS utngolcoin_db;
```

La aplicación aplica automáticamente las migraciones EF al iniciar. Si MySQL todavía está arrancando, reintenta la conexión durante aproximadamente 30 segundos antes de terminar con error.

## Configuración

Los valores locales por defecto son:

- MySQL: `localhost:3306`, base `utngolcoin_db`, usuario `root`, sin contraseña fija.
- Guacales: `http://localhost:18080/demo/api/v1/`.
- UTNGolCoin: `http://localhost:5001`.

Para una instalación con credenciales o puertos distintos, usa los overrides estándar de ASP.NET Core:

```bash
export ConnectionStrings__Default='server=localhost;port=3306;database=utngolcoin_db;user=utngolcoin;password=TU_PASSWORD'
export ServicioEstadisticas__BaseUrl='http://localhost:18080/demo/api/v1/'
```

No guardes contraseñas ni direcciones LAN personales en `appsettings.json`.

En la red del equipo, Mayra solo cambia la IP de Andrea:

```bash
export ServicioEstadisticas__BaseUrl='http://IP_DE_ANDREA:18080/demo/api/v1/'
make run
```

El `Makefile` escucha en `0.0.0.0:5001`; los demás consumen esta API con
`http://IP_DE_MAYRA:5001/api/`.

## Iniciar

Con Make:

```bash
make restore
make run
```

Para elegir otro puerto:

```bash
PORT=5010 make run
```

Comando equivalente sin Make:

```bash
dotnet restore
dotnet run --no-launch-profile --urls http://localhost:5001
```

Verificaciones:

```bash
curl http://localhost:5001/api/salud
curl http://localhost:5001/swagger/index.html
```

Swagger se habilita con `ASPNETCORE_ENVIRONMENT=Development`.

### Windows con Visual Studio 2022

1. Abre `UTNGolCoinApi.slnx`.
2. Inicia el servicio MySQL de Windows y crea `utngolcoin_db`.
3. En **Propiedades del proyecto → Depurar → Abrir perfiles de inicio**, usa
   `http://0.0.0.0:5001`.
4. Agrega estas variables al perfil:

```text
ASPNETCORE_ENVIRONMENT=Development
ServicioEstadisticas__BaseUrl=http://IP_DE_ANDREA:18080/demo/api/v1/
ConnectionStrings__Default=server=localhost;port=3306;database=utngolcoin_db;user=root;password=TU_PASSWORD
```

5. Ejecuta con **F5**. Si Windows pregunta por el firewall, permite redes privadas.

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
