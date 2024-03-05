using LibMgt.FrontEndRequests;
using LibMgt.LibDbContext;
using LibMgt.Models;
using LibMgt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibMgt.Controllers
{
    [ApiController]
    [Route("api/fine/[controller]")]
    public class FinesController : ControllerBase
    {


        private readonly ILogger<FinesController> _logger;
        private readonly LibraryDbContext _context;
        private readonly ValidationService _ValidationService;

        public FinesController(ILogger<FinesController> logger,LibraryDbContext context,ValidationService validationService)
        {
            _logger = logger;
            _context = context;
            _ValidationService = validationService;
        }

        [HttpPost("CreateFine")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFine(FineCreateRequest fine)
        {
            try
            {
                 _ValidationService.ValidateFineCreateRequest(fine);
                var _fine = new Fine()
                {
                    FineAmount = fine.FineAmount,
                    FineDate = fine.FineDate,
                    Status = fine.Status,
                    OtherDetails = fine.OtherDetails,
                    IsDeleted = false,
                    CreationTime = DateTime.UtcNow,
                };
                _context.Fines.Add(_fine);
                await _context.SaveChangesAsync();
                _logger.LogInformation("fine created" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    fine = _fine
                });

            }
            catch(Exception ex) {
                _logger.LogError(ex.Message + DateTime.UtcNow.ToString());
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }

        }
    
        [HttpGet( "GetFines")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetFines()
        {
            try
            {
                _logger.LogInformation("Get Fines" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    fines = _context.Fines.Where(x => x.IsDeleted == false).ToList()
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
        [HttpGet( "GetfineById/{Id:Guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetFinById(Guid Id)
        {
            try
            {
                var fine = await _context.Fines.Where(x => x.IsDeleted == false && x.Id==Id).FirstOrDefaultAsync();
                if (fine == null)
                {
                    _logger.LogError("Fine not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Fine not found"
                    }) ;
                }

                _logger.LogInformation("Get Fine by id"+Id.ToString()+ " " + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    fines = fine
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
 
        [HttpPut( "update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDeleteFine(UpdateFineRequest request)
        {
            try
            {
                bool check = false;
                if(!_ValidationService.ValidateString(request.Id.ToString()))
                {
                    _logger.LogError("Id is required" + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = "Id is required"
                    });
                }

                Fine fine = await _context.Fines.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if(fine == null)
                {
                    _logger.LogError("fine not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "fine not found"
                    });
                }
                if (_ValidationService.ValidateString(request.Title))
                {
                    if (!fine.FineAmount.Equals(request.Title))
                    {
                        fine.FineAmount = request.Title;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.ISBN))
                {
                    if (!fine.FineDate.Equals(request.ISBN))
                    {
                        fine.FineDate = request.ISBN;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.Author))
                {
                    if (!fine.Status.Equals(request.Author))
                    {
                        fine.Status = request.Author;
                        check = true;
                    }
                }

                if (_ValidationService.ValidateString(request.OtherDetails))
                {
                    if (!fine.OtherDetails.Equals(request.OtherDetails))
                    {
                        fine.OtherDetails = request.OtherDetails;
                        check = true;
                    }
                }
                if (request.IsDeleted!=null)
                {
                    fine.IsDeleted=(bool)request.IsDeleted;
                    if (request.IsDeleted==true)
                    {
                        fine.DeletionTIme = DateTime.UtcNow;
                        check = true;

                    }
                    else
                    {
                        fine.DeletionTIme = null;

                        check = true;
                    }
                }
                
                if (check)
                {
                    fine.LastModifiedTime = DateTime.UtcNow;
                    _context.Entry(fine).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("fine" +
                        " updated " + DateTime.UtcNow.ToString());
                   
                }
                return Ok(new
                {
                    fines = fine
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
