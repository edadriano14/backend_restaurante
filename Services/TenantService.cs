namespace RedeRestaurante.API.Services;

public interface ITenantService
{
    Guid? EstabelecimentoId { get; set; }
}

public class TenantService : ITenantService
{
    public Guid? EstabelecimentoId { get; set; }
}
