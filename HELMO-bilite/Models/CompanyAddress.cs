using System.ComponentModel.DataAnnotations;

namespace HELMO_bilite.Models;

public class CompanyAdress
{
    [Key]
    public int IdCompanyAdress { get; set; }
    
    [Required(ErrorMessage = "La rue est requise.")]
    [MaxLength(50, ErrorMessage = "La rue ne doit pas dépasser 100 caractères.")]
    public string Street { get; set; }
    
    [Required(ErrorMessage = "Le numéro est requis.")]
    [MaxLength(5, ErrorMessage = "Le numéro ne doit pas dépasser 5 caractères.")]
    public string Number { get; set; }
    
    [Required(ErrorMessage = "La localité est requise.")]
    [MaxLength(50, ErrorMessage = "La localité ne doit pas dépasser 50 caractères.")]
    public string Locality { get; set; }
    
    [Required(ErrorMessage = "Le code postal est requis.")]
    [DataType(DataType.PostalCode)]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Le code postal ne doit être constitué que de chiffres !")]
    public int PostalCode { get; set; }
    
    [Required(ErrorMessage = "Le pays est requis.")]
    [MaxLength(50, ErrorMessage = "Le pays ne doit pas dépasser 50 caractères.")]
    public string Coutry { get; set; }
    
    public CompanyAdress () {}
}