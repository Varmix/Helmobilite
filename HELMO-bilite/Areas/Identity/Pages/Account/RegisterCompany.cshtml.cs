// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using HELMO_bilite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HELMO_bilite.Areas.Identity.Pages.Account
{
    public class RegisterCompany : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterCompany> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterCompany(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterCompany> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "Minimum 6 caractères avec au minimum un caractère spéciale + deux chiffres", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required(ErrorMessage = "Nom de l'entreprise obligatoire")]
            [Display(Name = "Entreprise")]
            [MaxLength(50,  ErrorMessage = "Maximum 50 caractères pour le nom de la compagnie")]
            public string CompanyName { get; set; }
            
            [Required(ErrorMessage = "Numéro de l'entreprise obligatoire")]
            [Display(Name = "Numéro de l'entreprise")]
            [MaxLength(20,  ErrorMessage = "Maximum 25 caractères pour le numéro de la compagnie")]
            public string NumberCompany { get; set; }
            
            [Required(ErrorMessage = "Rue de l'entreprise obligatoire")]
            [MaxLength(50,  ErrorMessage = "Maximum 50 caractères pour la rue de la compagnie")]
            [Display(Name = "Rue")]
            public string Street { get; set; }
            
            [Required(ErrorMessage = "Numéro de la rue obligatoire")]
            [MaxLength(5, ErrorMessage = "Le numéro ne doit pas dépasser 5 caractères.")]
            [Display(Name = "Numéro")]
            public string Number { get; set; }
            
            [Required(ErrorMessage = "Localité de l'entreprise obligatoire")]
            [Display(Name = "Localité")]
            [MaxLength(50, ErrorMessage = "La localité ne doit pas dépasser 50 caractères.")]
            public string Locality { get; set; }
            
            [Required(ErrorMessage = "Code postal de l'entreprise obligatoire")]
            [Display(Name = "Code postal")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Le code postal ne doit être constitué que de chiffres !")]
            public int PostalCode { get; set; }
            
            [Required(ErrorMessage = "Pays de l'entreprise obligatoire")]
            [Display(Name = "Pays")]
            [MaxLength(50, ErrorMessage = "Le pays ne doit pas dépasser 50 caractères.")]
            public string Country { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var adressCompany = new CompanyAdress
                {
                    Street = Input.Street,
                    Number = Input.Number,
                    Locality = Input.Locality,
                    PostalCode = Input.PostalCode,
                    Coutry = Input.Country
                };

                var company = new Company
                {
                    NumberCompany = Input.NumberCompany,
                    CompanyName = Input.CompanyName,
                    CompanyAdress = adressCompany
                    
                };
                
                
                var user = new Client(company);
                var userName = Input.Email;
                user.UserName = userName;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var result2 = await _userManager.AddToRoleAsync(user, "Client");
                    
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return RedirectToAction("ShowDeliveries", "Client", new { status = "waiting" });
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
