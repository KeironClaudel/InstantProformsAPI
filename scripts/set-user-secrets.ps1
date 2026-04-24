param(
    [string]$ProjectPath = "C:\Users\Keiron\source\repos\InstantProforms\InstantProforms.API\InstantProforms.API.csproj",
    [switch]$Preview
)

$ErrorActionPreference = "Stop"

# Edit these values before running the script.
$settings = @{
    "ConnectionStrings:DefaultConnection" = "Host=localhost;Port=5432;Database=InstantProformsDb;Username=postgres;Password=1234"

    "JwtSettings:Issuer" = "InstantProforms"
    "JwtSettings:Audience" = "InstantProformsClient"
    "JwtSettings:SecretKey" = "TU_SECRET_LARGO_Y_SEGURO"
    "JwtSettings:AccessTokenExpirationMinutes" = "15"
    "JwtSettings:RefreshTokenExpirationDays" = "7"

    "SmtpSettings:Host" = "smtp.gmail.com"
    "SmtpSettings:Port" = "465"
    "SmtpSettings:SenderName" = "InstantProforms"
    "SmtpSettings:SenderEmail" = "tu-correo@dominio.com"
    "SmtpSettings:Username" = "tu-correo@dominio.com"
    "SmtpSettings:Password" = "tu-app-password"
    "SmtpSettings:UseSsl" = "true"

    "PasswordResetSettings:ResetUrl" = "http://localhost:5173/reset-password"
    "PasswordResetSettings:TokenExpirationMinutes" = "30"

    "ProformShareSettings:PublicDownloadUrl" = "https://localhost:7210/api/public/proforms/download"
    "ProformShareSettings:DefaultExpirationMinutes" = "60"

    "SupabaseStorage:Url" = "https://tu-proyecto.supabase.co"
    "SupabaseStorage:ServiceRoleKey" = "tu-supabase-service-role-key"
    "SupabaseStorage:BucketName" = "company-assets"
    "SupabaseStorage:CompanyLogosFolder" = "uploads/company-logos"
}

if (-not (Test-Path -LiteralPath $ProjectPath)) {
    throw "Project file not found: $ProjectPath"
}

foreach ($entry in $settings.GetEnumerator() | Sort-Object Key) {
    $key = $entry.Key
    $value = $entry.Value

    if ($Preview) {
        Write-Host "dotnet user-secrets set `"$key`" `"$value`" --project `"$ProjectPath`""
        continue
    }

    Write-Host "Setting secret: $key"
    dotnet user-secrets set $key $value --project $ProjectPath | Out-Null
}

if ($Preview) {
    Write-Host ""
    Write-Host "Preview mode only. No secrets were written."
}
else {
    Write-Host ""
    Write-Host "User secrets updated successfully."
}
