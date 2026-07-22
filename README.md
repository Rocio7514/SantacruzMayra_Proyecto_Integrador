# UTNGolCoin.API

Backend del **Servicio UTNGolCoin** — Proyecto Integrador UTN GolMundial 2026 (Persona B).

Tecnología: **ASP.NET Core Web API (.NET 10)** + **Entity Framework Core** + **MySQL (Pomelo)**.

## 1. Requisitos previos

- .NET SDK 10 instalado
- MySQL Server 8 corriendo, con la base `utngolcoin_db` creada:
  ```sql
  CREATE DATABASE utngolcoin_db;
  ```

## 2. Restaurar paquetes y regenerar la base de datos

Como las tablas cambiaron de inglés a español, hay que borrar las migraciones/tablas viejas y regenerar:

```bash
dotnet restore
```

En MySQL Workbench, borra las tablas viejas si existían (`wallets`, `transactions`, `predictions`, `dailybonuses`, `__efmigrationshistory`), o simplemente:
```sql
DROP DATABASE utngolcoin_db;
CREATE DATABASE utngolcoin_db;
```

Luego genera la migración nueva:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Esto crea las tablas `billeteras`, `transacciones`, `predicciones` y `bonos_diarios`.

## 3. Configurar la URL del Servicio de Estadísticas (Persona A)

En `appsettings.json`:
```json
"ServicioEstadisticas": {
  "BaseUrl": "http://localhost:8080/demo/api/v1/"
}
```
Actualiza la IP si tu compañera cambia de red.

## 4. Ejecutar el proyecto

```bash
dotnet run --urls http://0.0.0.0:5000
```

- Página de bienvenida: `http://localhost:5000/`
- Swagger: `http://localhost:5000/swagger`

## 5. Endpoints implementados

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/billeteras` | Crea billetera + bono de bienvenida (10 UGC). Lo llama Persona A al registrar un usuario. |
| GET | `/api/billeteras/{usuarioId}` | Saldo actual. |
| GET | `/api/billeteras/{usuarioId}/transacciones` | Historial completo (ledger). |
| GET | `/api/ranking` | Usuarios ordenados por saldo. |
| POST | `/api/predicciones` | Crea una predicción 1X2 (valida saldo, duplicado y que el partido no haya iniciado). |
| GET | `/api/predicciones/usuario/{usuarioId}` | Historial de predicciones de un usuario. |
| POST | `/api/predicciones/bono-diario/{usuarioId}` | Otorga 1 UGC si el saldo es 0 y no se ha recibido bono hoy. |
| POST | `/api/liquidaciones/{partidoId}` | **Endpoint clave**: lo llama Persona A al registrar el resultado oficial de un partido. Body: `{ "golesLocal": 2, "golesVisitante": 1 }`. Liquida todas las predicciones pendientes. |

## 6. Contrato con el Servicio de Estadísticas (Persona A)

**Lo que Persona A debe llamar hacia este servicio:**

1. Al registrar un usuario nuevo:
   ```
   POST http://<tu-IP>:5000/api/billeteras
   Body: { "usuarioId": 7 }
   ```
2. Al registrar el resultado oficial de un partido:
   ```
   POST http://<tu-IP>:5000/api/liquidaciones/{partidoId}
   Body: { "golesLocal": 2, "golesVisitante": 1 }
   ```
   (los mismos goles que ella ya recibió en su `PUT /partidos/{id}/resultado` — no necesita calcular el ganador, este servicio lo hace).

**Lo que este servicio consulta hacia Persona A** (`Services/InfoPartidoClient.cs`):
```
GET http://localhost:8080/demo/api/v1/partidos/{id}
```
Acepta el JSON de Guacales con `fechaHora` + `cuotas.{local,empate,visitante}`, y también el shape plano (`fechaHoraUtc`, `cuotaLocal`, etc.).

## 7. Notas de diseño

- **Autenticación de usuarios**: vive en el Servicio de Estadísticas (Persona A, endpoints `/autenticacion/registro` y `/autenticacion/sesion`, JWT). Este servicio no tiene tabla de usuarios ni de contraseñas — solo referencia `usuarioId` de forma lógica.
- **Ledger inmutable**: la tabla `transacciones` nunca se edita ni se borra, solo se agregan filas nuevas.
- **Restricciones de integridad** aplicadas con índices únicos en la base de datos:
  - Un usuario = una sola predicción por partido (`predicciones.usuario_id + partido_id`).
  - Un usuario = un solo bono diario por día (`bonos_diarios.usuario_id + fecha`).
- **Manejo de errores**: `Middleware/ExceptionHandlingMiddleware.cs` traduce cada regla de negocio violada a un código HTTP claro (400/404/409) con un mensaje JSON.

## 8. Próximos pasos sugeridos

1. Enviar a Persona A la sección 6 de este README con los 2 endpoints que debe llamar.
2. Clonar su repositorio y revisar su `README-setup` para las instrucciones de conexión de su lado.
3. Probar el flujo completo end-to-end una vez ambos backends estén corriendo en la misma red.
4. Agregar pruebas unitarias (xUnit) para `PrediccionService` y `LiquidacionService`.
