// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HELMO_bilite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HelmoBiliteDbContext _context;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager, [FromServices] HelmoBiliteDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

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
            // [Phone]
            // [Display(Name = "Phone number")]
            // public string PhoneNumber { get; set; }
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Entreprise")]
            public string CompanyName { get; set; }
            
            [Required]
            [Display(Name = "Numéro de l'entreprise")]
            public string NumberCompany { get; set; }
            
            [Required]
            // Dans les cas où on écrit pas en dur le texte du label
            [Display(Name = "Rue")]
            public string Street { get; set; }
            
               
            [Required]
            [Display(Name = "Numéro")]
            public string Number { get; set; }
            
            [Required]
            [Display(Name = "Localité")]
            public string Locality { get; set; }
            
            [Required]
            [Display(Name = "Code postal")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Le code postal ne doit être constitué que de chiffres !")]
            public int PostalCode { get; set; }
            
            [Required]
            [Display(Name = "Pays")]
            public string Country { get; set; }
            
        }

        /// <summary>
        /// Cette méthode va permettre de préremplir les informations de la société du client
        /// </summary>
        /// <param name="user">Le client connecté</param>
        private async Task LoadAsync(User user)
        { 
            
            var userName = await _userManager.GetUserNameAsync(user);

          // Vérifier si l'utilisateur est un client
           var client = user as Client;
          //  if (client == null)
          //  {
          //      throw new InvalidOperationException($"User {user.Id} is not a client.");
          //  }
          //  
          //  // Récupérer la compagnie du client
          // var company =  _context.Companies.Find(client.ClientCompany.IdCompany);
          //  
          //  if (company == null)
          //  {
          //      throw new InvalidOperationException($"Could not find company {client.ClientCompany.IdCompany} for client {client.Id}.");
          //  }
          //  
          //  // Récupérer l'adresse de la compagnie
          //  var companyAddress = await _context.CompanyAddresses.FindAsync(company.CompanyAdress.IdCompanyAdress);
          //  
          //  if (companyAddress == null)
          //  {
          //      throw new InvalidOperationException($"Could not find address {company.CompanyAdress.IdCompanyAdress} for company {company.IdCompany}.");
          //  }
          //  
          //  // Mettre à jour les propriétés du client avec la compagnie et l'adresse
          //  client.ClientCompany = company;
          // client.ClientCompany.CompanyAdress = companyAddress;
          //
          //  // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
          //   if (client != null)
          //   {
          //       Username = userName;
          //       Input = new InputModel
          //       {
          //           CompanyName = client.ClientCompany.CompanyName,
          //           NumberCompany = client.ClientCompany.NumberCompany,
          //           Street = client.ClientCompany.CompanyAdress.Street,
          //           Number = client.ClientCompany.CompanyAdress.Number,
          //           Locality = client.ClientCompany.CompanyAdress.Locality,
          //           PostalCode = client.ClientCompany.CompanyAdress.PostalCode,
          //           Country = client.ClientCompany.CompanyAdress.Coutry
          //       };
          //   }
            // else
            // {
                // Si l'utilisateur n'est pas un client, il n'aurait qu'un son information la plus basique, à savoir
                // son adresse email
            //     Username = userName;
            //
            //     Input = new InputModel
            //     {
            //         Email = user.Email
            //     };
            //     Input = new InputModel();
            // }
            Username = userName;

            Input = new InputModel
            {
                Email = user.Email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            // if (Input.PhoneNumber != phoneNumber)
            // {
            //     var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //     if (!setPhoneResult.Succeeded)
            //     {
            //         StatusMessage = "Unexpected error when trying to set phone number.";
            //         return RedirectToPage();
            //     }
            // }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
