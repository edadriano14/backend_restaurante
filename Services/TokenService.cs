using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RedeRestaurante.API.Models;

namespace RedeRestaurante.API.Services;

public static class TokenService
{
    public static string GenerateJwt(Utilizador user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? "MinhaChaveSuperSecretaParaRedeDeRestaurantes2026!#$@123"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("nome", user.Nome),
            new Claim(ClaimTypes.Role, user.Perfil),
            new Claim("estabelecimento_id", user.EstabelecimentoId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"] ?? "RedeRestaurante",
            audience: config["Jwt:Audience"] ?? "RedeRestauranteApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
