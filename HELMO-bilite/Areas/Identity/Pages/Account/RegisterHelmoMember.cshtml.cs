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
using HELMO_bilite.Controllers;

namespace HELMO_bilite.Areas.Identity.Pages.Account
{
    public class RegisterDispatcher : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterDispatcher> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterDispatcher(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterDispatcher> logger,
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
            [Required(ErrorMessage = "Adresse email obligatoire")]
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
            
            
            [Required(ErrorMessage = "Nom obligatoire")]
            [Display(Name = " Prénom")]
            [StringLength(100, ErrorMessage = "Le prénom ne doit pas dépasser 100 caractères.")]
            [RegularExpression(@"^[a-zA-ZÀ-ÿ'\-.\s]+$", ErrorMessage = "Le prénom doit contenir uniquement des lettres, des espaces, des apostrophes, des traits d'union et des points.")]
            public string FirstName { get; set; }
            
            [Required(ErrorMessage = "Prénom obligatoire")]
            [Display(Name = " Nom")]
            [StringLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
            [RegularExpression(@"^[a-zA-ZÀ-ÿ'\-.\s]+$", ErrorMessage = "Le nom doit contenir uniquement des lettres, des espaces, des apostrophes, des traits d'union et des points.")]
            public string LastName { get; set; }
            
            [Required(ErrorMessage = "Matricule obligatoire")]
            [Display(Name = " Matricule")]
            [RegularExpression(@"^[A-Za-z][0-9]{6}$", ErrorMessage = "Le matricule doit être une lettre suivie de 6 chiffres.")]
            public string Matricule { get; set; }
            
   
            [CustomValidation(typeof(InputModel), "ValidatePermis")]
            [Display(Name = "PermisB")]
            public bool PermisB { get; set; }

           
            [Display(Name = "PermisC")]
            public bool PermisC { get; set; }

            
            [Display(Name = "PermisCE")]
            public bool PermisCE { get; set; }
            
            public static ValidationResult ValidatePermis(object value, ValidationContext context)
            {
                var model = context.ObjectInstance as InputModel;
                if (model == null)
                {
                    return new ValidationResult("Erreur lors de l'enregistrement.");
                }

                if (model.PermisB  || model.PermisC  || model.PermisCE )
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Sélectionnez au moins un type de permis.");
            }
            
            
          [CustomValidation(typeof(InputModel), "ValidateLevelOfStudy")]
            [Display(Name = "Niveau d'étude")]
            public string LevelOfStudy { get; set; }
            
            public static ValidationResult ValidateLevelOfStudy(object value, ValidationContext context)
            {
                var model = context.ObjectInstance as InputModel;

                if (model.PermisB && !model.PermisC && !model.PermisCE && string.IsNullOrEmpty(model.LevelOfStudy))
                {
                    return new ValidationResult("Le niveau d'études est obligatoire en tant que dispatcher");
                }

                return ValidationResult.Success;
            }
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
                Dispatcher dispatcherConfirm = null;
                MemberPerson driverConfirm = null;
                
                    
                //CREATION DISPATCHER 
                if (Input.PermisB && !Input.PermisC && !Input.PermisCE)
                {
                    var levelOfStudy = StudyLevel.CESS;
                    
                    //VERIFICATION DU NIVEAU D'ETUDE
                    if (Input.LevelOfStudy.Equals("Bachelier"))
                    {
                        levelOfStudy = StudyLevel.Bachelier;
                    }else if (Input.LevelOfStudy.Equals("Licencier"))
                    {
                        levelOfStudy = StudyLevel.Licencier;
                    }
                    
                    var dispatcher = new Dispatcher()
                    {
                        LastName = Input.LastName,
                        FirstName = Input.FirstName, 
                        Matricule = Input.Matricule,
                        Permis = TypeDrivingLicences.B,
                        StudyLevel = levelOfStudy
                    };

                    dispatcherConfirm = dispatcher;
                    dispatcherConfirm.UserName = Input.Email;
                }
                //CREATION MEMBRE 
                else
                {
                    bool input = Input.PermisCE;
                    
                    TypeDrivingLicences permis = TypeDrivingLicences.C;
                    
                    //VERIFICATION DES PERMIS
                    if (Input.PermisCE)
                    {
                        permis = TypeDrivingLicences.CE;
                    }

                    var truckDriver = new TruckDriver()
                    {
                        LastName = Input.LastName,
                        FirstName = Input.FirstName, 
                        Matricule = Input.Matricule,
                        Permis = permis
                    };
                    
                    driverConfirm = truckDriver;
                    driverConfirm.UserName = Input.Email;
                }
             

                await _userStore.SetUserNameAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    string role = null;
                    if (dispatcherConfirm != null)
                    {
                        var result2 = await _userManager.AddToRoleAsync(dispatcherConfirm, "Dispatcher");
                        role = "Dispatcher";
                    }
                    
                    if (driverConfirm != null)
                    {
                        var result2 = await _userManager.AddToRoleAsync(driverConfirm, "TruckDriver");
                        role = "TruckDriver";
                    }
                    
                    var userId = await _userManager.GetUserIdAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    
                        await _signInManager.SignInAsync(dispatcherConfirm == null ? driverConfirm : dispatcherConfirm, isPersistent: false);
                        // Rediriger en fonction du rôle
                        if(role == "Dispatcher") 
                        {
                            return RedirectToAction("Index", "Dispatcher");
                        }

                        if(role == "TruckDriver") 
                        {
                            return RedirectToAction("Index", "TruckDriver");
                        }

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
