using System.ComponentModel.DataAnnotations;

namespace HELMO_bilite.Models;

public class Dispatcher : MemberPerson
{
    public Dispatcher() {}
    
    [Required(ErrorMessage = "Le niveau d'étude est requis.")]
    public StudyLevel StudyLevel {get; set;}
}