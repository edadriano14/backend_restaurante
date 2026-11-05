using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RedeRestaurante.API.Data;
using RedeRestaurante.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Carregar variáveis do ficheiro .env local (se existir) para conectar ao Supabase e Frontend
var envFilePath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (File.Exists(envFilePath))
{
    foreach (var line in File.ReadAllLines(envFilePath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
        var idx = trimmed.IndexOf('=');
        if (idx <= 0) continue;
        var key = trimmed[..idx].Trim();
        var val = trimmed[(idx + 1)..].Trim().Trim('"', '\'');
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, val);
        }
    }
    builder.Configuration.AddEnvironmentVariables();
}

// Adicionar Controladores e CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var frontendOrigins = (builder.Configuration["Frontend:Url"] ?? Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});

// Injeção de Dependência do Tenant
builder.Services.AddScoped<ITenantService, TenantService>();

// Conexão PostgreSQL (Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("SUPABASE_DB_CONNECTION")
    ?? "Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.zkabrymvsdvvaprvhiac;Password=MnQzdnQybjJqZXBpbHk1em5lNm9saHY3b29maGd5ZzM;SSL Mode=Require;Trust Server Certificate=true;Pooling=true";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "MinhaChaveSuperSecretaParaRedeDeRestaurantes2026!#$@123";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// Habilitar Swagger em todos os ambientes (incluindo Produção no Fly.io)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rede de Restaurantes Multi-Tenant API v1");
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();

// Rotas de Raiz e Healthcheck (evita HTTP 404 na raiz https://backend-restaurante.fly.dev/)
app.MapGet("/", () => Results.Ok(new
{
    status = "online",
    servico = "Rede de Restaurantes Multi-Tenant API (.NET 8)",
    supabaseUrl = builder.Configuration["Supabase:Url"] ?? Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "https://zkabrymvsdvvaprvhiac.supabase.co",
    frontendUrl = builder.Configuration["Frontend:Url"] ?? Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "https://frontend-restaurante.fly.dev",
    documentacao = "/swagger",
    health = "/health"
}));

app.MapGet("/health", async (AppDbContext db) =>
{
    bool dbConnected = false;
    try
    {
        dbConnected = await db.Database.CanConnectAsync();
    }
    catch
    {
        dbConnected = false;
    }
    return Results.Ok(new
    {
        status = "healthy",
        databaseConnected = dbConnected,
        supabaseProject = "zkabrymvsdvvaprvhiac",
        timestamp = DateTime.UtcNow
    });
});

app.MapControllers();

app.Run();
