using HELMO_bilite.Models;
using HELMO_bilite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "Dispatcher")]
public class DispatcherController : Controller
{
    private readonly HelmoBiliteDbContext _context;
    
    public DispatcherController(HelmoBiliteDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        //Récupération de toutes les livraisons en attentes
        var deliveries = await _context.Delivery
            .Include(d => d.LinkedClient)
            .Where(d => d.LinkedTruck == null && d.LinkedTruckDriver == null)
            .OrderBy(d => d.DateAndTimeOfLoading)
            .ToListAsync();
        
        List <AssignDriverViewModel> assignDriverViewModel= new List<AssignDriverViewModel>();
        
        //Ajout de chaque livraison avec les drivers et trucks dispo à ce moment au AssignDriverViewModel
        foreach (var currentDelivery in deliveries)
        {
            var availableDrivers = await _context.TruckDrivers
                .Where(driver => driver.Permis != TypeDrivingLicences.B && !driver.Deliveries
                    .Any(delivery => !delivery.IsFinish && 
                                     (delivery.DateAndTimeOfLoading < currentDelivery.DateAndTimeOfUnLoading.AddHours(1).AddMinutes(1) &&
                                      delivery.DateAndTimeOfUnLoading.AddHours(1).AddMinutes(1) > currentDelivery.DateAndTimeOfLoading)))
                .ToListAsync();


            var availableTrucks = await _context.Truck
                .Where(truck => !truck.Deliveries
                    .Any(delivery => !delivery.IsFinish && 
                                     (delivery.DateAndTimeOfLoading < currentDelivery.DateAndTimeOfUnLoading.AddHours(1).AddMinutes(1) &&
                                      delivery.DateAndTimeOfUnLoading.AddHours(1).AddMinutes(1) > currentDelivery.DateAndTimeOfLoading)))
                .ToListAsync();

            if (availableDrivers.Count == 0 || availableTrucks.Count == 0)
            {
                currentDelivery.IsFinish = true;
                currentDelivery.IsSucces = false;
                currentDelivery.Comment = "Pas de chauffeur ou camion disponible.";
                await _context.SaveChangesAsync();
            }
            
            //Return de la vue avec le ViewModel
            assignDriverViewModel.Add(new AssignDriverViewModel(currentDelivery, availableDrivers, availableTrucks));
        }
       
        
        return View("AssignDriverToDelivery", assignDriverViewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessAssignDelivery()
    {
      
        int selectedTruckId = int.Parse(Request.Form["trucks"]);
        string selectedDriverId = Request.Form["truckDrivers"];
        
        var selectedDriver = await _context.TruckDrivers.FindAsync(selectedDriverId);
        var selectedTruck = await _context.Truck.FindAsync(selectedTruckId);

        if (selectedDriver.Permis == TypeDrivingLicences.C &&
            selectedTruck.RequiredDrivingLiscence == TypeDrivingLicences.CE)
        {
            TempData["ErrorMessage"] = "Le permis du conducteur sélectionné ne correspond pas au type de permis requis pour le camion sélectionné.";
            return RedirectToAction("Index", "Dispatcher");
        }
        
        int deliveryId = int.Parse(Request.Form["deliveryId"]);
        var selectedDelivery = await _context.Delivery.FindAsync(deliveryId);

        selectedDelivery.LinkedTruckDriver = selectedDriver;
        selectedDelivery.LinkedTruck = selectedTruck;
        
        selectedDriver.Deliveries.Add(selectedDelivery);
        
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Livraison assignée avec succès au chauffeur " + selectedDriver.LastName + " " + selectedDriver.FirstName + ".";
        return RedirectToAction("Index", "Dispatcher");
    }
    
}