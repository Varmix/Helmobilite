using System.Security.Claims;

namespace HELMO_bilite.Models;

public interface IPictureService
{
    Task<string> GetUserImageUrlAsync(ClaimsPrincipal user);
    
}