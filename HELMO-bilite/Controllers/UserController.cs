using HELMO_bilite.Models;
using HELMO_bilite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "Admin, TruckDriver, Dispatcher, Client")]
public class UserController : Controller
{

    private readonly IPictureService _pictureService;

    public UserController([FromServices] IPictureService pictureService)
    {
        _pictureService = pictureService;
    }
    
    /// <summary>
    /// Cette méthode permet de récupérer l'image d'un utilisateur
    /// connecté
    /// </summary>
    /// <returns>le chemin de l'image associé à un utilisateur connecté</returns>
    [HttpGet]
    public async Task<IActionResult> GetProfileImage()
    {
        var imageUrl = await _pictureService.GetUserImageUrlAsync(User);
        return Redirect(imageUrl);
    }
}