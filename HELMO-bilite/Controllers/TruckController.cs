using System.ComponentModel.DataAnnotations;
using System.Globalization;
using HELMO_bilite.Models;
using HELMO_bilite.Models.Exceptions;
using HELMO_bilite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Controllers;

[Authorize(Roles = "Admin")]
public class TruckController : Controller
{
    
    private readonly HelmoBiliteDbContext _context;
    private readonly PictureManager _pictureManager;
    public TruckController([FromServices] HelmoBiliteDbContext context,  [FromServices] IWebHostEnvironment env)
    {
        _context = context;
        _pictureManager = new PictureManager(env);
    }

    // GET
    public IActionResult Index()
    {
        return View(_context.Truck.ToList());
    }
    
    [HttpGet]
    [ResponseCache(Duration = 60 * 60 * 24, Location = ResponseCacheLocation.Any, NoStore = false)]
    public IActionResult Create()
    {
        var model = new TruckViewModel
        {
            Truck = new Truck()
        };
        return View("CreateEditView", model);
    }
    
    [HttpGet]
    // Méthode pour afficher le formulaire d'édition
    public IActionResult Edit(int id)
    {
        // Récupérez le camion existant de la base de données en utilisant l'ID
        var existingTruck = _context.Truck.FirstOrDefault(t => t.IdTruck== id);
        
        if (existingTruck == null)
        {
            return NotFound();
        }
        var model = new TruckViewModel
        {
            Truck = existingTruck
        };
        return View("CreateEditView", model);
    }

    // Méthode pour créer un nouveau camion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TruckViewModel model)
    {
        var imagePath = await HandleTruckImageUpload(model);
        if (!string.IsNullOrEmpty(imagePath))
        {
            model.Truck.PictureTruckPath = imagePath;
        }

        if (!ModelState.IsValid)
        { 
            // En cas d'erreur de validation du modèle, les valeurs du modèles
            // sont stockées dans le 'ModelState'. Si une valeur (ici l'image) existe
            // à la fois dans le ModelState (Image par défaut, dans le constructeur)
            // et le model que l'on modifie ici via la méthode HandleTruckIamgeUpload. ModelState
            // préviligie les valeurs du ModelState à afficher plutôt que les nouvelles valeurs
            // mises au modèle
            ModelState.SetModelValue("Truck.PictureTruckPath",
                new ValueProviderResult(model.Truck.PictureTruckPath, CultureInfo.CurrentCulture));
            return View("CreateEditView", model);
        }

        _context.Truck.Add(model.Truck);
        _context.SaveChanges();

        TempData["NotificationMessage"] = "Camion ajouté avec succès.";
        return RedirectToAction("Index");
    }

