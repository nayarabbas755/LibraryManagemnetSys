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
        public AuthController(ILogger<AuthController> logger, LibraryDbContext context,UserManager<User> userManager,ValidationService validationService,EmailService emailService,SignInManager<User> signInManager,JWTService jwtService)
        { 
            _context = context;
            _userManager = userManager;
            _validationService = validationService;
            _emailService=emailService;
            _signinManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

  
        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("register")]
        public async Task<IActionResult> Create(SignupRequest user)
        {

            try
            {
                _validationService.ValidateSignupRequest(user);

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
                            return Ok(new
                            {
                                user = new {
                                Id=_user.Id,
                                UserName= _user.UserName,
                                Email= _user.Email,
                                CreationTIme= _user.CreationTime,
                                EmailConfirmed= _user.EmailConfirmed,
                             
                                }
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
                if (user == null)
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
                    return Ok(new
                    {
                        user = new
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            CreationTIme = user.CreationTime,
                            EmailConfirmed = user.EmailConfirmed,
                            Token = _jwtService.GenerateJWT(user)
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
                return Ok(new
                {
                    user = new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        CreationTIme = user.CreationTime,
                        EmailConfirmed = user.EmailConfirmed,
                        Token = _jwtService.GenerateJWT(user)
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
   
    }
}
