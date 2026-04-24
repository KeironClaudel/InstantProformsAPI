# InstantProforms API

Backend para la gestion de proformas con autenticacion segura, branding por empresa, generacion de PDF, envio por correo y enlaces publicos temporales para compartir documentos.

## Overview

InstantProforms API resuelve el flujo completo de una proforma dentro de una misma plataforma:

- onboarding de companias con configuracion inicial y usuario administrador
- autenticacion basada en JWT usando cookies seguras
- administracion de branding y datos fiscales por empresa
- creacion, consulta y actualizacion de proformas
- listado paginado con estado, impuestos y datos del cliente
- generacion de PDF para descarga o envio por correo
- enlaces temporales para compartir proformas de forma publica

## Features

- Autenticacion con `accessToken` y `refreshToken` en cookies `HttpOnly`
- Proteccion CSRF para operaciones autenticadas
- Rate limiting global y politicas estrictas para autenticacion
- Validacion de requests con FluentValidation
- Arquitectura por capas con separacion de responsabilidades
- Persistencia con PostgreSQL y Entity Framework Core
- Generacion de PDF con QuestPDF
- Envio de correos con MailKit
- Swagger en ambiente de desarrollo
- CORS configurado para integracion local con frontend en `localhost:5173`
- Soporte para `UserSecrets` en desarrollo

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL
- MediatR
- FluentValidation
- JWT Bearer Authentication
- QuestPDF
- MailKit
- Swagger / Swashbuckle

## Architecture

La solucion sigue una estructura por capas:

- `InstantProforms.API`: capa HTTP, controladores, middleware, contratos y seguridad
- `InstantProforms.Application`: casos de uso, comandos, queries y validaciones
- `InstantProforms.Domain`: entidades y reglas centrales del dominio
- `InstantProforms.Infrastructure`: persistencia, JWT, email, PDF y almacenamiento local

```text
InstantProforms/
|- InstantProforms.API/
|- InstantProforms.Application/
|- InstantProforms.Domain/
|- InstantProforms.Infrastructure/
`- InstantProforms.slnx
```

## Quick Start

### Prerrequisitos

- .NET 8 SDK
- PostgreSQL
- Credenciales SMTP validas si se usaran correos

### Configuracion

La API soporta configuracion desde `appsettings.json` y `UserSecrets`.

Para desarrollo local, se recomienda usar `UserSecrets` para credenciales y valores sensibles:

```bash
dotnet user-secrets init --project InstantProforms.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=InstantProformsDb;Username=postgres;Password=your-password" --project InstantProforms.API
dotnet user-secrets set "JwtSettings:SecretKey" "REPLACE_WITH_A_SECRET_KEY_OF_AT_LEAST_32_CHARACTERS" --project InstantProforms.API
dotnet user-secrets set "SmtpSettings:Host" "smtp.gmail.com" --project InstantProforms.API
dotnet user-secrets set "SmtpSettings:Port" "465" --project InstantProforms.API
dotnet user-secrets set "SmtpSettings:SenderEmail" "your-email@example.com" --project InstantProforms.API
dotnet user-secrets set "SmtpSettings:Username" "your-email@example.com" --project InstantProforms.API
dotnet user-secrets set "SmtpSettings:Password" "your-app-password" --project InstantProforms.API
```

Ajusta como minimo:

- `ConnectionStrings:DefaultConnection`
- `JwtSettings`
- `SmtpSettings`
- `PasswordResetSettings`
- `ProformShareSettings`

Ejemplo base:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=InstantProformsDb;Username=postgres;Password=your-password"
  },
  "JwtSettings": {
    "Issuer": "InstantProforms",
    "Audience": "InstantProformsClient",
    "SecretKey": "REPLACE_WITH_A_SECRET_KEY_OF_AT_LEAST_32_CHARACTERS",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

Configuracion relevante adicional:

- `PasswordResetSettings:ResetUrl`
- `ProformShareSettings:PublicDownloadUrl`

### Ejecutar localmente

```bash
dotnet restore
dotnet ef database update --project InstantProforms.Infrastructure --startup-project InstantProforms.API
dotnet run --project InstantProforms.API
```

URLs locales configuradas:

- `http://localhost:5020`
- `https://localhost:7210`

Swagger en desarrollo:

- `https://localhost:7210/swagger`

Frontend local permitido por CORS:

- `http://localhost:5173`
- `https://localhost:5173`

## Main Endpoints

### Auth

- `POST /api/auth/register-company`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`

### Security

- `GET /api/security/csrf-token`

### Company Settings

- `GET /api/company-settings`
- `PUT /api/company-settings`
- `PUT /api/company-settings/logo`

### Proforms

- `POST /api/proforms`
- `GET /api/proforms`
- `GET /api/proforms/{id}`
- `PATCH /api/proforms/{id}/status`
- `GET /api/proforms/{id}/pdf`
- `POST /api/proforms/{id}/send-email`
- `POST /api/proforms/{id}/share-link`
- `GET /api/proforms/{id}/share-links`
- `DELETE /api/proforms/{id}/share-links/{shareTokenId}`

### Public Access

- `GET /api/public/proforms/download?token={token}`

## Security

- JWT Bearer con lectura del token desde cookie `accessToken`
- Refresh token en cookie `refreshToken`
- Logout idempotente via `POST /api/auth/logout` aunque no haya sesion autenticada activa
- Cookie/header CSRF:
  - cookie `XSRF-TOKEN`
  - header `X-CSRF-TOKEN`
- Politica global de autenticacion por defecto
- Rate limit por IP y politicas especiales para endpoints sensibles
- Limpieza automatica de cookie CSRF legacy de `/api/auth`

## Notes

- Swagger solo se habilita en `Development`
- La API crea automaticamente `wwwroot/uploads` si no existe
- Los logos y archivos publicos se sirven desde `InstantProforms.API/wwwroot`
- Los enlaces publicos dependen de `ProformShareSettings:PublicDownloadUrl`
- La recuperacion de contrasena depende de `PasswordResetSettings:ResetUrl`
- El frontend local permitido por defecto es `localhost:5173`

## Project Status

Proyecto funcional para:

- autenticacion y sesiones
- configuracion de empresa
- gestion de proformas
- actualizacion manual de estado de proformas
- descarga de PDF
- envio por correo
- enlaces publicos temporales

## License

Pendiente de definir.
