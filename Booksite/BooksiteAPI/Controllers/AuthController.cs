using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooksiteAPI.Data;
using BooksiteAPI.Models.Auth;
using BooksiteAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BooksiteContext _context;
        private readonly IJWTService _jwt;
        private readonly IMailService _mail;

        public AuthController(IJWTService jwtService, BooksiteContext context,
            IMailService mailService)
        {
            _jwt = jwtService;
            _context = context;
            _mail = mailService;
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
                || loginData.Password == null)
            {
                return BadRequest();
            }

            AuthResDto authRes = await _jwt.GetAccessAsync(loginData);
            if (authRes == null || !authRes.IsSuccess)
            {
                return new AuthResDto { IsSuccess = false, Message = "incorrect email/password" };
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

        [HttpPost("signout")]
        public async Task<ActionResult<AuthResDto>>
            Signout()
        {

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

            var sessionsToDelete = await _context.RefreshSessions
                    .Where(rs => rs.RsRefreshToken == refreshSession)
                    .FirstOrDefaultAsync();
            if (sessionsToDelete != null)
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

            return new AuthResDto { IsSuccess = true, Message = "signed out" };
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
                return new AuthResDto { IsSuccess = false, Message = "not found" };
            }

            if (refreshSession.RsExpiresIn < DateTime.UtcNow)
            {
                _context.RefreshSessions.Remove(refreshSession);
                await _context.SaveChangesAsync();
                Response.Cookies.Delete("refresh");
                return new AuthResDto { IsSuccess = false, Message = "expired" };
            }

            AuthResDto authRes = await _jwt.GetRefreshAsync(
                refreshSession.RsUserId, refreshToken, fingerprint);

            if (authRes == null || !authRes.IsSuccess)
            {
                return new AuthResDto { IsSuccess = false, Message = "server error" };
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
            if (_context.RefreshSessions == null
                || _context.Users == null
                || _context.UserTypes == null)
            {
                return NotFound();
            }

            if (regData.FirstName == null
                || regData.Email == null
                || regData.Password == null
                || regData.Fingerprint == null
                || regData.Phone == null)
            {
                return BadRequest();
            }

            //reject if account with this email exists
            var userExists = await _context.Users.Where(
                u => u.UEmail == regData.Email).FirstOrDefaultAsync() != null;
            if (userExists)
            {
                return new AuthResDto
                {
                    IsSuccess = false,
                    Message = "email already used"
                };
            }

            //Add new user and user role to db
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

            //send email with verification link
            var isEmailSentSuccessfully = await _mail
                .SendMailAsync(new Models.Mail.MailReqDto {
                Subject = "Booksite: Confirm your email",
                Body = $"Greetings, {regData.FirstName}. " +
                $"To get verified status please click this link: " +
                $"<a href=\"https://localhost:4200/verify?email={regData.Email}\">verify</a>",
                ToEmail = regData.Email
            });

            if (isEmailSentSuccessfully)
            {
                newUser.M2muutUts.Add(_context.UserTypes
                    .First(ut => ut.UtName == "unverified"));
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                return new AuthResDto
                {
                    IsSuccess = false,
                    Message = "can't send verification email"
                };
            }

            AuthResDto authRes = await _jwt.GetAccessAsync(regData);
            Response.Cookies.Append("refresh", authRes.RefreshToken!,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(30),
                    Path = "/api/auth"
                });
            authRes.RefreshToken = "cookie";
            authRes.Message = "successfully registered";
            return authRes;
        }

        [HttpPost("verify")]
        public async Task<ActionResult<AuthResDto>>
            VerifyEmail(string email)
        {
            if (_context.RefreshSessions == null
                || _context.Users == null
                || _context.UserTypes == null)
            {
                return NotFound();
            }

            if (email == null || !_mail.ValidateEmail(email))
            {
                return new AuthResDto
                {
                    IsSuccess = false,
                    Message = "bad email address"
                };
            }

            email = email.Trim();

            //reject if account with this email doesn't exist
            var user = await _context.Users.Include(ut => ut.M2muutUts)
                .Where(u => u.UEmail == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return new AuthResDto
                {
                    IsSuccess = false,
                    Message = "email not found"
                };
            }

            var unverifiedStatus = user.M2muutUts
                .Where(ut => ut.UtName == "unverified").FirstOrDefault();
            //check if account has unverified status
            if (unverifiedStatus == null)
            {
                return new AuthResDto
                {
                    IsSuccess = false,
                    Message = "user is already verified"
                };
            }

            //Remove unverified and assign verified status
            var verifiedStatus = await _context.UserTypes
                .Where(ut => ut.UtName == "verified").SingleAsync();
            user.M2muutUts.Remove(unverifiedStatus);
            user.M2muutUts.Add(verifiedStatus);
            await _context.SaveChangesAsync();

            return new AuthResDto
            {
                IsSuccess = true,
                Message = $"successfully verified {email}"
            };
        }

        [Authorize(Roles = "admin,verified,unverified")]
        [HttpPost("accessTest")]
        public async Task<ActionResult<string>> AccessTest()
        {
            return await Task.FromResult(User.Claims.First(
                c => c.Type == "role").Value);
        }

        private bool RefreshSessionExists(long id)
        {
            return (_context.RefreshSessions?.Any(e => e.RsId == id)).GetValueOrDefault();
        }
    }
}
