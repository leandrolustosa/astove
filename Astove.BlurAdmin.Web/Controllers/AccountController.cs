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
using Astove.BlurAdmin.Web.Models;
using AInBox.Astove.Core.Attributes;
using AInBox.Astove.Core.Model;
using AInBox.Astove.Core.Security;
using System.Net.Mail;
using System.Collections.Generic;

namespace Astove.BlurAdmin.Web.Controllers
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



        private void SendEMail(string email, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.EnableSsl = true;
            //client.Host = "smtp.gmail.com";
            //client.Port = 587;

            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("leandro@AInBox.com.br", "Senha@leandro7");
            //client.UseDefaultCredentials = false;
            //client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("leandro@ainbox.com.br");
            msg.To.Add(new MailAddress(email));

            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;

            client.Send(msg);
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Unauthorized(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("login_two_columns");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [AstoveValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginBindingModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                    .Select(m => m.ErrorMessage).ToArray()
                });
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Json(new
                    {
                        success = true,
                        returnUrl = returnUrl
                    });
                case SignInStatus.LockedOut:
                    return Json(new
                    {
                        success = false,
                        lockedOut = true
                    });
                case SignInStatus.RequiresVerification:
                    return Json(new
                    {
                        success = false,
                        sendCode = true,
                        returnUrl = returnUrl,
                        rememberMe = false
                    });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "O usuário ou a senha informados está incorreto.");
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                        .Select(m => m.ErrorMessage).ToArray()
                    });
            }
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("forgot_password");
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [AstoveValidateAntiForgeryToken]
        public async Task<JsonResult> ForgotPassword(string UserName)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(UserName);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Json(new
                    {
                        success = false,
                        forgotPasswordConfirmation = true
                    });
                }

                //For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Redefinição da Senha", "<b>Se você esqueceu a sua senha, e solicitou a sua redefinição, por favor clique no endereço abaixo:</b><br/> <a href=\"" + callbackUrl + "\">here</a>");
                return Json(new { success = true });
            }

            // If we got this far, something failed, redisplay form
            return Json(new
            {
                success = false,
                errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                    .Select(m => m.ErrorMessage).ToArray()
            });
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult LockScreen()
        {
            return View("lockscreen");
        }

        #region Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("register");
        }

        [HttpPost]
        [AstoveValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<JsonResult> Register(AInBox.Astove.Core.Model.RegisterBindingModel model)
        {
            IdentityResult result = null;

            if (!ModelState.IsValid)
            {
                return Json(new {
                    success = false,
                    errors = ErrorMessage(ModelState)
                });
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return Json(new
                {
                    success = true
                });
            }
            AddErrors(result);

            //// If we got this far, something failed, redisplay form
            return Json(new
            {
                success = false,
                errors = result.Errors.ToArray()
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetRegisterModel()
        {
            var model = new AInBox.Astove.Core.Model.RegisterBindingModel();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public List<string> Errors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }

        [NonAction]
        public List<string> Errors(Exception ex)
        {
            var errors = new List<string>();
            errors.Add(ex.Message);
            if (ex.InnerException != null)
                errors.Add(ex.InnerException.Message);

            return errors;
        }

        [NonAction]
        public string ErrorMessage(ModelStateDictionary modelState)
        {
            var errors = Errors(modelState);
            return string.Join(", ", errors.ToArray());
        }

        [NonAction]
        public string ErrorMessage(Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null)
                error = ex.InnerException.Message;

            return error;
        }

        #endregion
    }
}