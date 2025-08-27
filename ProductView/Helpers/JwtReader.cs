using System.IdentityModel.Tokens.Jwt;

namespace WebClient.Helpers;

public static class JwtReader
{
    public static DateTimeOffset? GetExpiry(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Lấy claim exp
        var exp = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (long.TryParse(exp, out var seconds))
            return DateTimeOffset.FromUnixTimeSeconds(seconds);

        return null;
    }
}
