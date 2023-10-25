using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Models.Helpers
{
    public static class Constants
    {
        public static class Message
        {
            public const string SuccessFullyLoginMessage = "Logged in successfully.";
            public const string LoginFailedMessage = "Invalid login credentials.";
            public const string InActiveUserMessage = "Your account is inactive. Please contact your administrator.";
            public const string ResetPasswordMessage = "Your password has been reset.";
            public const string InValidEmail = "Please enter valid email address!";
            public const string EmailSendSuccessfully = "{0} mail send successfully";

            public const string CreateSuccessMessage = "{0} added successfully!";
            public const string UpdateSuccessMessage = "{0} updated successfully!";
            public const string DeleteSuccessMessage = "{0} deleted successfully!";
            public const string NotFoundMessage = "{0} detail not found!";

            public const string AlreadyExistsMessage = "{0} already exists.Please use a different {0}";
            public const string NotExistsMessage = "{0} not exists.";
            public const string NotCreateMessage = "{0} creation failed! Please check {0} details and try again.";
            public const string GlobalExceptionError = "Something went wrong. Please contact to administrator.";
        }

        public const string User = "User";
        public const string UserName = "User name";
        public const string Email = "Email";
        public const string ForgotPassword = "Reset Password";
        public const string ResetPassword = "/auth/resetpassword?code={0}";
    }
}
