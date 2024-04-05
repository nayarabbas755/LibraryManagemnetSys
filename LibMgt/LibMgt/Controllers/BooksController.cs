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
    [Route("api/book/[controller]")]
    public class BooksController : ControllerBase
    {


        private readonly ILogger<BooksController> _logger;
        private readonly LibraryDbContext _context;
        private readonly ValidationService _ValidationService;

        public BooksController(ILogger<BooksController> logger,LibraryDbContext context,ValidationService validationService)
        {
            _logger = logger;
            _context = context;
            _ValidationService = validationService;
        }

        [HttpPost("CreateBook")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(BookCreateRequest book)
        {
            try
            {
                 _ValidationService.ValidateBookCreateRequest(book);
                var _book = new Book()
                {
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    Genre =  book.Genre,
                    PublicationDate = book.PublicationDate,
                    AvailabilityStatus = book.AvailabilityStatus,
                    OtherDetails = book.OtherDetails,
                    IsDeleted = false,
                    CreationTime = DateTime.UtcNow,
                };
                _context.Books.Add(_book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Book created" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    book = _book
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
     
        public static Genre GetGenreName(string genreId,LibDbContext.LibraryDbContext _context)
        {
            try
            {
                var name = _context.Genres.Where(y => y.Id == new Guid(genreId)).FirstOrDefault();
                return name;
            }
            catch 
            {
                return null;
            }
                }
        [HttpGet( "GetBooks")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetBooks()
        {
            try
            {
                _logger.LogInformation("Get books" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    books = _context.Books.Where(x => x.IsDeleted == false).Select(x =>
                    new {
                        Title=x.Title,
                        Author = x.Author,
                        Genre= GetGenreName(x.Genre,_context) ,
                        PublicationDate = x.PublicationDate,
                        AvailabilityStatus = x.AvailabilityStatus,
                        Id = x.Id,
                        ISBN = x.ISBN,
                        OtherDetails = x.OtherDetails
                    }).AsEnumerable()
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
    
 
        [HttpGet( "GetBookById/{Id:Guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetBookById(Guid Id)
        {
            try
            {
                var book = await _context.Books.Where(x => x.IsDeleted == false && x.Id==Id).FirstOrDefaultAsync();
                if (book == null)
                {
                    _logger.LogError("Book not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Book not found"
                    }) ;
                }

                _logger.LogInformation("Get books by id"+Id.ToString()+ " " + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    books = new 
                    {
                        Title = book.Title,
                        Author = book.Author,
                        Genre = GetGenreName(book.Genre, _context),
                        PublicationDate = book.PublicationDate,
                        AvailabilityStatus = book.AvailabilityStatus,
                        Id = book.Id,
                        ISBN = book.ISBN,
                        OtherDetails = book.OtherDetails

                    }
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
        public async Task<IActionResult> UpdateDeleteBook(UpdateBookRequest request)
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

                Book book = await _context.Books.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if(book == null)
                {
                    _logger.LogError("Book not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "Book not found"
                    });
                }
                if (_ValidationService.ValidateString(request.Title))
                {
                    if (!book.Title.Equals(request.Title))
                    {
                        book.Title = request.Title;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.Genre))
                {
                    if (!book.Genre.Equals(request.Genre))
                    {
                        book.Genre = request.Genre;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.ISBN))
                {
                    if (!book.ISBN.Equals(request.ISBN))
                    {
                        book.ISBN = request.ISBN;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.Author))
                {
                    if (!book.Author.Equals(request.Author))
                    {
                        book.Author = request.Author;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.AvailabilityStatus))
                {
                    if (!book.AvailabilityStatus.Equals(request.AvailabilityStatus))
                    {
                        book.AvailabilityStatus = request.AvailabilityStatus;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.OtherDetails))
                {
                    if (!book.OtherDetails.Equals(request.OtherDetails))
                    {
                        book.OtherDetails = request.OtherDetails;
                        check = true;
                    }
                }
                if (request.IsDeleted!=null)
                {
                    book.IsDeleted=(bool)request.IsDeleted;
                    if (request.IsDeleted==true)
                    {
                        book.DeletionTIme = DateTime.UtcNow;
                        check = true;

                    }
                    else
                    {
                        book.DeletionTIme = null;

                        check = true;
                    }
                }
                if (_ValidationService.ValidateDateTime(request.PublicationDate))
                {
                    book.PublicationDate = request.PublicationDate;
                    check = true;
                }
                if (check)
                {
                    book.LastModifiedTime = DateTime.UtcNow;
                    _context.Entry(book).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Book updated " + DateTime.UtcNow.ToString());
                   
                }
                return Ok(new
                {
                    books = book
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
