using BooksiteAPI.Models.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace BooksiteAPI.Services
{
    public interface IJWTService
    {
        Task<AuthResDto> GetAccessAsync(AuthReqDto reqDto);
        Task<AuthResDto> GetRefreshAsync(int userId, Guid oldRefresh, string fingerprint);
        Task<bool> ValidateAccessAsync(JwtSecurityToken token);
    }
}
