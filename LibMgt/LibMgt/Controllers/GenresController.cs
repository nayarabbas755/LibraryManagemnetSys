using LibMgt.LibDbContext;
using LibMgt.Models;
using LibMgt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibMgt.Controllers
{

    public record CreateGenreRequest(string GenreName, string Description, string OtherDetails);
    public record UpdateGenreRequest(Guid Id,string? GenreName,string? Description,string? OtherDetails,bool? IsDeleted);
    [ApiController]
    [Route("api/genre/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly LibraryDbContext _context;
        private readonly ValidationService _ValidationService;

        public GenresController(ILogger<GenresController> logger, LibraryDbContext context, ValidationService validationService)
        {
            _logger = logger;
            _context = context;
            _ValidationService = validationService;
        }

        [HttpPost("CreateGenre")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGenre(CreateGenreRequest genre)
        {
            try
            {
                _ValidationService.ValidateGenreRequest(genre);
                var _genre = new Genre()
                {
                    GenreName = genre.GenreName,
                    Description = genre.Description,
                    OtherDetails = genre.OtherDetails,
                    IsDeleted = false,
                    CreationTime = DateTime.UtcNow,
                };
                _context.Genres.Add(_genre);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Genre created" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    genre = _genre
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

        [HttpGet("GetGenres")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetGenres()
        {
            try
            {
                _logger.LogInformation("Get genres " + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    genres = _context.Genres.Where(x => x.IsDeleted == false).ToList()
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
        [HttpGet("GetGenreById/{Id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetgenreById(Guid Id)
        {
            try
            {
                var genre= await _context.Genres.Where(x => x.IsDeleted == false && x.Id == Id).FirstOrDefaultAsync();
                if (genre == null)
                {
                    _logger.LogError("Genre not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Genre not found"
                    });
                }

                _logger.LogInformation("Get genre by id" + Id.ToString() + " " + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    genres = genre
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

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDeleteGenre(UpdateGenreRequest request)
        {
            try
            {
                bool check = false;
                if (!_ValidationService.ValidateString(request.Id.ToString()))
                {
                    _logger.LogError("Id is required" + DateTime.UtcNow.ToString());
                    return BadRequest(new
                    {
                        Message = "Id is required"
                    });
                }

                Genre genre = await _context.Genres.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if (genre == null)
                {
                    _logger.LogError("Genre not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "genre not found"
                    });
                }
                if (_ValidationService.ValidateString(request.GenreName))
                {
                    if (!genre.GenreName.Equals(request.GenreName))
                    {
                        genre.GenreName = request.GenreName;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.Description))
                {
                    if (!genre.Description.Equals(request.Description))
                    {
                        genre.Description = request.Description;
                        check = true;
                    }
                }
               
                if (_ValidationService.ValidateString(request.OtherDetails))
                {
                    if (!genre.OtherDetails.Equals(request.OtherDetails))
                    {
                        genre.OtherDetails = request.OtherDetails;
                        check = true;
                    }
                }
                if (request.IsDeleted != null)
                {
                    genre.IsDeleted = (bool)request.IsDeleted;
                    if (request.IsDeleted == true)
                    {
                        genre.DeletionTIme = DateTime.UtcNow;
                        check = true;

                    }
                    else
                    {
                        genre.DeletionTIme = null;

                        check = true;
                    }
                }
             
                if (check)
                {
                    genre.LastModifiedTime = DateTime.UtcNow;
                    _context.Entry(genre).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("genre updated " + DateTime.UtcNow.ToString());

                }
                return Ok(new
                {
                    genre = genre
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
