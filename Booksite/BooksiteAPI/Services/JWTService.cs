using BooksiteAPI.Models.Auth;
using BooksiteAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace BooksiteAPI.Services
{
    public class JWTService : IJWTService
    {
        private readonly BooksiteContext _context;
        private readonly IConfiguration _config;

        public JWTService(BooksiteContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResDto> GetAccessAsync(AuthReqDto req)
        {
            var user = _context.Users.FirstOrDefault(
                u => u.UEmail.Equals(req.Email)
                && u.UPassword.Equals(req.Password));
            if (user is null)
                return await Task.FromResult<AuthResDto>(
                    new AuthResDto
                    {
                        IsSuccess = false,
                        Message = "invalid_creds"
                    });
            var userRole = _context.UserTypes
                .Where(ut => ut.M2muutUs.Contains(user)).SingleOrDefault();
            if (userRole is null)
                return await Task.FromResult<AuthResDto>(
                    new AuthResDto
                    {
                        IsSuccess = false,
                        Message = "no_role_for_user"
                    });
            string accessToken = GenerateToken(req, userRole.UtName);
            string refreshToken = GenerateRefreshToken();
            return await SaveTokenDetails(user, req.Fingerprint!,
                accessToken, refreshToken);
        }
        public async Task<AuthResDto> GetRefreshAsync(
            int userId, Guid oldRefresh, string fingerprint)
        {
            var user = _context.Users.FirstOrDefault(
                u => u.UId == userId);
            var userRole = _context.UserTypes
                .Where(ut => ut.M2muutUs.Contains(user!)).SingleOrDefault();
            string accessToken = GenerateToken(
                new AuthReqDto { Email = user!.UEmail, Fingerprint = fingerprint },
                userRole!.UtName);
            string newRefresh = GenerateRefreshToken();
            return await SaveTokenDetails(user!, fingerprint,
                accessToken, newRefresh);
        }

        public async Task<bool> ValidateAccessAsync(JwtSecurityToken token)
        {
            try
            {
                bool isValid = false;
                string fingerprint = token.Claims.
                    First(x => x.Type == "fingerprint").Value;
                if (fingerprint != null)
                {
                    isValid = _context.RefreshSessions.FirstOrDefault(
                        rs => rs.RsFingerprint == fingerprint) != null;
                }
                return await Task.FromResult(isValid);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        private string GenerateToken(AuthReqDto req, string role)
        {
            var jwtKeyBytes = Encoding.ASCII.GetBytes(_config["JWT:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Audience = _config["JWT:ValidAudience"],
                Issuer = _config["JWT:ValidIssuers:0"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, req.Email!),
                    new Claim(ClaimTypes.Role, role!),
                    new Claim("fingerprint", req.Fingerprint!),

                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(descriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task<AuthResDto> SaveTokenDetails(
            User user, string fingerprint, string access, string refresh)
        {
            //Check if refresh session with given user id and fingerprint exists
            var activeSessions = _context.RefreshSessions
                .Where(rs => rs.RsUserId == user.UId).ToList();
            var identicalSession = activeSessions
                .Where(rs => rs.RsFingerprint == fingerprint).FirstOrDefault();

            //Identical session exists, update refresh and expiry time
            if (identicalSession != null)
            {
                identicalSession.RsRefreshToken = Guid.Parse(refresh);
                identicalSession.RsExpiresIn = DateTime.UtcNow.AddDays(30);
            }
            else
            {
                //check if user already has 5 sessions
                if (activeSessions.Count >= 5)
                {
                    _context.RemoveRange(activeSessions);
                }
                _context.Add(new RefreshSession
                {
                    RsUserId = user.UId,
                    RsFingerprint = fingerprint,
                    RsRefreshToken = Guid.Parse(refresh),
                    RsCreatedAt = DateTime.UtcNow,
                    RsExpiresIn = DateTime.UtcNow.AddDays(30),
                });
            }

            int res = await _context.SaveChangesAsync();
            return new AuthResDto
            {
                IsSuccess = true,
                Message = "success",
                AccessToken = access,
                RefreshToken = refresh
            };
        }
    }
}
