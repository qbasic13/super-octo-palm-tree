using System.Security.Claims;
using BooksiteAPI.Data;
using BooksiteAPI.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly BooksiteContext _context;

        public ProfileController(BooksiteContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "unverified,verified,admin")]
        [HttpGet]
        public async Task<ProfileResDto> GetProfileDetails()
        {
            var userEmailClaim = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null)
                return new ProfileResDto()
                {
                    IsSuccess = false,
                    Status = "unauthorized",
                    Message = "Unauthorized"
                };

            var userData = await _context.Users
                .Include(ut => ut.M2muutUts)
                .Where(u => u.UEmail == userEmailClaim.Value)
                .SingleOrDefaultAsync();
           
            if (userData == null)
                return new ProfileResDto()
                {
                    IsSuccess = false,
                    Status = "not_found",
                    Message = "User not found"
                };

            ProfileDto profile = new ProfileDto()
            {
                Id = userData.UId,
                Email = userData.UEmail,
                FirstName = userData.UFirstName,
                LastName = userData.ULastName,
                MiddleName = userData.UMiddleName,
                Phone = userData.UPhone,
                AccountType = userData.M2muutUts.Count > 0 
                    ? userData.M2muutUts.First().UtName : null,
                RegisterDate = userData.URegisterDt
            };

            return new ProfileResDto()
            {
                IsSuccess = true,
                Status = "success",
                Message = "Got profile successfully",
                Profile = profile
            };
        }
    }
}
