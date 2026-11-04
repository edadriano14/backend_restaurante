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
        // 2. Se não estiver no header, tenta obter da Claim do JWT autenticado
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("estabelecimento_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantIdFromClaim))
            {
                tenantService.EstabelecimentoId = tenantIdFromClaim;
            }
        }

        await _next(context);
    }
}
