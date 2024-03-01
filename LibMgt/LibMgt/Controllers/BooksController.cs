using Microsoft.AspNetCore.Mvc;

namespace LibMgt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {


        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBooks")]
        public IEnumerable<IActionResult> GetBooks()
        {
            return new List<IActionResult>();
        }
    }
}
