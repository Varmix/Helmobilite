using System.Globalization;
using HELMO_bilite.Models;
using HELMO_bilite.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "TruckDriver")]
public class TruckDriverController : Controller
{
    
    private readonly HelmoBiliteDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public TruckDriverController([FromServices] HelmoBiliteDbContext context, [FromServices] UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    } 

    // GET
    public async Task<IActionResult> Index(int? weekOffset)
    {
        var user = await _userManager.GetUserAsync(User);
        var truckDriver = await _context.TruckDrivers
            .Include(t => t.Deliveries)
            .FirstOrDefaultAsync(t => t.Id == user.Id);
        
        if (truckDriver == null)
        {
            return NotFound();
        }
        int weekOffsetValue = weekOffset ?? 0;
        ViewBag.WeekOffset = weekOffsetValue;
        (DateTime startOfWeek, DateTime endOfWeek) = DateManager.GetWeekRange(weekOffsetValue);
        ViewBag.StartOfWeek = startOfWeek;
        ViewBag.EndOfWeek = endOfWeek;
        var deliveriesByWeek = GetDeliveriesByWeek(truckDriver, weekOffset ?? 0);
        return View(deliveriesByWeek);
    }
    
    private List<IGrouping<int, DeliveryModel>> GetDeliveriesByWeek(TruckDriver driver, int weekOffset)
    {
        
        var startOfWeek = DateManager.GetWeekRange(weekOffset).Start;
        var endOfWeek = DateManager.GetWeekRange(weekOffset).End;

        // Filtrer les livraisons dont la date de chargement est compris entre le début et la fin de la semaine en cours
        var deliveriesByWeek = driver.Deliveries
                // date de déchargement de la livraison est compris entre le début et la fin de la semaine en cours
            .Where(d => d.IsFinish == false && d.DateAndTimeOfUnLoading.Date >= startOfWeek && d.DateAndTimeOfUnLoading.Date <= endOfWeek)
            .OrderBy(d => d.DateAndTimeOfLoading)
            .GroupBy(d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d.DateAndTimeOfLoading, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            .ToList();
        // Utilisation de la méthode GetWeekOfYear disponible dans la Classe 'Calendar' qui va calculer le numéro de la semaine pour chaque livraison
        // en se basant sur la date de chargement, en considérant le premier jour de la semaine comme étant le lundi

        return deliveriesByWeek;
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessDelivery(int IdDelivery, string Comment, bool IsSuccess, int currentWeekOffset)
    {
        if (string.IsNullOrWhiteSpace(Comment))
        {
            TempData["ModalError"] = "Veuillez entrer un commentaire.";
            TempData["ModalId"] = IsSuccess ? "completeDeliveryModal" : "incompleteDeliveryModal";
            return RedirectToAction("Index", new { weekOffset = currentWeekOffset });
        }
        var delivery = await _context.Delivery.FindAsync(IdDelivery);
        if (delivery == null)
        {
            return NotFound();
        }
        // Considérer la livraison comme terminée
        delivery.IsFinish = true;
        delivery.IsSucces = IsSuccess;
        // Ajout du commentaire de fin de livraison
        delivery.Comment = Comment;
        await _context.SaveChangesAsync();
        TempData["IsSuccess"] = IsSuccess;
        TempData["NotificationMessage"] =
            IsSuccess ? "La livraison a été validée avec succès." : "La livraison n'a pas pu être terminée.";

        return RedirectToAction("Index", new { weekOffset = currentWeekOffset });
    }

    



}