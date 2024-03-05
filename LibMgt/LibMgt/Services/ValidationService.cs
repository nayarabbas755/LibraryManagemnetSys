using LibMgt.FrontEndRequests;
using LibMgt.Controllers;
using System.Text.RegularExpressions;

namespace LibMgt.Services
{
    public class ValidationService
    {

        public bool ValidateEmail(string email)
        {
            if(!ValidateString(email))
            {
                return false;
            }
            if (!Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            return true;
        }
        public bool ValidateString(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            return true;
        }
        public bool ValidateUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }
            return true;
        }
        public void ValidateSignupRequest(SignupRequest request)
        {
            if (!ValidateEmail(request.Email))
            {
                throw new ArgumentException("Invalid email");

            };
            if (!ValidatePassword(request.Password))
            {
                throw new ArgumentException("Password is required");
            };
            if (!ValidateUserName(request.UserName))
            {
                throw new ArgumentException("UserName is required");
            };
         
        }
        public void ValidateCreateReservationRequest(CreateReservationRequest request)
        {
            if (request.BookId == null)
            {
                throw new ArgumentException("BookId is required");
            }
            if (request.PatronID == null)
            {
                throw new ArgumentException("PatronID is required");
            }

            if (!ValidateDateTime(request.ReservationDate))
            {
                throw new ArgumentException("ReservationDate is required");
            };
            if (!ValidateString(request.Status))
            {
                throw new ArgumentException("Status is required");
            };
            if (!ValidateString(request.OtherDetails))
            {
                throw new ArgumentException("OtherDetails is required");
            };
            if (request.UserId==null)
            {
                throw new ArgumentException("UserId is required");
            };

        }
        public void ValidateLoginRequest(LoginRequest request)
        {
            if (!ValidateEmail(request.Email))
            {
                throw new ArgumentException("Invalid email");

            };
            if (!ValidatePassword(request.Password))
            {
                throw new ArgumentException("Password is required");
            };
       
         
        }

        public bool ValidateDateTime(DateTime? time)
        {
            if (time == null)
            {
                return false;
            }
            DateTime tempDate;
            if (!DateTime.TryParse(time.ToString(), out tempDate))
            {
                return false;
            }
            return true;
        }
        public void ValidateTransactionCreateRequest(TransactionCreateRequest request)
        {
           
            if (!ValidateString(request.Title))
            {
                throw new ArgumentException("Title is required");
            };
       
            if (!ValidateString(request.Author))
            {
                throw new ArgumentException("Author is required");
            };
       
            if (!ValidateString(request.ISBN))
            {
                throw new ArgumentException("ISBN is required");
            };
       
            if (!ValidateString(request.Genre))
            {
                throw new ArgumentException("Genre is required");
            };
            if (!ValidateDateTime(request.PublicationDate))
            {
                throw new ArgumentException("PublicationDate is required");
            };
            if (!ValidateString(request.AvailabilityStatus))
            {
                throw new ArgumentException("AvailabilityStatus is required");
            };
            if (!ValidateString(request.OtherDetails))
            {
                throw new ArgumentException("OtherDetails is required");
            };
       
         
        } 
        public void ValidatePatronCreateRequest(CreatePatronRequest request)
        {
           
            if (!ValidateString(request.Name))
            {
                throw new ArgumentException("Password is required");
            };
       
            if (!ValidateString(request.PhoneNumber))
            {
                throw new ArgumentException("PhoneNumber is required");
            };
       
            if (!ValidateEmail(request.Email))
            {
                throw new ArgumentException("Invalid email");
            };
       
            if (!ValidateString(request.Address))
            {
                throw new ArgumentException("Address is required");
            };
          
            if (!ValidateString(request.OtherDetails))
            {
                throw new ArgumentException("OtherDetails is required");
            };
       
         
        }

    }
}
