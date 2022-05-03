using EventProjectFinal.DTOs;
using EventProjectFinal.Models;
using EventProjectFinal.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventProjectFinal.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EVENTS3Context _context;
        private readonly IConfiguration _config;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(EVENTS3Context context,
                              IConfiguration configuration,
                              SignInManager<AppUser> signInManager,
                              UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _config = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        
        public async Task<ActionResult> Register([FromBody] Register model)
        {
            Response response = new Response();

            try
            {

                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = "Missing Values.";

                    return BadRequest(response);
                }

                AppUser existsUser = await _userManager.FindByNameAsync(model.Email);

                if (existsUser != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Already exists.";

                    return BadRequest(response);
                }

                if (model.Role == Roles.Admin)
                {
                    response.IsSuccess = false;
                    response.Message = "You cannot register as admin.";

                    return BadRequest(response);
                }


                AppUser user = new AppUser();

                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Role = model.Role;
                user.SurName = model.SurName;


                //Trim removes blanks
                IdentityResult result = await _userManager.CreateAsync(user, model.Password.Trim());


                if (result.Succeeded)

                {
                    if (!await _roleManager.RoleExistsAsync(Roles.Organizator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Roles.Organizator));
                    }
                    if (!await _roleManager.RoleExistsAsync(Roles.Participant))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Roles.Participant));
                    }
                    if (model.Role == "Organizator")
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Organizator);
                    }

                    if (model.Role == "Participant")
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Participant);
                    }


                    response.IsSuccess = true;
                    response.Message = "User has been created successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("An error occured: {0}", result.Errors.FirstOrDefault().Description);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

                return BadRequest(response);
            }
        }

        [HttpPost]
        
        public async Task<ActionResult> Login([FromBody] Login model)
        {
            Response response = new Response();

            try
            {
                

                if (ModelState.IsValid == false)
                {
                    response.IsSuccess = false;
                    response.Message = "Missing Values.";
                    return BadRequest(response);
                }


                AppUser user = await _userManager.FindByNameAsync(model.Email);


                if (user == null)
                {
                    return Unauthorized();
                }

                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user,
                                                                                                                   model.Password,
                                                                                                                   false,
                                                                                                                   false);

                if (signInResult.Succeeded == false)
                {
                    response.IsSuccess = false;
                    response.Message = "Username or password is wrong.";

                    return Unauthorized(response);
                }


                AppUser appUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);

                AccessTokenGenerator accessTokenGenerator = new AccessTokenGenerator(_context, _config, appUser);
                AppUserTokens userTokens = accessTokenGenerator.GetToken();

                response.IsSuccess = true;
                response.Message = "User logged in " + "with User ID: " + appUser.Id;
                
                response.TokenInfo = new TokenINFO
                {
                    Token = userTokens.Value,
                    ExpireDate = userTokens.ExpireDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

                return BadRequest(response);
            }
        }
         
        
       

        [HttpPost]
        
        public async Task<ActionResult> RegisterCompany([FromBody] CompanyRegister model)
        {
            Response response = new Response();

            try
            {

                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = "Missing Values.";

                    return BadRequest(response);
                }

                AppUser existsUser = await _userManager.FindByNameAsync(model.Email);

                if (existsUser != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Already exists.";

                    return BadRequest(response);
                }

                if (model.Role == Roles.Admin)
                {
                    response.IsSuccess = false;
                    response.Message = "You cannot register as admin.";

                    return BadRequest(response);
                }


                AppUser user = new AppUser();


                user.Name = ".";
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Role = model.Role;
                user.SurName = ".";




                IdentityResult result = await _userManager.CreateAsync(user, model.Password.Trim());

                if (!await _roleManager.RoleExistsAsync(Roles.Company))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.Company));
                }

                if (model.Role == "Company")
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Company);
                    }

                if (result.Succeeded)

                {
                    response.IsSuccess = true;
                    response.Message = "User has been created successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("An error occured: {0}", result.Errors.FirstOrDefault().Description);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

                return BadRequest(response);
            }
        }

    }
}
