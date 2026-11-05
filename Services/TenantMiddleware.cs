using System.Security.Claims;

namespace RedeRestaurante.API.Services;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate _next)
    {
        this._next = _next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // 1. Tenta obter do Header HTTP X-Tenant-Id
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader) &&
            Guid.TryParse(tenantHeader, out var tenantIdFromHeader))
        {
            tenantService.EstabelecimentoId = tenantIdFromHeader;
        }
        // 2. Se não estiver no header, tenta obter da Claim do JWT autenticado (direto ou dentro de app_metadata do Supabase)
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("estabelecimento_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantIdFromClaim))
            {
                tenantService.EstabelecimentoId = tenantIdFromClaim;
            }
            else
            {
                var appMetaJson = context.User.FindFirst("app_metadata")?.Value;
                if (!string.IsNullOrEmpty(appMetaJson))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(appMetaJson);
                        if (doc.RootElement.TryGetProperty("estabelecimento_id", out var prop) &&
                            Guid.TryParse(prop.GetString(), out var tenantFromMeta))
                        {
                            tenantService.EstabelecimentoId = tenantFromMeta;
                        }
                    }
                    catch { }
                }
            }
        }

        await _next(context);
    }
}
