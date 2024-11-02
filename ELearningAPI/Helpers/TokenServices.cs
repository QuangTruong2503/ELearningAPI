using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ELearningAPI.Helpers
{
    public class TokenServices
    {
        private readonly string _secretKey;
        public TokenServices(string secretKey)
        {
            _secretKey = secretKey;
        }
        public string GenerateToken(string userID, string role)
        {
            var claims = new[]
            {
            new Claim("userID", userID),
            new Claim("roleID", role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("23f9dc32-e9ee-4f39-b1dd-040a6b69ac21");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,  // Vẫn kiểm tra thời gian hết hạn của token
                ValidateIssuer = false,  // Bỏ qua kiểm tra Issuer
                ValidateAudience = false  // Bỏ qua kiểm tra Audience
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                // Token không hợp lệ
                return null;
            }
        }

    }
}
