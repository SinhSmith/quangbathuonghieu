using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Portal.CMS.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using Portal.Core.Service;
using Portal.Core.Database;
using Portal.Core.Util;

namespace Portal.CMS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    RequestConfirmViewModel requestConfirmViewModel = new RequestConfirmViewModel();
                    requestConfirmViewModel.Email = model.Email;
                    return View("RequestConfirm", requestConfirmViewModel);
                    //return RedirectToAction("RequestConfirm", "Account");

                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Thông tin đăng nhập không đúng.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult RequestConfirm()
        {
            RequestConfirmViewModel model = new RequestConfirmViewModel();
            return View(model);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestConfirm(RequestConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null && !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, provider = string.Empty }, protocol: Request.Url.Scheme);
                    Portal.Core.Util.Mail.Send(model.Email, "ZODY - Xác nhận địa chỉ email", MailContent.ConfirmEmailContent.Replace("[LINK]", "<a href=\"" + callbackUrl + "\">" + callbackUrl.ToLower() + "</a>"));
                    

                    ViewBag.Title = "Xác nhận địa chỉ email";
                    ViewBag.Message = "<p>&#8226; Để hoàn tất quá trình đăng ký, chúng tôi cần xác nhận rằng bạn sở hữu địa chỉ email mà bạn sử dụng để cài đặt tài khoản</p>";
                    ViewBag.Message += "<p>&#8226; Bạn vui lòng kiểm tra email của bạn và tìm email xác nhận (chủ đề: \"Xác minh email của ZODY\"). Làm theo các bước trong email để xác nhận địa chỉ email của bạn.</p>";

                    return View("AlertWithLoginSocial");
                }
                else if (user == null)
                {
                    return View("AlertWithLoginSocial");
                }
                else if (await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    return View("AlertWithLoginSocial");
                }
                return View("AlertWithLoginSocial");
            }
            return View("AlertWithLoginSocial");
        }



        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //CompanyService.CreateCompany(Guid.Parse(user.Id));
                    //ResumeService.CreateResume(Guid.Parse(user.Id), model.Email);
                    //ProfileService.UpdateProfile(new Profile() { UserId = Guid.Parse(user.Id), Role = model.Role });
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, provider = string.Empty }, protocol: Request.Url.Scheme);
                    Portal.Core.Util.Mail.Send(model.Email, "Xác minh email của ZODY", "Cảm ơn bạn rất nhiều vì đã tham gia vào Mạng việc làm và tuyển dụng ZODY.VN. Để kết thúc việc đăng ký, bạn chỉ cần xác nhận rằng chúng tôi có đúng địa chỉ email của bạn. Vui lòng nhấn vào đường dẫn sau để xác nhận:  <a href=\"" + callbackUrl + "\">here</a>");

                    ViewBag.Title = "Xác nhận địa chỉ email";
                    ViewBag.Message = "<p>&#8226; Để hoàn tất quá trình đăng ký, chúng tôi cần xác nhận rằng bạn sở hữu địa chỉ email mà bạn sử dụng để cài đặt tài khoản</p>";
                    ViewBag.Message += "<p>&#8226; Bạn vui lòng kiểm tra email của bạn và tìm email xác nhận (chủ đề: \"Xác minh email của ZODY\"). Làm theo các bước trong email để xác nhận địa chỉ email của bạn.</p>";

                    return View("AlertWithLoginSocial");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                else
                {
                    //For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);                   
                    Portal.Core.Util.Mail.Send(model.Email, "ZODY - Thay đổi mật khẩu", MailContent.ResetEmailContent.Replace("[LINK]", "<a href=\"" + callbackUrl + "\">" + callbackUrl.ToLower() + "</a>"));
                    
                    ViewBag.Title = "Đăng ký tìm lại mật khẩu";
                    ViewBag.Message = "<p>&#8226; Chúng tôi đã gửi yêu cầu việc lấy lại mật khẩu qua email của bạn. Vui lòng kiểm tra lại hòm thư email và làm theo hướng dẫn.</p>";
                    ViewBag.Message += "<p><strong>&#8226; Lưu ý:</strong> Kiểm tra mục Bulk/Spam mail nếu bạn không tìm thấy mail lấy lại mật khẩu trong Inbox. Nếu bạn thấy mail của ZODY.VN trong mục Bulk/Spam mail, vui lòng đánh dấu lại email \"Not junk/spam\"</p>";
                    
                    return View("AlertWithLoginSocial");
                    //return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            if (userId == null)
                return View("Error");
            else
            {
                var user = UserManager.FindById(userId);
                if (user == null)
                    return View("Error");
                else
                {
                    ResetPasswordViewModel model = new ResetPasswordViewModel();
                    model.Email = user.Email;
                    model.Code = code;
                    model.Password = "";
                    model.ConfirmPassword = "";
                    return View(model);
                }
            }
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    if (loginInfo.Login.LoginProvider == "Google")
                    {
                        var json = JObject.Parse(loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "urn:tokens:google:name").Value);
                        string firstName = (string)json["givenName"];
                        string lastName = (string)json["familyName"];

                        string avatarUrl = (string)JObject.Parse(loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "urn:tokens:google:image").Value)["url"];

                        var user = await UserManager.FindByNameAsync(loginInfo.Email);
                        Profile profile = null;
                        ////if (user != null)
                        ////    profile = ProfileService.GetProfile(user.Id);

                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                        {
                            Email = loginInfo.Email,
                            FirstName = firstName,
                            LastName = lastName,
                            LoginProvider = loginInfo.Login.LoginProvider,
                            Avatar = avatarUrl
                        });
                    }
                    else if (loginInfo.Login.LoginProvider.Equals("LinkedIn"))
                    {

                        string accessToken = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals("LinkedIn:AccessToken")).Value.ToString();

                        string urlPicture = GetUrlPictureFromLinkedIn(accessToken);

                        var user = await UserManager.FindByNameAsync(loginInfo.Email);
                        Profile profile = null;
                        ////if (user != null)
                        ////    profile = ProfileService.GetProfile(user.Id);

                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                        {
                            Email = loginInfo.Email,
                            FirstName = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals("LinkedIn:FirstName")).Value.ToString(),
                            LastName = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals("LinkedIn:LastName")).Value.ToString(),
                            LoginProvider = loginInfo.Login.LoginProvider,
                            Avatar = urlPicture
                        });

                    }
                    else if (loginInfo.Login.LoginProvider.Equals("Facebook"))
                    {
                        string avatar = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value.ToString();
                        avatar = String.Format("https://graph.facebook.com/{0}/picture", avatar);

                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                        {
                            Email = loginInfo.Email,
                            FirstName = string.Empty,
                            LastName = loginInfo.ExternalIdentity.Name,
                            LoginProvider = loginInfo.Login.LoginProvider,
                            Avatar = avatar
                        });
                    }
                    else if (loginInfo.Login.LoginProvider == "Microsoft")
                    {
                        var user = await UserManager.FindByNameAsync(loginInfo.Email);
                        Profile profile = null;
                        ////if (user != null)
                        ////    profile = ProfileService.GetProfile(user.Id);

                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
                        {
                            Email = loginInfo.Email,
                            FirstName = "",
                            LastName = "",
                            LoginProvider = loginInfo.Login.LoginProvider,
                            Avatar = ""
                        });
                    }
                    else
                    {
                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
                    }

            }
        }

        private string GetUrlPictureFromLinkedIn(string accessToken)
        {
            string result = string.Empty;
            var apiRequrestResult = new Uri("https://api.linkedin.com/v1/people/~:(picture-url)?format=json");
            using (var webclient = new WebClient())
            {
                webclient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
                var json = webclient.DownloadString(apiRequrestResult);
                dynamic x = JsonConvert.DeserializeObject(json);
                string userPicture = x.pictureUrl;
                result = userPicture;
            }

            return result;
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var existingUser = await UserManager.FindByEmailAsync(user.Email);
                IdentityResult result = null;
                if (existingUser == null)
                    result = await UserManager.CreateAsync(user);
                else
                    user.Id = existingUser.Id;
                if (existingUser != null || result.Succeeded)
                {
                    ////CompanyService.CreateCompany(Guid.Parse(user.Id));
                    ////ResumeService.CreateResume(Guid.Parse(user.Id), model.Email);
                    ////ProfileService.UpdateProfile(new Profile() { UserId = Guid.Parse(user.Id), Role = model.Role });

                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        ////ProfileService.UpdateProfile(new Profile() { UserId = Guid.Parse(user.Id), Role = model.Role });
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Job");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Job");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}