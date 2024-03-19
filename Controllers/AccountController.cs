using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DataAccess.Blog.DataRequests;
using DataAccess.Blog.Entities;
using DataAccess.Blog.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogHomeworkWebAPI.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IBlogUnitOfWork _blogUnitOfwork;
        private IConfiguration _configuration;

        private class UserResponse
        {
            public string username { get; set; }
            public string token { get; set; }
            public string refresh_token { get; set; }
        }

        public AccountController(IBlogUnitOfWork blogUnitOfwork, IConfiguration configuration)
        {
            _blogUnitOfwork = blogUnitOfwork;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginRequestData requestData)
        {
            try
            {
                if (requestData == null || requestData.username == null || requestData.password == null)
                {
                    return BadRequest();
                }

                var user = await _blogUnitOfwork._userRepository.Login(requestData);

                if (user == null || user.id == null)
                {
                    return Ok("User ko ton tai!");
                }

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                };

                var newAccessToken = CreateToken(authClaims);

                var token = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
                var refreshToken = GenerateRefreshToken();
                
                var expriredDateSettingDay = _configuration["JWT:RefreshTokenValidityInDays"] ?? "";
                var userNewRefreshToken = new UserUpdateRefeshTokenRequestData
                {
                    id = user.id,
                    refresh_token  = refreshToken,
                    refresh_token_expired_date= DateTime.Now.AddDays(Convert.ToInt32(expriredDateSettingDay))
                };
                
                var update = await _blogUnitOfwork._userRepository.UpdateRefeshToken(userNewRefreshToken);

                var response = new UserResponse();
                response.username = user.username;
                response.token = token;
                response.refresh_token = refreshToken;

                return Ok(response);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}