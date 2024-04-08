using Microsoft.AspNetCore.Mvc;
using LibMgt.LibDbContext;
using LibMgt.Models;
using Microsoft.AspNetCore.Identity;
using LibMgt.FrontEndRequests;
using LibMgt.Services;
using NuGet.Common;
using System.Security.Policy;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace LibMgt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ValidationService _validationService;
        private readonly EmailService _emailService;
        private readonly SignInManager<User> _signinManager;
        private readonly JWTService _jwtService;
        private readonly ILogger<AuthController> _logger;
        private readonly RoleManager<Role> _role;
        public AuthController(ILogger<AuthController> logger,RoleManager<Role> role, LibraryDbContext context,UserManager<User> userManager,ValidationService validationService,EmailService emailService,SignInManager<User> signInManager,JWTService jwtService)
        { 
            _context = context;
            _userManager = userManager;
            _validationService = validationService;
            _emailService=emailService;
            _signinManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
            _role = role;
        }


        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost("createRoles")]
        public async Task<IActionResult> createRoles()
        {
            try
            {
                var role = await _role.FindByNameAsync("Admin");
                if (role == null)
                {
                    Role nrole = new Role()
                    {
                        RoleType = 0,
                        CreationTime = DateTime.UtcNow,
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        IsDeleted = false,


                    };
                    await _role.CreateAsync(nrole);
                    _logger.LogInformation("Admin role created" + DateTime.UtcNow.ToString());
                }
                role = await _role.FindByNameAsync("User");
                if (role == null)
                {
                    Role nrole = new Role()
                    {
                        RoleType = 1,
                        CreationTime = DateTime.UtcNow,
                        Name = "User",
                        NormalizedName = "USER",
                        IsDeleted = false,


                    };
                    await _role.CreateAsync(nrole);
                    _logger.LogInformation("User role created" + DateTime.UtcNow.ToString());
                }

                return Ok(new
                {
                    roles = _role.Roles.Where(x => x.IsDeleted == false).ToList(),
                });
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpPost("createAdmin")]
        public async Task<IActionResult> createAdmin()
        {
            try
            {
                var role = await _role.FindByNameAsync("Admin");
                if (role == null)
                {
                   
                    _logger.LogInformation("Admin role not found" + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = ("Admin role not found")
                    });
                }
                var _user = new User
                {
                    Email = "nayarw1933879@gmail.com",
                    UserName = "Nayar",
                    CreationTime = DateTime.UtcNow,
                    IsDeleted = false,
                    EmailConfirmed = true,
                    NormalizedEmail = "nayarw1933879@gmail.com".ToLower(),
                    NormalizedUserName = "Nayar".ToUpper(),
                };

                _logger.LogInformation("Admin User role created" + DateTime.UtcNow.ToString());
                var result = await _userManager.CreateAsync(_user);

                if (result.Succeeded)
                {
                    var IsPasswordSet = await _userManager.AddPasswordAsync(_user, "Nayar@123");
                    await _userManager.AddToRoleAsync(_user, "Admin");
                    var roles = await _userManager.GetRolesAsync(_user);
                    return Ok(new
                    {
                        user = new
                        {
                            Id = _user.Id,
                            UserName = _user.UserName,
                            Email = _user.Email,
                            CreationTIme = _user.CreationTime,
                            EmailConfirmed = _user.EmailConfirmed,
                            role = role,
                            Token=_jwtService.GenerateJWT(_user, roles)
                        },
                    });
                }
                else
                {
                    await _userManager.DeleteAsync(_user);
                    var err = "";
                    foreach (var e in result.Errors)
                    {
                        err += e.Description + " ";
                    }
                    _logger.LogError(err + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = err
                    });
                }

                  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Create(SignupRequest user)
        {

            try
            {
                _validationService.ValidateSignupRequest(user);
                var role = await _role.FindByNameAsync("User");
                if (role == null)
                {

                    _logger.LogInformation("User role not found" + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = ("User role not found")
                    });
                }
                var exists = await _userManager.FindByEmailAsync(user.Email);
                if (exists != null)
                {
                    _logger.LogError("Email already in user" + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = "Email already in use"
                    });
                }
                else
                {
                    var _user = new User
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        CreationTime = DateTime.UtcNow,
                        IsDeleted = false,
                        EmailConfirmed = false,
                        NormalizedEmail = user.Email.ToLower(),
                        NormalizedUserName = user.Email.ToUpper(),
                    };

                    var result = await _userManager.CreateAsync(_user);

                    if (result.Succeeded)
                    {
                        var IsPasswordSet = await _userManager.AddPasswordAsync(_user, user.Password);
                        if (IsPasswordSet.Succeeded)
                        {
                            
                            var token= await _userManager.GenerateEmailConfirmationTokenAsync(_user);
                            var  url = "<a href='" + HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/auth/activate/" + _user.Id
                           + "/" + token
                           +
                           "'>Click here</a>";
                            var body = "Hi!<br/><b>" + _user.UserName + "<b>Thanks for choosing Library management system<br/>"+url+" to activate your account";
                            _emailService.SendEmail(_user.Email, "Activate your account", body);
                            await _userManager.AddToRoleAsync(_user, "User");
                          
                            return Ok(new
                            {
                                user = new {
                                Id=_user.Id,
                                UserName= _user.UserName,
                                Email= _user.Email,
                                CreationTIme= _user.CreationTime,
                                EmailConfirmed= _user.EmailConfirmed,
                                role = role
                                },
                              
                            });
                        }
                        else
                        {
                            await _userManager.DeleteAsync(_user);
                            var err = "";
                            foreach (var e in IsPasswordSet.Errors)
                            {
                                err += e.Description + " ";
                            }
                            _logger.LogError(err+DateTime.UtcNow.ToString());
                            return BadRequest(new
                            {
                                Message = err
                            });
                        }
                    }
                    else
                    {
                        var err = "";
                        foreach (var e in result.Errors)
                        {
                            err += e.Description + " ";
                        }
                        _logger.LogError(err + DateTime.UtcNow.ToString());
                        return BadRequest(new
                        {
                            Message = err
                        });
                    }

                }
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                _validationService.ValidateLoginRequest(request);
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || user.IsDeleted == true)
                {
                    _logger.LogError("User doesn't exists" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "User doesn't exists"
                    });
                }

                var result = await _signinManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
                if(result.Succeeded)
                {
                    //if (user.EmailConfirmed == false)
                    //{
                    //    _logger.LogError("Account not activated" + DateTime.UtcNow.ToString());
                    //    return Unauthorized(new
                    //    {
                    //        Message = "Account not activated"
                    //    });
                    //}
                    var roles = await _userManager.GetRolesAsync(user);
                 
                 
                    return Ok(new
                    {
                        user = new
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            CreationTIme = user.CreationTime,
                            Role= roles.FirstOrDefault(),
                            EmailConfirmed = user.EmailConfirmed,
                            Token = _jwtService.GenerateJWT(user,roles)
                        }
                    });
                }
                else
                {
                    _logger.LogError("Invalid credentials" + DateTime.UtcNow.ToString());
                    return Unauthorized(new
                    {
                        Message = "Invalid credentials"
                    });
                }
          
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
         
        }

        [HttpGet("activate/{Id:Guid}/{Token}")]
        public async Task<IActionResult> ActivateAccount(Guid Id, string Token)
        {
  
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if(user== null)
            {
                return BadRequest(new
                {
                    Message = "User doesn't exists"
                });
            }
            var result = await _userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Account activated" + DateTime.UtcNow.ToString());
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new
                {
                    user = new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        CreationTIme = user.CreationTime,
                        EmailConfirmed = user.EmailConfirmed,
                        Token = _jwtService.GenerateJWT(user, roles)
                    }
                });
            }

            else
            {
                var err = "";
                foreach (var e in result.Errors)
                {
                    err += e.Description + " ";
                }
                _logger.LogError(err + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = err
                });
            }

        }
        [HttpGet("token-resend/{Id:Guid}")]
        public async Task<IActionResult> ResendActivationToken(Guid Id)
        {
            try { 
            var _user = await _userManager.FindByIdAsync(Id.ToString());
            if (_user == null)
            {
                    _logger.LogError("User doesn't exists" + DateTime.UtcNow.ToString());
                    return NotFound(new
                {
                    Message = "User doesn't exists"
                });
            }
            
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(_user);

                var url = "<a href='" + HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/api/auth/activate/" + _user.Id
           + "/" + token

           +
           "'>Click here</a>";
            var body = "Hi!<br/><b>" + _user.UserName + "<b>Thanks for choosing Library management system<br/>" + url + " to activate your account";
            _emailService.SendEmail(_user.Email, "Activate your account", body);
            _user.EmailConfirmed = false;
            _context.Entry(_user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

                _logger.LogInformation("Activation mail sent" + DateTime.UtcNow.ToString());
                return Ok(new{
                Mesasge="Activation mail sent"
            });
        }catch(Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
    });
            }
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> me() { 
            try
            {
                var email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value;
               var user = await _userManager.FindByEmailAsync(email);

                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new 
                    {
                    user = new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = roles.FirstOrDefault(),
                        CreationTIme = user.CreationTime,
                        EmailConfirmed = user.EmailConfirmed
                    }
                }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            try
            {
                _logger.LogInformation("Get users" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    //s.Select(user => new
                    //{
                    //    Id = user.Id,
                    //    UserName = user.UserName,
                    //    Email = user.Email,
                    //    CreationTIme = user.CreationTime,
                    //    EmailConfirmed = user.EmailConfirmed,

                    //}).
                    users = _context.User.Where(x=>x.IsDeleted==false).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("delete/{Id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            try
            {
                 var user  = _context.Users.FirstOrDefault(u => u.Id == Id);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        Message = "User doesn't exists"
                    });
                }

                user.IsDeleted = true;
                user.DeletionTIme = DateTime.UtcNow;
                user.LastModifiedTime = DateTime.UtcNow;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Delete user" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                  Message="User deleted"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
