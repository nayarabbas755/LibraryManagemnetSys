using LibMgt.LibDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibMgt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {


        private readonly ILogger<BooksController> _logger;
        private readonly LibraryDbContext _context;

        public BooksController(ILogger<BooksController> logger,LibraryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetBooks")]
        [Authorize]
        public IActionResult GetBooks()
        {
            return Ok(new
            {
                books= _context.Books.Where(x => x.IsDeleted == false).ToList()
        });
        }
    }
}
