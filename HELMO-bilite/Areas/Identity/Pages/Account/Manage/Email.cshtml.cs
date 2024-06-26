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
    public class EditClientModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly HelmoBiliteDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EditClientModel(
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
            
            // Informations concernant le le client
            
            [EmailAddress]
            [Display(Name = "Nouvelle adresse email")]
            public string NewEmail { get; set; }
            
            
            // Informations concernant l'entreprise
            
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
            
            // Permettre de récupérer l'image envoyé par une requête HTTP
            // afin de l'enregistrer côté serveur
            [Display(Name = "Logo de l'entreprise")]
            public IFormFile UploadedPicture { get; set; }
            
            // Le chemin de l'image enregistré en base de données.
            // Ce dernier permettra la possibilité d'afficher
            // l'image à l'utilisateur à tout moment
            public string PictureUrl { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            // Récupérer le client
            var client = await _context.Clients
                .Include(c => c.ClientCompany)
                .ThenInclude(cc => cc.CompanyAdress)
                .FirstOrDefaultAsync(c => c.Id == user.Id);

            if (client == null)
            {
                throw new InvalidOperationException($"User {user.Id} is not a client.");
            }

            // Récupérer la compagnie du client
            var company = client.ClientCompany;

            if (company == null)
            {
                throw new InvalidOperationException($"Could not find company for client {client.Id}.");
            }

            // Récupérer l'adresse de la compagnie
            var companyAddress = company.CompanyAdress;

            if (companyAddress == null)
            {
                throw new InvalidOperationException($"Could not find address for company {company.IdCompany}.");
            }
            

            // Stocker les propriétés de l'objet Company, CompanyAddress et Client dans les propriétés de la classe EditClientModel
            Input = new InputModel
            {
                CompanyName = company.CompanyName,
                NumberCompany = company.NumberCompany,
                Street = companyAddress.Street,
                Number = companyAddress.Number,
                Locality = companyAddress.Locality,
                PostalCode = companyAddress.PostalCode,
                Country = companyAddress.Coutry,
                PictureUrl = client.PicturePath
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossible de charger l'utilisateur avec L'ID '{_userManager.GetUserId(User)}'.");
            }
            
            await LoadAsync(user);
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
                await LoadAsync(user);
                return Page();
            }
            
            // Récupérer le client correspondant à l'utilisateur courant
            var client = await _context.Clients
                .Include(c => c.ClientCompany)
                .ThenInclude(cc => cc.CompanyAdress)
                .FirstOrDefaultAsync(c => c.Id == user.Id);

            if (client == null)
            {
                throw new InvalidOperationException($"User {user.Id} is not a client.");
            }

            // Mettre à jour les propriétés du client et de ses entités liées
            client.ClientCompany.CompanyName = Input.CompanyName;
            client.ClientCompany.NumberCompany = Input.NumberCompany;
            client.ClientCompany.CompanyAdress.Coutry = Input.Country;
            client.ClientCompany.CompanyAdress.Locality = Input.Locality;
            client.ClientCompany.CompanyAdress.Number = Input.Number;
            client.ClientCompany.CompanyAdress.PostalCode = Input.PostalCode;
            client.ClientCompany.CompanyAdress.Street = Input.Street;
            
            // Images
            if (Input.UploadedPicture != null)
            {
                var pictureUploaded = new PictureManager(_env);
                string relativePicturePath = null;
                try
                {
                    relativePicturePath = await pictureUploaded.UploadAsync(Input.UploadedPicture, user.UserName);
                }
                catch (HelmobiliteStorageException e)
                {
                    ModelState.AddModelError(string.Empty, $"{e.Message} Votre ancienne image a été conservée.");
                    // Rebind les informations nécessaires en cas d'erreur
                    Email = client.Email;
                    Input.PictureUrl = client.PicturePath;
                    return Page();
                }

                if (!string.IsNullOrEmpty(relativePicturePath))
                {
                    client.PicturePath = relativePicturePath;
                }
                
            }
            
            // Enregistrer les modifications dans la base de données
            await _context.SaveChangesAsync();
            //ou await _userManager.UpdateAsync(user);

            

            StatusMessage = "Vos informations ont été mises à jour.";
            return RedirectToPage();
        }
        
    }
}
