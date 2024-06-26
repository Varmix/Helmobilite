using Microsoft.AspNetCore.Identity;

namespace HELMO_bilite.Models;

public class Client : User
{
    
    public Client() {}
    public Client(Company company)
    {
        ClientCompany = company;
    }
    
    
    // Référence vers l'entreprise du client (1 entreprise pour un client)
    public Company ClientCompany { get; set; }
    public bool IsBadPayer { get; set;}
}