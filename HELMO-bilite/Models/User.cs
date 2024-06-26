using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HELMO_bilite.Models;

public class User : IdentityUser
{
    public User()
    {
        PicturePath ??= PictureService.DefaultImageUser;
    }
    
    [MaxLength(500)]
    public string? PicturePath { get; set; }
   
}