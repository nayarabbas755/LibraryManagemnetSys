using LibMgt.FrontEndRequests;
using System.Text.RegularExpressions;

namespace LibMgt.Services
{
    public class ValidationService
    {

        public bool ValidateEmail(string email)
        {
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
    }
}
