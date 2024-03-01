using Microsoft.AspNetCore.Mvc;
using LibMgt.LibDbContext;
using LibMgt.Models;
using Microsoft.AspNetCore.Identity;
using LibMgt.FrontEndRequests;

namespace LibMgt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<User> _userManager;

        public AuthController(LibraryDbContext context,UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

  
        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Signup")]
        public async Task<IActionResult> Create(SignupRequest user)
        {
            var exists =await _userManager.FindByEmailAsync(user.Email);
            if (exists!=null)
            {
                return BadRequest(new{
                    Message="Email already in use"
                });
            }
            else
            {
                var _user = new User
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    CreationTime = DateTime.UtcNow,
                    IsDeleted=false,
                    EmailConfirmed = true,
                    NormalizedEmail= user.Email.ToLower(),
                    NormalizedUserName = user.Email.ToUpper(),
            };
   
                var result =  await _userManager.CreateAsync(_user);

                if (result.Succeeded)
                {
                  var IsPasswordSet=   await _userManager.AddPasswordAsync(_user, user.Password);
                    if(IsPasswordSet.Succeeded)
                    {
                        return Ok(new
                        {
                            user = _user
                        });
                    }
                    else
                    {
                        await _userManager.DeleteAsync(_user);
                        var err = "";
                        foreach(var e in IsPasswordSet.Errors)
                        {
                            err += e.Description + " ";
                        }
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
                    return BadRequest(new
                    {
                        Message = err
                    });
                }

            }
        }

    }
}
