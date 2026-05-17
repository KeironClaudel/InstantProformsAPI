# InstantProforms API

Backend para la gestion de proformas con autenticacion segura, branding por empresa, generacion de PDF, envio por correo y enlaces publicos temporales para compartir documentos.

## Overview

InstantProforms API resuelve el flujo completo de una proforma dentro de una misma plataforma:

- onboarding de companias con configuracion inicial y usuario administrador
- autenticacion basada en JWT usando cookies seguras
- administracion de branding, logos y datos fiscales por empresa
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
- Envio de correos con Resend
- Almacenamiento de logos en Supabase Storage
- Proxy autenticado de logos desde el backend
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
- Resend Email API
- Swagger / Swashbuckle

## Architecture

La solucion sigue una estructura por capas:

- `InstantProforms.API`: capa HTTP, controladores, middleware, contratos y seguridad
- `InstantProforms.Application`: casos de uso, comandos, queries y validaciones
- `InstantProforms.Domain`: entidades y reglas centrales del dominio
- `InstantProforms.Infrastructure`: persistencia, JWT, email, PDF y almacenamiento en Supabase

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
- Supabase Storage para logos de empresa
- Cuenta de Resend
- Dominio o remitente verificado en Resend para salida real
- Clave maestra de 32 bytes en Base64 para cifrar secretos por empresa

### Configuracion

La API soporta configuracion desde `appsettings.json` y `UserSecrets`.

Para desarrollo local, se recomienda usar `UserSecrets` para credenciales y valores sensibles:

```bash
dotnet user-secrets init --project InstantProforms.API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=InstantProformsDb;Username=postgres;Password=your-password" --project InstantProforms.API
dotnet user-secrets set "JwtSettings:SecretKey" "REPLACE_WITH_A_SECRET_KEY_OF_AT_LEAST_32_CHARACTERS" --project InstantProforms.API
dotnet user-secrets set "SecretProtectionSettings:MasterKey" "BASE64_ENCODED_32_BYTE_KEY" --project InstantProforms.API
dotnet user-secrets set "ResendSettings:BaseUrl" "https://api.resend.com/" --project InstantProforms.API
dotnet user-secrets set "SupabaseStorage:Url" "https://your-project-ref.supabase.co" --project InstantProforms.API
dotnet user-secrets set "SupabaseStorage:ServiceRoleKey" "your-service-role-key" --project InstantProforms.API
dotnet user-secrets set "SupabaseStorage:BucketName" "logos" --project InstantProforms.API
dotnet user-secrets set "SupabaseStorage:CompanyLogosFolder" "company-logos" --project InstantProforms.API
```

Ajusta como minimo:

- `ConnectionStrings:DefaultConnection`
- `JwtSettings`
- `SecretProtectionSettings:MasterKey`
- `ResendSettings:BaseUrl`
- `SupabaseStorage`
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
- `SecretProtectionSettings:MasterKey`
- `ResendSettings:BaseUrl`
- `SupabaseStorage:Url`
- `SupabaseStorage:ServiceRoleKey`
- `SupabaseStorage:BucketName`
- `SupabaseStorage:CompanyLogosFolder`

### Correos con Resend

La API envia correos mediante `POST /emails` de Resend y mantiene intactos los flujos existentes de:

- recuperacion de contrasena
- envio de proformas con PDF adjunto

La configuracion de Resend ahora es por empresa y se guarda cifrada dentro de `CompanySettings`. La API nunca devuelve el `ApiKey` al frontend; solo expone si ya existe una clave configurada.

Configuracion global minima:

- `SecretProtectionSettings:MasterKey`
- `ResendSettings:BaseUrl`

Configuracion por empresa en `PUT /api/company-settings`:

- `ResendApiKey`
- `ResendSenderEmail`
- `ResendSenderName`
- `ResendReplyToEmail`

Para pruebas rapidas puedes usar un remitente de sandbox como `onboarding@resend.dev`, pero para produccion cada empresa necesita un remitente o dominio valido dentro de su cuenta de Resend.

### Logos de empresa

Los logos se suben a Supabase Storage durante el registro de empresa y al reemplazarlos desde configuracion.

La API guarda en base de datos la ruta relativa del objeto, por ejemplo:

```text
company-logos/{companyId}/{fileName}.png
```

La ruta relativa no debe incluir el nombre del bucket. El bucket se configura por separado en `SupabaseStorage:BucketName`.

Aunque el archivo este en Supabase, el frontend no debe usar directamente una URL publica de Supabase. El endpoint `GET /api/company-settings` devuelve `logoUrl` apuntando al backend:

```text
/api/company-settings/logo?v={storedPath}
```

Luego `GET /api/company-settings/logo` lee el objeto desde Supabase usando `SupabaseStorage:ServiceRoleKey` y devuelve los bytes con el `Content-Type` correcto. Esto evita depender de que el bucket sea publico y mantiene el logo disponible aunque Supabase rechace `/storage/v1/object/public/...`.

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
- `GET /api/company-settings/logo`
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
- Los logos se guardan en Supabase Storage y se sirven al frontend mediante `GET /api/company-settings/logo`
- Si un logo devuelve `400 Bucket not found` desde `/storage/v1/object/public/...`, revisar que el frontend este usando el `logoUrl` del backend y que Render tenga configurado el bucket correcto en `SupabaseStorage:BucketName`
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
