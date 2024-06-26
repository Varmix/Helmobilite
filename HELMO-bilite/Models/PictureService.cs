using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace HELMO_bilite.Models;

public class PictureService : IPictureService
{
    private readonly UserManager<User> _userManager;
    public static readonly string DefaultImageUser = "~/mylib/other/profile.png";
    public static readonly string DefaultTruckUser = "~/mylib/trucks/default-truck.png";

    public PictureService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetUserImageUrlAsync(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(user);
        if (currentUser == null)
        {
            return DefaultImageUser;
        }
        return currentUser.PicturePath ?? DefaultImageUser;
    }

    public static string GetTruckImageUrlAsync(TruckDriver truckDriver)
    {
        return truckDriver.PicturePath ?? DefaultTruckUser;
    }
}