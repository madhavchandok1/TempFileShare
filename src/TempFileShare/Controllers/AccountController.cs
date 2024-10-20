using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TempFileShare.Contracts.DataTransferObjects.Accounts;
using TempFileShare.Contracts.Interfaces.Utilities;
using TempFileShare.Contracts.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TempFileShare.Controllers
{
    [ApiController]
    [Route("/accounts")]
    public class AccountController(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService) : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;


        //POST: /account/register
        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ApplicationUser appUser = new()
                {
                    UserName = register.Username.ToLower(),
                    Email = register.Email,
                };

                IdentityResult createdUser = await _userManager.CreateAsync(appUser, register.Password);

                if (createdUser.Succeeded)
                {
                    IdentityResult roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUser
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        //POST: /account/login
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(row => row.UserName == login.UserName.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username!!! Your username is not registered. Please register it and try again");
            }

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Username not found and/or password is incorrect");
            }

            return Ok(new NewUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            });
        }
    }
}

