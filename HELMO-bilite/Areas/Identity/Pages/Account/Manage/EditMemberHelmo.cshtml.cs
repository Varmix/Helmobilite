// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HELMO_bilite.Models;
using HELMO_bilite.Models.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Areas.Identity.Pages.Account.Manage
{
    public class EditMemberHelmoModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly HelmoBiliteDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditMemberHelmoModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender, [FromServices] HelmoBiliteDbContext context,
            [FromServices] IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
            _env = env;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Adresse email")]
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            
            [EmailAddress]
            [Display(Name = "Nouvelle adresse email")]
            public string NewEmail { get; set; }
            
            
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
            
            [Required]
            [Display(Name = " Permis")]
            public TypeDrivingLicences DrivingLicence { get; set; }
            
               
            [CustomValidation(typeof(InputModel), "ValidateLevelOfStudy")]
            [Display(Name = "Niveau d'étude")]
            public string LevelOfStudy { get; set; }
            
            public static ValidationResult ValidateLevelOfStudy(object value, ValidationContext context)
            {
                var model = context.ObjectInstance as InputModel;

                // Si le permis de conduire est de type B, c'est un dispatcher
                if (model != null && model.LevelOfStudy != null && model.DrivingLicence == TypeDrivingLicences.B && string.IsNullOrEmpty(model.LevelOfStudy))
                {
                    return new ValidationResult("Le niveau d'étude est obligatoire en tant que dispatcher");
                }

                return ValidationResult.Success;
            }

        
            [CustomValidation(typeof(InputModel), "ValidateBirthDate")]
            [Display(Name = "Date de naissance")]
            public DateTime? BirthDate { get; set; }
            
            public static ValidationResult ValidateBirthDate(object value, ValidationContext context)
            {
                var model = context.ObjectInstance as InputModel;

                if (model != null && model.BirthDate.HasValue && model.BirthDate.Value.AddYears(18) > DateTime.Today)
                {
                    return new ValidationResult("Vous devez avoir au moins 18 ans.");
                }

                return ValidationResult.Success;
            }

            // Permettre de récupérer l'image envoyé par une requête HTTP
            // afin de l'enregistrer côté serveur
            [Display(Name = "Logo de l'entreprise")]
            public IFormFile UploadedPicture { get; set; }
            
            // Le chemin de l'image enregistré en base de données.
            // Ce dernier permettra la possibilité d'afficher
            // l'image à l'utilisateur à tout moment
            public string PictureUrl { get; set; }
        }

        private async Task LoadAsyncDispatcher(User dispatcher)
        {
            var email = await _userManager.GetEmailAsync(dispatcher);
            Email = email;

            // Récupérer le chauffeur ou le dispatcher en fonction du rôle
            Dispatcher currentDispatcher = await _context.Dispatcher.FirstOrDefaultAsync(ds => ds.Id == dispatcher.Id);


            if (currentDispatcher == null)
            {
                throw new InvalidOperationException($"L'utilisateur {dispatcher.Id} n'est pas un dispatcher.");
            }


            // Stocker les propriétés de l'objet Company, CompanyAddress et Client dans les propriétés de la classe EditClientModel
            Input = new InputModel
            {
                FirstName = currentDispatcher.FirstName,
                LastName = currentDispatcher.LastName,
                Matricule = currentDispatcher.Matricule,
                LevelOfStudy = currentDispatcher.StudyLevel.ToString(),
                DrivingLicence = currentDispatcher.Permis,
                PictureUrl = currentDispatcher.PicturePath,
                BirthDate = currentDispatcher.BirthDate
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(dispatcher);
        }
        
        private async Task LoadAsyncTruckDriver(User dispatcher)
        {
            var email = await _userManager.GetEmailAsync(dispatcher);
            Email = email;

            // Récupérer le chauffeur ou le dispatcher en fonction du rôle
            TruckDriver currentTruckDriver = await _context.TruckDrivers.FirstOrDefaultAsync(td => td.Id == dispatcher.Id);

            if (currentTruckDriver == null)
            {
                throw new InvalidOperationException($"L'utilisateur {dispatcher.Id} n'est pas un chauffeur.");
            }


            // Stocker les propriétés de l'objet Company, CompanyAddress et Client dans les propriétés de la classe EditClientModel
            Input = new InputModel
            {
                FirstName = currentTruckDriver.FirstName,
                LastName = currentTruckDriver.LastName,
                Matricule = currentTruckDriver.Matricule,
                LevelOfStudy = null,
                DrivingLicence = currentTruckDriver.Permis,
                PictureUrl = currentTruckDriver.PicturePath,
                BirthDate = currentTruckDriver.BirthDate
                    
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(dispatcher);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec L'ID '{_userManager.GetUserId(User)}'.");
            }

            if (User.IsInRole("Dispatcher"))
            {
                await LoadAsyncDispatcher(user);
            }

            if (User.IsInRole("TruckDriver"))
            {
                await LoadAsyncTruckDriver(user);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                if (User.IsInRole("Dispatcher"))
                {
                    await LoadAsyncDispatcher(user);
                }

                if (User.IsInRole("TruckDriver"))
                {
                      await LoadAsyncTruckDriver(user);
                }
                return Page();
            }
            
            // Récupérer le client correspondant à l'utilisateur courant
            if (User.IsInRole("Dispatcher"))
            {
                Dispatcher currentDispatcher = await _context.Dispatcher.FirstOrDefaultAsync(ds => ds.Id == user.Id);
                
                if (currentDispatcher == null)
                {
                    throw new InvalidOperationException($"L'utilisateur {user.Id} n'est pas un dispatcher.");
                }
                
                var levelOfStudy = StudyLevel.CESS;
                
                if (Input.LevelOfStudy.Equals("Bachelier"))
                {
                    levelOfStudy = StudyLevel.Bachelier;
                } else if (Input.LevelOfStudy.Equals("Licencier"))
                {
                    levelOfStudy = StudyLevel.Licencier;
                }

                if (Input.BirthDate != null)
                {
                    currentDispatcher.BirthDate = Input.BirthDate;
                }
                
                //Mettre à jour les propriétés
                currentDispatcher.Matricule = Input.Matricule;
                currentDispatcher.FirstName = Input.FirstName;
                currentDispatcher.LastName = Input.LastName;
                currentDispatcher.StudyLevel = levelOfStudy;
                
                // Images
                string relativePicturePath = null;
                try
                {
                    relativePicturePath = await UpdatePictureAsync(Input.UploadedPicture, user.UserName, currentDispatcher.PicturePath);
                }
                catch (InvalidOperationException e)
                {
                    ModelState.AddModelError(string.Empty, $"{e.Message} Votre ancienne image a été conservée.");
                    Email = currentDispatcher.Email;
                    Input.PictureUrl = currentDispatcher.PicturePath;
                    return Page();
                }

                if (!string.IsNullOrEmpty(relativePicturePath))
                {
                    currentDispatcher.PicturePath = relativePicturePath;
                }
                
            }

            if (User.IsInRole("TruckDriver"))
            {
                // Récupérer le chauffeur ou le dispatcher en fonction du rôle
                TruckDriver currentTruckDriver = await _context.TruckDrivers.FirstOrDefaultAsync(td => td.Id == user.Id);

                if (currentTruckDriver == null)
                {
                    throw new InvalidOperationException($"L'utilisateur {user.Id} n'est pas un chauffeur.");
                }
                
                if (Input.BirthDate != null)
                {
                    currentTruckDriver.BirthDate = Input.BirthDate;
                }
                
                //Mettre à jour les propriétés
                currentTruckDriver.Matricule = Input.Matricule;
                currentTruckDriver.FirstName = Input.FirstName;
                currentTruckDriver.LastName = Input.LastName;
                
                // Images
                string relativePicturePath = null;
                try
                {
                    relativePicturePath = await UpdatePictureAsync(Input.UploadedPicture, user.UserName,
                        currentTruckDriver.PicturePath);
                }
                catch (InvalidOperationException e)
                {
                    ModelState.AddModelError(string.Empty, $"{e.Message} Votre ancienne image a été conservée.");
                    Email = currentTruckDriver.Email;
                    Input.PictureUrl = currentTruckDriver.PicturePath;
                    return Page();
                }

                if (!string.IsNullOrEmpty(relativePicturePath))
                {
                    currentTruckDriver.PicturePath = relativePicturePath;
                }
            }


            // Mettre à jour les propriétés du dispatcher ou chauffeur
            
            
            // Enregistrer les modifications dans la base de données
            await _context.SaveChangesAsync();
            //ou await _userManager.UpdateAsync(user);
            

            StatusMessage = "Vos informations ont été mises à jour.";
            return RedirectToPage();
        }
        
        private async Task<string> UpdatePictureAsync(IFormFile uploadedPicture, string userName, string currentPicturePath)
        {
            if (uploadedPicture == null) return currentPicturePath;

            var pictureUploaded = new PictureManager(_env);
            string relativePicturePath;
            try
            {
                relativePicturePath = await pictureUploaded.UploadAsync(uploadedPicture, userName);
            }
            catch (HelmobiliteStorageException e)
            {
                throw new InvalidOperationException(e.Message);
            }

            return relativePicturePath;
        }
        
    }
}
