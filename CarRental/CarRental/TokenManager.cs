using CarRental.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental
{
    public class TokenManager
    {
        // Secret key for token generation
        public static string Secret = "qwerwerqwersadfasdfsdfsadfsdfsadfasdfasdfsafd2342142341234";

        public static string GenerateToken(string email, string role)
        {
            // Create a security key from the secret key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

            // Create a token descriptor with claims, expiration time, and signing credentials
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(8), // Token expiration time
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            // Create a JWT token
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            // Write the token as a string and return it
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null)
                {
                    return null;
                }

                TokenValidationParameters parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                };

                // Validate the token with the parameters
                SecurityToken validatedToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static TokenClaim ValidateToken(string RawToken)
        {
            string[] array = RawToken.Split(' ');
            var token = array[1];
            ClaimsPrincipal principal = GetPrincipal(token);

            if (principal == null)
                return null;

            ClaimsIdentity identity = null;

            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (Exception ex)
            {
                return null;
            }

            TokenClaim tokenClaim = new TokenClaim();
            var temp = identity.FindFirst(ClaimTypes.Email);
            tokenClaim.Email = temp.Value;
            temp = identity.FindFirst(ClaimTypes.Role);
            tokenClaim.Role = temp.Value;

            return tokenClaim;
        }


    }



}
