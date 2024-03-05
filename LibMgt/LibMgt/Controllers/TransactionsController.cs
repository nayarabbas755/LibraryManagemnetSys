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
                    tran = _context.Transactions.Where(x => x.IsDeleted == false).ToList()
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
                var transaction = await _context.Transactions.Where(x => x.IsDeleted == false && x.Id==Id).FirstOrDefaultAsync();
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
 
        [HttpPut( "update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDeleteTransaction(UpdateTransactionRequest request)
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

                Transaction transaction = await _context.Transactions.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if(transaction == null)
                {
                    _logger.LogError("transaction not found" + DateTime.UtcNow.ToString());
                    return NotFound(new
                    {
                        Message = "transaction not found"
                    });
                }
                if (_ValidationService.ValidateString(request.Title))
                {
                    if (!transaction.TransactionType.Equals(request.Title))
                    {
                        transaction.TransactionType = request.Title;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.ISBN))
                {
                    if (!transaction.FineAmount.Equals(request.ISBN))
                    {
                        transaction.FineAmount = request.FineAmount;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.Author))
                {
                    if (!transaction.Author.Equals(request.Author))
                    {
                        transaction.Author = request.Author;
                        check = true;
                    }
                }
                if (_ValidationService.ValidateString(request.AvailabilityStatus))
                {
                    if (!transaction.AvailabilityStatus.Equals(request.AvailabilityStatus))
                    {
                        transaction.AvailabilityStatus = request.AvailabilityStatus;
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
                if (request.IsDeleted!=null)
                {
                    transaction.IsDeleted=(bool)request.IsDeleted;
                    if (request.IsDeleted==true)
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
                if (_ValidationService.ValidateDateTime(request.PublicationDate))
                {
                    transaction.PublicationDate = request.PublicationDate;
                    check = true;
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
