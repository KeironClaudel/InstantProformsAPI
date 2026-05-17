# Supabase RLS Future Multitenant Policy

Este documento prepara una politica futura para el caso en que `InstantProforms` use acceso directo a Postgres por medio de Supabase Auth en vez de pasar siempre por el backend .NET.

## Estado actual

Hoy la app:

- autentica y autoriza desde el backend .NET
- usa cookies/JWT propios del backend
- conecta a Postgres como `postgres`
- no usa `anon` ni `authenticated` para consultar tablas de negocio desde el frontend

Por eso la politica actualmente aplicada en Supabase es de bloqueo total para `anon` y `authenticated`.

## Cuando usar esta politica futura

Usa la politica del archivo `supabase-rls-future-multitenant.sql` solo si en el futuro decides:

- usar Supabase Auth
- emitir JWT de Supabase al frontend
- consultar tablas de negocio directamente desde el cliente o Edge Functions usando el rol `authenticated`

## Supuestos del JWT

La politica preparada asume que el JWT de Supabase incluye estos claims dentro de `app_metadata`:

- `company_id`: UUID del tenant al que pertenece el usuario
- `app_role`: uno de `Owner`, `Admin` o `Employee`

Ejemplo:

```json
{
  "app_metadata": {
    "company_id": "4cfd35ae-c59c-4b62-b51e-1911e15d4579",
    "app_role": "Owner"
  }
}
```

## Diseño de seguridad

La politica propuesta sigue estas reglas:

- cada usuario solo puede leer filas de su propia empresa
- `Owner` y `Admin` pueden modificar configuracion de empresa y branding
- `Owner`, `Admin` y `Employee` pueden trabajar con clientes y proformas de su empresa
- tablas internas sensibles no se exponen al cliente:
  - `Users`
  - `Roles`
  - `RefreshTokens`
  - `PasswordResetTokens`
  - `ProformShareTokens`
  - `__EFMigrationsHistory`

## Recomendacion importante

Aunque algun dia uses Supabase Auth, sigue siendo mejor no exponer la tabla `Users` directamente porque contiene `PasswordHash` y otros datos internos. Si necesitas mostrar usuarios al frontend:

- crea una vista o RPC sanitizada
- o sigue usando tu backend .NET como fachada

## Orden recomendado para adopcion futura

1. Añadir `company_id` y `app_role` al JWT de Supabase.
2. Probar la politica en una branch o entorno de staging.
3. Verificar flujos por rol y por tenant.
4. Solo entonces reemplazar la politica actual de bloqueo total.

## Estado recomendado hoy

Mantener la politica actual de bloqueo total para `anon` y `authenticated`.

