using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HELMO_bilite.Models;
using HELMO_bilite.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

public class AnonymousController : Controller
{
    private readonly ILogger<AnonymousController> _logger;
    private readonly HelmoBiliteDbContext _context;
    public AnonymousController(ILogger<AnonymousController> logger, HelmoBiliteDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Client"))
        {
            return RedirectToAction("ShowDeliveries", "Client", new { status = "waiting" });
        }
        else if (User.IsInRole("TruckDriver"))
        {
            //CHANGER LA VUE PAR DEFAUT DU TRUCKDRIVER
            return RedirectToAction("Index", "TruckDriver");
        }
        else if (User.IsInRole("Dispatcher"))
        {
            return RedirectToAction("Index", "Dispatcher");
        }
        else if (User.IsInRole("Admin"))
        {
            return RedirectToAction("CrudDrivingLicences", "Admin");
        }

        return View(await GiveHelmoBiliteViewModel());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("Error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<HelmoBiliteViewModel> GiveHelmoBiliteViewModel()
    {
        var memberPersons = await _context.Set<MemberPerson>().ToListAsync();
        var clients = _context.Clients
            .Include(c => c.ClientCompany)
            .ToList();
        
        var trucks = await _context.Set<Truck>().ToListAsync();

        return new HelmoBiliteViewModel(memberPersons, clients, trucks);
    }
}