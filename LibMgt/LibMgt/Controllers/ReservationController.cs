using LibMgt.LibDbContext;
using LibMgt.Models;
using LibMgt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace LibMgt.Controllers
{
    public record CreateReservationRequest(Guid BookId, Guid PatronID, DateTime ReservationDate, string Status, string OtherDetails,Guid UserId);
    [Route("api/reservation/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly LibraryDbContext _context;
        private readonly ValidationService _ValidationService;

        public ReservationController(ILogger<BooksController> logger, LibraryDbContext context, ValidationService validationService)
        {
            _logger = logger;
            _context = context;
            _ValidationService = validationService;
        }

        [HttpPost("CreateReservation")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateReservation(CreateReservationRequest request)
        {
            try
            {
                _ValidationService.ValidateCreateReservationRequest(request);
                var book = await _context.Books.Where(x=>x.Id==request.BookId).FirstOrDefaultAsync();
                if(book==null)
                {
                    _logger.LogError("Book not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Book not found"
                    });
                }
                var patron = await _context.Patrons.Where(x=>x.Id==request.PatronID).FirstOrDefaultAsync();
                if(patron==null)
                {
                    _logger.LogError("Patron not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Patron not found"
                    });
                }

                Reservation reservation = new Reservation()
                {
                    BookID = book.Id,
                    PatronID = patron.Id,
                    Status = request.Status,
                    OtherDetails = request.OtherDetails,
                    IsDeleted=false,
                    ReservationDate = request.ReservationDate,
                    CreationTime = DateTime.UtcNow,
                  

                };
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    reservation = reservation,
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
        [HttpGet("GetReservations")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservations()
        {
            try
            {
                _logger.LogInformation("Get reservations" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    reservations = _context.Reservations.Include(x=>x.Patron).Include(x=>x.Book).Include(x=>x.Patron).ToList()
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
        } [HttpGet("GetReservationsbyUser")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetReservationsByUser()
        {
            try
            {
                _logger.LogInformation("Get reservations" + DateTime.UtcNow.ToString());
                var email = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Ok(new
                {
                    reservations = _context.Reservations.Include(x=>x.Patron).Include(x=>x.Book).Where(x => x.IsDeleted == false && x.Patron.Email.Equals(email) ).ToList()
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
