using Authentication.Models.Entities;
using Authentication.Models.Helpers;
using Authentication.Models.ViewModels;
using Authentication.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using static Authentication.Models.Helpers.Constants;
using static Authentication.Models.Helpers.Enums;

namespace Authentication.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly IConfiguration _configuration;
        protected readonly IUnitOfWorkRepository _iUnitOfWorkRepository;
        private readonly UIProjectSetting _uiIProjectSetting;
        private Microsoft.Extensions.Hosting.IHostEnvironment _hostingEnvironment;
        private readonly IEmailSender _emailSender;

        public AuthenticateController(
            UserManager<Users> userManager,
            RoleManager<Roles> roleManager,
            IConfiguration configuration,
            IUnitOfWorkRepository iUnitOfWorkRepository,
            IOptions<UIProjectSetting> uiIProjectSetting,
            IHostEnvironment hostingEnvironment,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _iUnitOfWorkRepository = iUnitOfWorkRepository;
            _uiIProjectSetting = uiIProjectSetting.Value;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserVM model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new CResponseModel
                {
                    IsSuccess = false,
                    SuccessCode = ResponseStatusCode.ClientError,
                    Message = string.Format(Message.AlreadyExistsMessage, Constants.User)
                });
            }

            Users user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FullName = model.FullName,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new CResponseModel
                    {
                        IsSuccess = false,
                        SuccessCode = ResponseStatusCode.ClientError,
                        Message = string.Format(Message.NotCreateMessage, Constants.User, Constants.User.ToLower())
                    });
            }

            return Ok(new CResponseModel
            {
                IsSuccess = true,
                SuccessCode = ResponseStatusCode.SuccessFully,
                Message = string.Format(Message.CreateSuccessMessage, Constants.User)
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.IsActive)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var token = GetToken(authClaims);

                    return StatusCode(StatusCodes.Status200OK, new CResponseModel
                    {
                        IsSuccess = true,
                        SuccessCode = ResponseStatusCode.SuccessFully,
                        Data = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            userDetail = user,
                        },
                        Message = Message.SuccessFullyLoginMessage
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new CResponseModel
                    {
                        IsSuccess = false,
                        SuccessCode = ResponseStatusCode.ServerError,
                        Message = Message.InActiveUserMessage
                    });
                }
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new CResponseModel
            {
                IsSuccess = false,
                SuccessCode = ResponseStatusCode.ServerError,
                Message = Message.LoginFailedMessage
            });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            try
            {
                var user = new Users();
                //if (resetPasswordVM.userId != null)
                //{
                //    user = _userManager.Users.Where(m => m.Id == resetPasswordVM.userId).FirstOrDefault();
                //    resetPasswordVM.Code = user != null ? await _iUnitOfWorkRepository.UserManager.GeneratePasswordResetTokenAsync(user) : null;
                //}
                if (resetPasswordVM.Email != null)
                {
                    user = _userManager.Users.Where(m => m.Email == resetPasswordVM.Email).FirstOrDefault();
                }

                // await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new CResponseModel
                    {
                        IsSuccess = false,
                        SuccessCode = ResponseStatusCode.ClientError,
                        Message = Message.InValidEmail
                    });
                }
                var result = (dynamic?)null;
                //if (resetPasswordVM.CurrentPassword == null)
                //{
                var code = resetPasswordVM.Code?.Replace(" ", "+");
                result = await _userManager.ResetPasswordAsync(user, code, resetPasswordVM.Password);
                //}
                //else
                //{
                //result = await _userManager.ChangePasswordAsync(user, resetPasswordVM.CurrentPassword, resetPasswordVM.Password);
                //}
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new CResponseModel
                    {
                        IsSuccess = true,
                        SuccessCode = ResponseStatusCode.SuccessFully,
                        Message = Message.ResetPasswordMessage
                    });
                }

                string msg = string.Empty;
                foreach (var error in result.Errors)
                {
                    msg = msg + error.Description;
                }
                if (msg != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new CResponseModel
                    {
                        IsSuccess = false,
                        SuccessCode = ResponseStatusCode.ServerError,
                        Message = msg
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new CResponseModel
                    {
                        IsSuccess = true,
                        SuccessCode = ResponseStatusCode.SuccessFully,
                        Message = Message.ResetPasswordMessage
                    });
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest, new CResponseModel
                {
                    IsSuccess = false,
                    SuccessCode = ResponseStatusCode.ClientError,
                    Message = Constants.Message.GlobalExceptionError
                });
            }
        }

        [HttpPost]
        [Route("SendEmailForChangePassword")]
        public async Task<IActionResult> SendEmailForChangePassword(List<string> emails)
        {
            try
            {
                foreach (var email in emails)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new CResponseModel
                        {
                            IsSuccess = false,
                            SuccessCode = ResponseStatusCode.ClientError,
                            Message = string.Format(Message.NotExistsMessage, "Email")
                        });
                    }
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = string.Concat(_uiIProjectSetting.URL, string.Format(Constants.ResetPassword, code));

                    string bodyTemplate = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplate/ForgotPasswordLink.html"));
                    bodyTemplate = bodyTemplate.Replace("[confirmationLink]", HtmlEncoder.Default.Encode(callbackUrl));

                    await _emailSender.SendEmailAsyncWithBody(user.Email, "Reset your account password", bodyTemplate, true);
                }
                return StatusCode(StatusCodes.Status200OK, new CResponseModel
                {
                    IsSuccess = true,
                    SuccessCode = ResponseStatusCode.SuccessFully,
                    Message = string.Format(Message.EmailSendSuccessfully, Constants.ForgotPassword)
                });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new CResponseModel
                {
                    IsSuccess = false,
                    SuccessCode = ResponseStatusCode.ClientError,
                    Message = Constants.Message.GlobalExceptionError,
                    ExceptionMessage = e.Message,
                    ExceptionObject = e
                });
            }
        }

    }
}
