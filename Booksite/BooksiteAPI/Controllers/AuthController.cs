using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Data;
using BooksiteAPI.Models.Auth;
using BooksiteAPI.Services;

namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BooksiteContext _context;
		private readonly IJWTService _jwtService;

		public AuthController(IJWTService jwtService, BooksiteContext context)
        {
			_jwtService = jwtService;
            _context = context;
        }

		[HttpPost("signin")]
        public async Task<ActionResult<AuthResDto>> 
			Signin(AuthReqDto loginData)
		{
			if (_context.RefreshSessions == null)
			{
				return NotFound();
			}

			if (loginData.Fingerprint == null || loginData.Email == null
				|| loginData.Password == null) {
				return BadRequest();
			}

			AuthResDto authRes = await _jwtService.GetAccessAsync(loginData);
			if (authRes == null || !authRes.IsSuccess) {
				return new AuthResDto { IsSuccess = false, Message = "incorrect email/password" };
			}

			Response.Cookies.Append("refresh",authRes.RefreshToken!, 
				new CookieOptions {
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(30),
				Path = "/api/auth"
				});
			authRes.RefreshToken = "cookie";
			return authRes;
        }

		[HttpPost("signout")]
		public async Task<ActionResult<AuthResDto>>
			Signout() {
			
			if (_context.RefreshSessions == null)
			{
				return NotFound();
			}

			//check if RefreshSession exists
			Guid refreshSession;
			bool parseRes = Guid.TryParse(Request.Cookies
				.SingleOrDefault(c => c.Key == "refresh").Value,
				out refreshSession);
			if (!parseRes)
			{
				return BadRequest();
			}

			var sessionsToDelete =  await _context.RefreshSessions
					.Where(rs => rs.RsRefreshToken == refreshSession)
					.FirstOrDefaultAsync();
			if(sessionsToDelete != null)
			{
				_context.RefreshSessions.Remove(sessionsToDelete);
				await _context.SaveChangesAsync();
			}
			Response.Cookies.Append("refresh", "", new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.Now.AddDays(-1),
				Path = "/api/auth"
			});

			return new AuthResDto {IsSuccess = true, Message = "signed out" };
		}

		[HttpPost("refresh")]
		public async Task<ActionResult<AuthResDto>>
		RefreshToken(string fingerprint)
		{
			if (_context.RefreshSessions == null)
			{
				return NotFound();
			}

			Guid refreshToken;
			bool parseRes = Guid.TryParse(Request.Cookies
				.SingleOrDefault(c => c.Key == "refresh").Value,
				out refreshToken);
			if (!parseRes)
			{
				return BadRequest();
			}

			var refreshSession = await _context.RefreshSessions.Where(
				rs => rs.RsRefreshToken == refreshToken 
				&& rs.RsFingerprint == fingerprint).FirstOrDefaultAsync();
			
			if (refreshSession is null)
			{
				return new AuthResDto { IsSuccess = false, Message = "not_found" };
			}

			if (refreshSession.RsExpiresIn < DateTime.UtcNow) {
				_context.RefreshSessions.Remove(refreshSession);
				await _context.SaveChangesAsync();
				Response.Cookies.Delete("refresh");
				return new AuthResDto { IsSuccess = false, Message = "expired" };
			}

			AuthResDto authRes =  await _jwtService.GetRefreshAsync(
				refreshSession.RsUserId,refreshToken,fingerprint);

			if (authRes == null || !authRes.IsSuccess)
			{
				return new AuthResDto { IsSuccess = false, Message = "server_error" };
			}

			Response.Cookies.Append("refresh", authRes.RefreshToken!,
				new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTime.UtcNow.AddDays(30),
					Path = "/api/auth"
				});
			authRes.RefreshToken = "cookie";
			return authRes;
		}

		[HttpPost("signup")]
		public async Task<ActionResult<AuthResDto>>
			Signup(RegisterDto regData)
		{
			if(_context.RefreshSessions == null
				||_context.Users == null 
				||_context.UserTypes == null)
			{
				return NotFound();
			}

			if(regData.FirstName == null
				|| regData.Email == null
				|| regData.Password == null 
				|| regData.Fingerprint == null
				|| regData.Phone == null)
			{
				return BadRequest();
			}

			//regect if account with this email exists
			var userExists = await _context.Users.Where(
				u => u.UEmail == regData.Email).FirstOrDefaultAsync() != null;
			if(userExists)
			{
				return new AuthResDto { IsSuccess = false, 
					Message = "email already used" };
			}

			User newUser = new()
			{
				UEmail = regData.Email,
				UPassword = regData.Password,
				UFirstName = regData.FirstName,
				ULastName = regData.LastName,
				UMiddleName = regData.MiddleName,
				UPhone = regData.Phone,
				URegisterDt = DateTime.Now
			};
			newUser.M2muutUts.Add(_context.UserTypes
				.First(ut => ut.UtName == "unverified"));
			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();

			AuthResDto authRes = await _jwtService.GetAccessAsync(regData);
			Response.Cookies.Append("refresh", authRes.RefreshToken!,
				new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTime.UtcNow.AddDays(30),
					Path = "/api/auth"
				});
			authRes.RefreshToken = "cookie";
			authRes.Message = "successfuly registered";
			return authRes;
		}

		private bool RefreshSessionExists(long id)
        {
            return (_context.RefreshSessions?.Any(e => e.RsId == id)).GetValueOrDefault();
        }
    }
}
