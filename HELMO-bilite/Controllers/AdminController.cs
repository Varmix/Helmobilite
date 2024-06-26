using HELMO_bilite.Models;
using HELMO_bilite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HelmoBiliteDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public AdminController(HelmoBiliteDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    } 
    
    public IActionResult CrudDrivingLicences()
    {
        // Récupération de tous les TruckDriver en base de données
        List<TruckDriver> truckDrivers = _context.TruckDrivers.ToList();

        // Récupération de tous les Dispatcher en base de données
        List<Dispatcher> dispatchers = _context.Dispatcher.ToList();
        
        var memberPersons = new MemberPersonViewModel(dispatchers, truckDrivers);
        return View("CrudDrivingLicences", memberPersons);
    }
    
    
    public async Task<IActionResult> ProcessChangeDrivingLicenceType()
    {
        TypeDrivingLicences drivingLicenceTypeB = MemberPerson.GiveDrivingLicenceType(Request.Form["drivingLicenceB"]);
        TypeDrivingLicences drivingLicenceTypeC = MemberPerson.GiveDrivingLicenceType(Request.Form["drivingLicenceC"]);
        TypeDrivingLicences drivingLicenceTypeCE = MemberPerson.GiveDrivingLicenceType(Request.Form["drivingLicenceCE"]);
        
        string memberId = Request.Form["memberId"];

        var driver = await _context.TruckDrivers.FindAsync(memberId);

        if (driver != null)
        {
            if (drivingLicenceTypeCE == TypeDrivingLicences.CE)
            {
                driver.Permis = drivingLicenceTypeCE;
            }
            else if (drivingLicenceTypeC == TypeDrivingLicences.C)
            {
                driver.Permis = drivingLicenceTypeC;
            }
            else if (drivingLicenceTypeB == TypeDrivingLicences.B)
            {
                driver.Permis = drivingLicenceTypeB;
                await DeleteAllDeliveries(driver);
            }
            else
            {
                driver.Permis = TypeDrivingLicences.Empty;
                await DeleteAllDeliveries(driver);
            }      
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("CrudDrivingLicences", "Admin");
    }
    
    public Task<IActionResult> ShowAllClients()
    {
        var clients = _context.Clients
            .Include(c => c.ClientCompany)
            .ThenInclude(company => company.CompanyAdress)
            .Where(c => _context.Delivery.Any(d => d.LinkedClient != null && d.LinkedClient.Id == c.Id))
            .ToList();
        
        
        return Task.FromResult<IActionResult>(View("ListClients", clients));
    }

    public async Task<IActionResult> ProcessBadClient()
    {
        string clientId = Request.Form["clientId"];
        var client = await _context.Clients.FindAsync(clientId);
        if (client != null) client.IsBadPayer = true;
        await _context.SaveChangesAsync();
        
        return RedirectToAction("ShowAllClients", "Admin");
    }


    public async Task<IActionResult> ShowStatistics()
    {
        List<TruckDriverStatisticsViewModel> listTruckDriverStatistics= new List<TruckDriverStatisticsViewModel>();
        List<ClientStatisticsViewModel> listClientStatistics= new List<ClientStatisticsViewModel>();
        
        var truckDrivers = await _context.TruckDrivers.Include(td => td.Deliveries).ToListAsync();

        //STATS TRUCKDRIVERS
        foreach (var truckDriver in truckDrivers)
        {
            var deliveriesDate = await _context.Delivery
                .Where(d => d.LinkedTruckDriver == truckDriver && d.IsFinish && d.IsSucces)
                .Select(d => d.DateAndTimeOfUnLoading) // Sélectionne uniquement DateAndTimeOfUnLoading
                .ToListAsync();
    
            listTruckDriverStatistics.Add(new TruckDriverStatisticsViewModel(truckDriver, deliveriesDate));
        }
        
        //STATS CLIENTS
        var clients = _context.Clients
            .Include(c => c.ClientCompany).ToList();

        foreach (var client in clients)
        {
            var deliveriesDate = await _context.Delivery
                .Where(d => d.LinkedClient == client && d.IsFinish && d.IsSucces)
                .Select(d => d.DateAndTimeOfUnLoading) // Sélectionne uniquement DateAndTimeOfUnLoading
                .ToListAsync();

            listClientStatistics.Add(new ClientStatisticsViewModel(client, deliveriesDate));
        }

        var statistics = new StatisticsViewModel(listClientStatistics, listTruckDriverStatistics);
        
        return await Task.FromResult<IActionResult>(View("ShowStatistics", statistics));
    }

    private async Task DeleteAllDeliveries(TruckDriver truckDriver)
    {
        var deliveries = await _context.Delivery
            .Include(d => d.LinkedClient)
            .Include(d => d.LinkedTruck)
            .Where(d => d.LinkedTruckDriver.Id == truckDriver.Id)
            .ToListAsync();
        
        foreach (var delivery in deliveries)
        {
            delivery.LinkedTruckDriver = null;
            delivery.LinkedTruck = null;
        }

        truckDriver.Deliveries = new List<DeliveryModel>();
        await _context.SaveChangesAsync();
    }
}