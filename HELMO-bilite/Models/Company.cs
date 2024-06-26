using System.ComponentModel.DataAnnotations;

namespace HELMO_bilite.Models;

public class Company
{
    public Company(CompanyAdress companyAdress)
    {
        CompanyAdress = companyAdress;
    }
    
    public Company() {}

    [Key]
    public int IdCompany { get; set; }
    
    [Required(ErrorMessage = "Le numéro de l'entreprise est requis.")]
    [MaxLength(20, ErrorMessage = "Le numéro de l'entreprise ne doit pas dépasser 20 caractères.")]
    public string NumberCompany { get; set; }
    
    [Required(ErrorMessage = "Le nom de l'entreprise est requis.")]
    [MaxLength(50, ErrorMessage = "Le nom de l'entreprise ne doit pas dépasser 150 caractères.")]
    public string CompanyName { get; set; }
    // Référence vers l'adresse de la companie (on part toujours d'une entrepreprise pour avoir son adresse)
    public CompanyAdress CompanyAdress { get; set; }

    public  int CompanyAdressIdCompanyAdress { get; set; }
    
    // Référence de la compagnie vers le client (permettra de récupérer la société d'un client pour plus tard)
    public string CompanyOfTheClientId { get; set; }
    
    public Client ClientOfTheCompany { get; set; }
    
}