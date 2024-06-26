using HELMO_bilite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "Client")]
public class ClientController : Controller
{
    
    private readonly HelmoBiliteDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public ClientController(HelmoBiliteDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    } 
    
    public async Task<IActionResult> ShowDeliveries(String status)
    {
        var deliveries = new List<DeliveryModel>();
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        
        if (status.Equals("waiting"))
        {
            deliveries = await _context.Delivery.Where(d => d.LinkedClient.Id == userId &&  d.IsFinish == false && d.LinkedTruckDriver == null && d.LinkedTruck == null).ToListAsync();
            ViewBag.Status = "waiting";
        }
        else if (status.Equals("inProgress"))
        {
            deliveries = await _context.Delivery.Where(d => d.LinkedClient.Id == userId &&  d.LinkedTruckDriver != null && d.LinkedTruck != null && d.IsFinish == false).ToListAsync();
            ViewBag.Status = "inProgress";
        }
        else if (status.Equals("finished"))
        {
            deliveries = await _context.Delivery.Where(d => d.LinkedClient.Id == userId &&  d.IsFinish == true).ToListAsync();
            ViewBag.Status = "finished";
        }
        else
        {
            return NotFound();
        }
        
        return View("DeliveriesManager", deliveries);
    }
    
    
    public IActionResult CreateDelivery()
    {
        ViewBag.Type = "Create";
        DateTime now = DateTime.Now;
        DateTime roundedDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        return View("ProcessDeliveryRequest", new DeliveryModel()
        {
            IdDelivery = 0,
            DateAndTimeOfLoading = roundedDateTime,
            DateAndTimeOfUnLoading = roundedDateTime
        });
    }
    
    public async Task<IActionResult> EditDelivery()
    {

        int deliveryId = int.Parse(Request.Form["deliveryId"]);
        var delivery =  _context.Delivery.Find(deliveryId);

        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients.FindAsync(user.Id);
        if (delivery.LinkedClient.Id != client.Id)
        {
            ViewData["ErrorIdDelivery"] = "Vous tentez de modifier une livraison qui n'est pas l'une des vôtres";
            return RedirectToAction("ShowDeliveries", new { status = "waiting" });
        }

        ViewBag.Type = "Edit";
        return await Task.FromResult<IActionResult>(View("ProcessDeliveryRequest", delivery));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatesDeliveryRequestForm(DeliveryModel delivery)
    {
        
       if (delivery.UnloadingDateIsLowerThanTheLoadingDate(delivery))
        {
            ViewData["ErrorDelivery"] = " La date de chargement doit être antérieur à la date de déchargement";
            return View("ProcessDeliveryRequest", delivery);
        }

        if (delivery.LoadingDateIsLowerThanToday(delivery))
        {
            ViewData["ErrorDelivery"] = " La date de chargement doit être supérieur à la date de actuelle";
            return View("ProcessDeliveryRequest", delivery);
        }

        if (!ModelState.IsValid)
        {
            return View("ProcessDeliveryRequest", delivery);
        }
        
        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients.FindAsync(user.Id);
        
        if (client == null)
        {
            //L'utilisateur n'est pas un client
            return View("Error");
        }

        //Attribution des attributs non demandé à l'utilisateur
        delivery.LinkedClient = client;
        delivery.IsFinish = false;
        
        //Ajout en BD
        _context.Add(delivery);
        _context.SaveChanges();
        
        return RedirectToAction("ShowDeliveries", "Client", new { status = "waiting" });
    }
    


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDeliveryRequestForm(DeliveryModel deliveryModel)
    {
        var delivery = _context.Delivery
            .Include(d => d.LinkedClient)
            .FirstOrDefault(d => d.IdDelivery == deliveryModel.IdDelivery);
        
        if (delivery == null)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid)
        {
            return View("ProcessDeliveryRequest", delivery);
        }
        
        var user = await _userManager.GetUserAsync(User);
        var client = await _context.Clients.FindAsync(user.Id);
        if (delivery.LinkedClient.Id != client.Id)
        {
            ViewData["ErrorIdDelivery"] = "Vous tentez de modifier une livraison qui n'est pas l'une des vôtres";
            return RedirectToAction("EditDelivery");
        }
        
        delivery.PlaceLoadingDelivery = deliveryModel.PlaceLoadingDelivery;
        delivery.PlaceUnLoadingDeliver = deliveryModel.PlaceUnLoadingDeliver;
        delivery.Content = deliveryModel.Content;
        delivery.DateAndTimeOfLoading = deliveryModel.DateAndTimeOfLoading;
        delivery.DateAndTimeOfUnLoading = deliveryModel.DateAndTimeOfUnLoading;

        _context.SaveChanges();

        return RedirectToAction("ShowDeliveries", "Client", new { status = "waiting" });
    }
}