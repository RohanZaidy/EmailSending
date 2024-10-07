using IdentityFramework.Repositories.Interface;
using IdentityFramework.ViewModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityFramework.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        // for email use this
        private readonly IEmailSender emailsender;

        //Our email template in wwwroot folder to get it use Iwebhostenvironment
        private readonly IWebHostEnvironment webHostEnvironment;

        // our url for login (when email sent login button in email when we click then redirect to login page)
        // is in app setting.json to get it use iconfiguration
        private readonly IConfiguration configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailsender, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailsender = emailsender;
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Register(RegisterVM model)
		{
            try
            {
                if(ModelState.IsValid)
                {
                    var chkEmail = await userManager.FindByEmailAsync(model.Email);
                    if(chkEmail != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email already exists");
                        return View(model);
                    }
                    var user = new IdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                    };
                    var result = await userManager.CreateAsync(user,model.Password);
                    if(result.Succeeded)
                    {
                        //Also add this to emailsending function in EmailSender Class
                        //    BodyEncoding = System.Text.Encoding.ASCII,
                        //    IsBodyHtml = true

                        //string path = Path.Combine(webHostEnvironment.WebRootPath, "Template/Welcome.cshtml");
                        //string htmlString = System.IO.File.ReadAllText(path);
                        //htmlString = htmlString.Replace("{{title}}", "User Registration");
                        //htmlString = htmlString.Replace("{{username}}", model.Email);
                        //htmlString = htmlString.Replace("{{url}}", "https://localhost:7127/Account/Login");

                        // Instead we make function and call it in place of emailBody (Function is at end of this page)


                       bool status = await emailsender.EmailSendAsync(model.Email, "Account Created", await GetEmailBody(model.Email));
                       await signInManager.SignInAsync(user, isPersistent: false);
                       return RedirectToAction("Index", "Home");
                    }
                    if(result.Errors.Count() > 0)
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }


            }catch(Exception)
            {
                throw;
            }
			return View(model);
		}

		public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Login(LoginVM model)
		{
            try
            {
                if(ModelState.IsValid)
                {
                    IdentityUser chkEmail = await userManager.FindByEmailAsync(model.Email);
                    if(chkEmail == null)
                    {
						ModelState.AddModelError(string.Empty, "Email not exists");
						return View(model);
					}
                    if(await userManager.CheckPasswordAsync(chkEmail,model.Password) == false)
                    {
						ModelState.AddModelError(string.Empty, "Invalid Credentilas");
						return View(model);
					}
                    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
					if (result.Succeeded)
					{
                        return RedirectToAction("Index", "Home");
					}
					ModelState.AddModelError(string.Empty, "Invalid Login attempt");
				}


            }catch(Exception)
            {
                throw;
            }

			return View(model);
		}

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Method for getting Email Template
        public async Task<string> GetEmailBody(string username)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Template/Welcome.cshtml");
            string htmlString = System.IO.File.ReadAllText(path);
            htmlString = htmlString.Replace("{{title}}", "User Registration");
            htmlString = htmlString.Replace("{{username}}", username);
            // taking url from appsettings.json 
            string url = configuration.GetValue<string>("urls:LoginUrl") ?? string.Empty;
            htmlString = htmlString.Replace("{{url}}", url);

            return htmlString;
        }
	}
}