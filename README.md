# UTNGolCoin.API

Backend del **Servicio UTNGolCoin** — Proyecto Integrador UTN GolMundial 2026 (Persona B).

Tecnología: **ASP.NET Core Web API (.NET 10)** + **Entity Framework Core** + **MySQL (Pomelo)**.

## 1. Requisitos previos

- .NET SDK 10 instalado (https://dotnet.microsoft.com/download)
- MySQL Server 8 corriendo, con la base `utngolcoin_db` creada:
  ```sql
  CREATE DATABASE utngolcoin_db;
  ```
- Ajusta la cadena de conexión en `appsettings.json` si tu usuario/contraseña de MySQL son distintos.

## 2. Restaurar paquetes NuGet

Este proyecto **no incluye** la carpeta `bin/`, `obj/` ni los paquetes NuGet — se descargan al restaurar:

```bash
cd UTNGolCoinApi
dotnet restore
```

## 3. Crear la primera migración y aplicarla a la base de datos

```bash
dotnet tool install --global dotnet-ef   # solo la primera vez, si no lo tienes
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Esto crea las tablas `Wallets`, `Transactions`, `Predictions` y `DailyBonuses` con los índices únicos ya configurados en `Data/ApplicationDbContext.cs`.

## 4. Configurar la IP del Servicio de Estadísticas (Persona A)

En `appsettings.json`, cambia:
```json
"ServicioEstadisticas": {
  "BaseUrl": "http://192.168.1.11:8080/api/"
}
```
por la IP real de la laptop de tu compañero(a) en la red del equipo (ver manual de red del proyecto).

## 5. Ejecutar el proyecto

Para que sea visible desde otras laptops de la red (requisito del proyecto):
```bash
dotnet run --urls http://0.0.0.0:5000
```

Swagger (documentación interactiva de todos los endpoints) queda disponible en:
```
http://localhost:5000/swagger
```

## 6. Endpoints implementados

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/wallet/create` | Crea billetera + bono de bienvenida (10 UGC). Lo llama Persona A al registrar un usuario. |
| GET | `/wallet/{userId}` | Saldo actual. |
| GET | `/wallet/{userId}/transactions` | Historial completo (ledger). |
| GET | `/ranking` | Usuarios ordenados por saldo. |
| POST | `/prediction` | Crea una predicción 1X2 (valida saldo, duplicado y que el partido no haya iniciado). |
| GET | `/prediction/user/{userId}` | Historial de predicciones de un usuario. |
| POST | `/dailybonus/{userId}` | Otorga 1 UGC si el saldo es 0 y no se ha recibido bono hoy. |
| POST | `/api/rewards/process` | **Endpoint clave**: lo llama Persona A al registrar el resultado oficial de un partido. Liquida todas las predicciones pendientes. |

## 7. Notas de diseño

- **Autenticación de usuarios**: vive en el Servicio de Estadísticas (Persona A). Este servicio no tiene tabla de usuarios ni de contraseñas — solo referencia `userId` de forma lógica.
- **Ledger inmutable**: la tabla `Transactions` nunca se edita ni se borra, solo se agregan filas nuevas (ver `Models/Transaction.cs`).
- **Restricciones de integridad** aplicadas con índices únicos en la base de datos (no solo validación en código):
  - Un usuario = una sola predicción por partido (`Predictions.UserId + MatchId`).
  - Un usuario = un solo bono diario por día (`DailyBonuses.UserId + Date`).
- **Manejo de errores**: `Middleware/ExceptionHandlingMiddleware.cs` traduce cada regla de negocio violada (saldo insuficiente, predicción duplicada, partido ya iniciado, etc.) a un código HTTP claro (400/404/409) con un mensaje JSON, en vez de un error 500 genérico.

## 8. Próximos pasos sugeridos

1. Probar cada endpoint con Postman contra `localhost` primero, y luego contra tu IP real desde otra laptop del equipo.
2. Coordinar con la Persona A el contrato exacto de `GET /api/partidos/{id}` (campos `estado`, `fechaHoraUtc`, `cuotaLocal`, `cuotaEmpate`, `cuotaVisitante`) para que `MatchInfoClient` deserialice correctamente.
3. Agregar pruebas unitarias (xUnit) para `PredictionService` y `RewardService` — son la lógica más sensible del proyecto.
