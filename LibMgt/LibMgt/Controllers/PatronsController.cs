//using LibMgt.LibDbContext;
//using LibMgt.Models;
//using LibMgt.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace LibMgt.Controllers
//{

//    public record CreatePatronRequest(string Name,string Email,string Address, string PhoneNumber,string OtherDetails);
//    public record UpdatePatronRequest(Guid Id,string? Name,string? Email,string? Address, string? PhoneNumber,string? OtherDetails,bool? IsDeleted);
//    [ApiController]
//    [Route("api/patron/[controller]")]
//    public class PatronController : ControllerBase
//    {


//        private readonly ILogger<PatronController> _logger;
//        private readonly LibraryDbContext _context;
//        private readonly ValidationService _ValidationService;

//        public PatronController(ILogger<PatronController> logger, LibraryDbContext context, ValidationService validationService)
//        {
//            _logger = logger;
//            _context = context;
//            _ValidationService = validationService;
//        }

//        [HttpPost("CreatePatron")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> CreatePatron(CreatePatronRequest patron)
//        {
//            try
//            {
//                _ValidationService.ValidatePatronCreateRequest(patron);
//                var _patron = new Patron()
//                {
//                    Name = patron.Name,
//                    Address = patron.Address,
//                    PhoneNumber = patron.PhoneNumber,
//                    Email = patron.Email,
//                    OtherDetails = patron.OtherDetails,
//                    IsDeleted = false,
//                    CreationTime = DateTime.UtcNow,
//                };
//                _context.Patrons.Add(_patron);
//                await _context.SaveChangesAsync();
//                _logger.LogInformation("Patron created" + DateTime.UtcNow.ToString());
//                return Ok(new
//                {
//                    patron = _patron
//                });

//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
//                return BadRequest(new
//                {
//                    Message = ex.Message
//                });
//            }

//        }

//        [HttpGet("GetPatrons")]
//        [Authorize(Roles = "Admin")]
//        public IActionResult GetPatrons()
//        {
//            try
//            {
//                _logger.LogInformation("Get patrons " + DateTime.UtcNow.ToString());
//                return Ok(new
//                {
//                    patrons = _context.Patrons.Where(x => x.IsDeleted == false).ToList()
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
//                return BadRequest(new
//                {
//                    Message = ex.Message
//                });
//            }
//        }
//        [HttpGet("GetPatronById/{Id:Guid}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetpatronById(Guid Id)
//        {
//            try
//            {
//                var patron = await _context.Patrons.Where(x => x.IsDeleted == false && x.Id == Id).FirstOrDefaultAsync();
//                if (patron == null)
//                {
//                    _logger.LogError("Patron not found" + DateTime.UtcNow.ToString());
//                    return NotFound(new
//                    {
//                        Message = "Patron not found"
//                    });
//                }

//                _logger.LogInformation("Get patron by id" + Id.ToString() + " " + DateTime.UtcNow.ToString());
//                return Ok(new
//                {
//                    patrons = patron
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
//                return BadRequest(new
//                {
//                    Message = ex.Message
//                });
//            }
//        }

//        [HttpPut("update")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateDeletePatron(UpdatePatronRequest request)
//        {
//            try
//            {
//                bool check = false;
//                if (!_ValidationService.ValidateString(request.Id.ToString()))
//                {
//                    _logger.LogError("Id is required" + DateTime.UtcNow.ToString());
//                    return BadRequest(new
//                    {
//                        Message = "Id is required"
//                    });
//                }

//                Patron patron = await _context.Patrons.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
//                if (patron == null)
//                {
//                    _logger.LogError("Patron not found" + DateTime.UtcNow.ToString());
//                    return NotFound(new
//                    {
//                        Message = "patron not found"
//                    });
//                }
//                if (_ValidationService.ValidateString(request.Name))
//                {
//                    if (!patron.Name.Equals(request.Name))
//                    {
//                        patron.Name = request.Name;
//                        check = true;
//                    }
//                }
//                if (_ValidationService.ValidateString(request.Address))
//                {
//                    if (!patron.Address.Equals(request.Address))
//                    {
//                        patron.Address = request.Address;
//                        check = true;
//                    }
//                }
//                if (_ValidationService.ValidateString(request.PhoneNumber))
//                {
//                    if (!patron.PhoneNumber.Equals(request.PhoneNumber))
//                    {
//                        patron.PhoneNumber = request.PhoneNumber;
//                        check = true;
//                    }
//                }
//                if (_ValidationService.ValidateEmail(request.Email))
//                {
//                    if (!patron.Email.Equals(request.Email))
//                    {
//                        patron.Email = request.Email;
//                        check = true;
//                    }
//                }
//                if (_ValidationService.ValidateString(request.OtherDetails))
//                {
//                    if (!patron.OtherDetails.Equals(request.OtherDetails))
//                    {
//                        patron.OtherDetails = request.OtherDetails;
//                        check = true;
//                    }
//                }
//                if (request.IsDeleted != null)
//                {
//                    patron.IsDeleted = (bool)request.IsDeleted;
//                    if (request.IsDeleted == true)
//                    {
//                        patron.DeletionTIme = DateTime.UtcNow;
//                        check = true;

//                    }
//                    else
//                    {
//                        patron.DeletionTIme = null;

//                        check = true;
//                    }
//                }
             
//                if (check)
//                {
//                    patron.LastModifiedTime = DateTime.UtcNow;
//                    _context.Entry(patron).State = EntityState.Modified;
//                    await _context.SaveChangesAsync();
//                    _logger.LogInformation("patron updated " + DateTime.UtcNow.ToString());

//                }
//                return Ok(new
//                {
//                    patron = patron
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
//                return BadRequest(new
//                {
//                    Message = ex.Message
//                });
//            }
//        }
//    }
//}