    // Méthode pour modifier un camion existant
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TruckViewModel model)
    {
        var imagePath = await HandleTruckImageUpload(model);
        if (!string.IsNullOrEmpty(imagePath))
        {
            model.Truck.PictureTruckPath = imagePath;
        }

        if (!ModelState.IsValid)
        {
            ModelState.SetModelValue("Truck.PictureTruckPath",
                new ValueProviderResult(model.Truck.PictureTruckPath, CultureInfo.CurrentCulture));
            return View("CreateEditView", model);
        }

        if (!await CanUpdateTruck(model))
        {
            TempData["DeleteErrorMessage"] =
                $"Impossible de mettre à jour le camion {model.Truck.Brand} {model.Truck.Model} avec l'immatriculation : {model.Truck.NumberPlate} car des livraisons sont en cours et le chauffeur ne dispose pas du nouveau type de permis.";
            return RedirectToAction("Index");
        }

        // Récupérer l'entité Truck existante à partir de la base de données
        var existingTruck = await _context.Truck.FindAsync(model.Truck.IdTruck);

        if (existingTruck == null)
        {
            return NotFound();
        }

        BindValuesUpdateTruck(model, existingTruck);

        // Sauvegarder 
        _context.Update(existingTruck);
        await _context.SaveChangesAsync();


        TempData["NotificationMessage"] = "Camion modifié avec succès.";
        return RedirectToAction("Index");
    }

    private static void BindValuesUpdateTruck(TruckViewModel model, Truck existingTruck)
    {
        // Mettre à jour les propriétés
        existingTruck.Brand = model.Truck.Brand;
        existingTruck.Model = model.Truck.Model;
        existingTruck.NumberPlate = model.Truck.NumberPlate;
        existingTruck.RequiredDrivingLiscence = model.Truck.RequiredDrivingLiscence;
        existingTruck.MaximumTonnage = model.Truck.MaximumTonnage;
        existingTruck.PictureTruckPath = model.Truck.PictureTruckPath;
    }


    // Remplacez cette méthode par la version actuelle dans votre contrôleur Truck
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int idTruck)
    {
        var truck = await _context.Truck.FindAsync(idTruck);

        // Vérifiez si le camion est actuellement en cours d'utilisation
        bool isInUse = _context.Delivery.Any(d => d.LinkedTruck.IdTruck == idTruck && !d.IsFinish);

        if (isInUse)
        {
            TempData["DeleteErrorMessage"] = "Impossible de supprimer le camion car il est actuellement en cours d'utilisation.";
            return RedirectToAction("Index");
        }

        if (truck != null)
        {
            _context.Truck.Remove(truck);
            await _context.SaveChangesAsync();
            TempData["NotificationMessage"] = "Camion supprimé avec succès.";
        }
        else
        {
            TempData["DeleteErrorMessage"] = "Le camion n'a pas pu être trouvé.";
        }

        return RedirectToAction("Index");
    }
    
    /// <summary>
    /// Cette méthode permet de récupérer le chemin lié à une image associée à un camion après son enregistrement dans
    /// le système de fichiers du serveur
    /// </summary>
    /// <param name="model">le camion auquel on souhaite associer l'image</param>
    private async Task<string?> HandleTruckImageUpload(TruckViewModel model)
    {
        if (model.UploadedImage != null)
        {
            try
            {
                 return await _pictureManager.UploadAsync(model.UploadedImage, model.Truck.NumberPlate);
            }
            catch (HelmobiliteStorageException e)
            {
                ViewData["ErrorMessage"] = $"{e.Message}. Votre ancienne image a été conservée";
            }
        }

        return null;
    }
    
    /// <summary>
    /// Cette méthode permet de vérifier si on est en capacité de mettre à jour chauffeur.
    /// Imaginons que l'administrateur souhaite passer le permis requis de C à CE pour conduire
    /// un certain camion. Si le camion est actuellement est en train d'être utilisé par un chauffeur
    /// ne possédant que le permis C, alors il sera impossible de faire cette mise à jour pour le moment.
    /// </summary>
    /// <param name="model">Les informations mises à jour par l'administrateur sur un camion</param>
    /// <returns>true si un camion peut être mis à jour avec les nouvelles informations, false sinon</returns>
    private async Task<bool> CanUpdateTruck(TruckViewModel model)
    {
        var deliveriesInProgress = _context.Delivery
            .Include(d => d.LinkedTruckDriver)
            .Where(d => d.LinkedTruck.IdTruck == model.Truck.IdTruck && !d.IsFinish)
            .ToList();

        if (!deliveriesInProgress.Any())
        {
            return true;
        }

        var currentTruck = await _context.Truck.FindAsync(model.Truck.IdTruck);

        if (currentTruck == null || currentTruck.RequiredDrivingLiscence == model.Truck.RequiredDrivingLiscence)
        {
            return true;
        }

        if (model.Truck.RequiredDrivingLiscence != TypeDrivingLicences.CE)
        {
            return true;
        }

        return Truck.DriverHasNewLicense(deliveriesInProgress, model.Truck.RequiredDrivingLiscence);
    }
    



}