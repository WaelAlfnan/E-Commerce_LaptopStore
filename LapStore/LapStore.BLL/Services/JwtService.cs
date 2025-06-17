using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using LapStore.DAL.Data.Entities;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LapStore.BLL.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimService _userClaimService;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _expirationInDays;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;

        public JwtService(
            IConfiguration configuration,
            ILogger<JwtService> logger,
            UserManager<User> userManager,
            IUserClaimService userClaimService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userClaimService = userClaimService ?? throw new ArgumentNullException(nameof(userClaimService));
            
            _secretKey = _configuration["JwtSettings:JWT_SECRET_KEY"] ?? throw new ArgumentNullException("JwtSettings:JWT_SECRET_KEY");
            _issuer = _configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
            _audience = _configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience");
            _expirationInDays = double.Parse(_configuration["JwtSettings:ExpirationInDays"] ?? "2");

            // Validate secret key length
            if (Encoding.UTF8.GetBytes(_secretKey).Length < 32)
            {
                throw new ArgumentException("Secret key must be at least 32 bytes (256 bits) long");
            }

            // Initialize security key and credentials once
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var tokenId = Guid.NewGuid().ToString();
                var issuedAt = DateTime.UtcNow;
                var expires = issuedAt.AddDays(_expirationInDays);

                // Prepare standard user claims
                var standardClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                };

                // Get and add role claims
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    standardClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Ensure user has all necessary claims
                var claimResult = await _userClaimService.AddClaimsAsync(user.Id, standardClaims);
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to ensure claims for user {UserId}: {Message}", 
                        user.Id, claimResult.Message);
                }

                // Get all user claims
                var userClaims = await _userManager.GetClaimsAsync(user);
                if (!userClaims.Any())
                {
                    _logger.LogWarning("No claims found for user {UserId}, using prepared claims", user.Id);
                    userClaims = standardClaims;
                }

                // Add JWT-specific claims
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString())
                };

                // Add all user claims
                claims.AddRange(userClaims);

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    notBefore: issuedAt,
                    expires: expires,
                    signingCredentials: _signingCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("JWT token generated successfully for user {UserId} with {ClaimCount} claims", 
                    user.Id, claims.Count);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
                throw;
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                _logger.LogInformation("JWT token validated successfully");
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed");
                return null;
            }
        }
    }
} 