using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HELMO_bilite.Models;

public abstract class MemberPerson : User
{
    public MemberPerson() {}
    
    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ'\-.\s]+$", ErrorMessage = "Le nom doit contenir uniquement des lettres, des espaces, des apostrophes, des traits d'union et des points.")]
    [Display(Name = "Nom")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Le prénom est requis.")]
    [StringLength(100, ErrorMessage = "Le prénom ne doit pas dépasser 100 caractères.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ'\-.\s]+$", ErrorMessage = "Le prénom doit contenir uniquement des lettres, des espaces, des apostrophes, des traits d'union et des points.")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Le matricule est requis.")]
    [RegularExpression(@"^[A-Za-z][0-9]{6}$", ErrorMessage = "Le matricule doit être une lettre suivie de 6 chiffres.")]
    [Display(Name = "Matricule")]
    public string Matricule { get; set; }
    
    [Required(ErrorMessage = "Un permis doit être précisé.")]
    public TypeDrivingLicences Permis { get; set; }
    
    [DataType(DataType.DateTime, ErrorMessage = "La date de naissance est invalide. Elle doit respecter le format dd/mm/yyyy.")]
    public DateTime? BirthDate { get; set; }
    
    public static TypeDrivingLicences GiveDrivingLicenceType(string drinvingLicence)
    {
        switch (drinvingLicence)
        {
            case "B" :
                return TypeDrivingLicences.B;
            case "C" :
                return TypeDrivingLicences.C;
            case "CE":
                return TypeDrivingLicences.CE;
            default: 
                return TypeDrivingLicences.Empty;
        }
    }
}