# Notas del Frontend Público (Persona 4)

El portal de apuestas (`frontend-publico-mvc`, puerto 5081) consume esta API.

## Base URL local

```
http://localhost:5001/api
```

## Endpoints usados por el frontend

| Acción | Ruta |
|---|---|
| Saldo | `GET /billeteras/{usuarioId}` |
| Historial | `GET /billeteras/{usuarioId}/transacciones` |
| Crear predicción 1X2 | `POST /predicciones` |
| Mis predicciones | `GET /predicciones/usuario/{usuarioId}` |
| Ranking | `GET /ranking` |
| Bono diario | `POST /predicciones/bono-diario/{usuarioId}` |

El `usuarioId` lo emite el Servicio de Estadísticas (Guacales) al registrarse.
Este servicio no autentica JWT: confía en el id lógico del usuario.

## Partidos / cuotas

Al crear una predicción, UTNGolCoin consulta Guacales
`GET /demo/api/v1/partidos/{id}` (shape con `fechaHora` + `cuotas` anidadas o campos planos).
