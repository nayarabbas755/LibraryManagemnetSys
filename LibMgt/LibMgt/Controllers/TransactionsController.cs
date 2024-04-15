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
    public record TransactionCreateRequest(Guid BookID, Guid PatronID, string? TransactionType, DateTime? TransactionDate, DateTime? DueDate, decimal? FineAmount, string? OtherDetails);
    public record UpdateTransactionRequest(Guid Id,Guid? BookID, Guid PatronID, string? TransactionType, DateTime? TransactionDate, DateTime? DueDate, decimal? FineAmount, string? OtherDetails,bool? IsDeleted);
    [ApiController]
    [Route("api/transaction/[controller]")]
    public class TransactionsController : ControllerBase
    {


        private readonly ILogger<TransactionsController> _logger;
        private readonly LibraryDbContext _context;
        private readonly ValidationService _ValidationService;

        public TransactionsController(ILogger<TransactionsController> logger,LibraryDbContext context,ValidationService validationService)
        {
            _logger = logger;
            _context = context;
            _ValidationService = validationService;
        }

        [HttpPost("CreateTransactions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTransaction(TransactionCreateRequest transaction)
        {
            try
            {
                 _ValidationService.ValidateTransactionCreateRequest(transaction);
                var _transaction = new Transaction()
                {
                    TransactionType = transaction.TransactionType,
                    TransactionDate = transaction.TransactionDate,
                    DueDate = transaction.DueDate,
                    FineAmount = transaction.FineAmount, 
                    BookID=transaction.BookID,
                    PatronID=transaction.PatronID,
                    OtherDetails = transaction.OtherDetails,
                    IsDeleted = false,
                    CreationTime = DateTime.UtcNow,
                };
                _context.Transactions.Add(_transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation("transaction created" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    transaction = _transaction
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
    
        [HttpGet( "GetTransactions")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetTransactions()
        {
            try
            {
                _logger.LogInformation("Get transaction" + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    tran = _context.Transactions.Include(x => x.Book).Include(x => x.Patron).Where(x => x.IsDeleted == false).ToList()
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
        [HttpGet( "GetById/{Id:Guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetTransactionById(Guid Id)
        {
            try
            {
                var transaction = await _context.Transactions.Include(x => x.Book).Include(x => x.Patron).Where(x => x.IsDeleted == false && x.Id==Id).FirstOrDefaultAsync();
                if (transaction == null)
                {
                    _logger.LogError("transaction not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "transaction not found"
                    }) ;
                }

                _logger.LogInformation("Get transaction by id" + Id.ToString()+ " " + DateTime.UtcNow.ToString());
                return Ok(new
                {
                    tran = transaction
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
        public async Task<IActionResult> UpdateDeleteTransaction(UpdateTransactionRequest request)
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

                Transaction transaction = await _context.Transactions.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if (transaction == null)
                {
                    _logger.LogError("transaction not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "transaction not found"
                    });
                }
                if (_ValidationService.ValidateString(request.TransactionType))
                {
                    if (!transaction.TransactionType.Equals(request.TransactionType))
                    {
                        transaction.TransactionType = request.TransactionType;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateDecimal(request.FineAmount))
                {
                    if (!transaction.FineAmount.Equals(request.FineAmount))
                    {
                        transaction.FineAmount = request.FineAmount;
                        check = true;
                    }
                }
                if (request.PatronID!=null)
                {
                    if (!transaction.PatronID.ToString().Equals(request.PatronID.ToString()))
                    {
                        transaction.PatronID = request.PatronID;
                        check = true;
                    }
                }
                if (request.BookID!=null)
                {
                    if (!transaction.BookID.ToString().Equals(request.BookID.ToString()))
                    {
                        transaction.BookID = (Guid)request.BookID;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateDateTime(request.DueDate))
                {
                    if (!transaction.DueDate.Equals(request.DueDate))
                    {
                        transaction.DueDate = request.DueDate;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateDateTime(request.TransactionDate))
                {
                    if (!transaction.TransactionDate.Equals(request.TransactionDate))
                    {
                        transaction.TransactionDate = request.TransactionDate;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.OtherDetails))
                {
                    if (!transaction.OtherDetails.Equals(request.OtherDetails))
                    {
                        transaction.OtherDetails = request.OtherDetails;
                        check = true;
                    }
                }
                if (request.IsDeleted != null)
                {
                    transaction.IsDeleted = (bool)request.IsDeleted;
                    if (request.IsDeleted == true)
                    {
                        transaction.DeletionTIme = DateTime.UtcNow;
                        check = true;

                    }
                    else
                    {
                        transaction.DeletionTIme = null;

                        check = true;
                    }
                }
          
                if (check)
                {
                    transaction.LastModifiedTime = DateTime.UtcNow;
                    _context.Entry(transaction).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("transaction updated " + DateTime.UtcNow.ToString());

                }
                return Ok(new
                {
                    transactions = transaction
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
