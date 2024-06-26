using System.ComponentModel.DataAnnotations;

namespace HELMO_bilite.Models;

public class Truck
{
    [Key]
    public int IdTruck { get; set; }
    
    [Required(ErrorMessage = "La marque est requise.")]
    [MaxLength(25, ErrorMessage = "La marque ne doit pas dépasser 25 caractères.")]
    [DataType(DataType.Text)]
    public string Brand { get; set; }
    
    [Required(ErrorMessage = "Le modèle est requis.")]
    [MaxLength(25, ErrorMessage = "Le modèle ne doit pas dépasser 25 caractères.")]
    [DataType(DataType.Text)]
    public string Model { get; set; }
    
    [Required(ErrorMessage = "La plaque d'immatriculation est requise.")]
    [MaxLength(11, ErrorMessage = "La plaque d'immatriculation ne doit pas dépasser 11 caractères.")]
    [RegularExpression(@"\d{1}-[a-zA-Z]{3}-\d{3}", ErrorMessage = "La plaque d'immatriculation doit être au format 1-ABC-123.")]
    public string NumberPlate { get; set;}
    
    [Required(ErrorMessage = "Le type de permis de conduire est requis.")]
    public TypeDrivingLicences RequiredDrivingLiscence { get; set;}
    
    [Required(ErrorMessage = "La valeur de tonnage maximal est requise.")]
    [Range(2, 150, ErrorMessage = "Le tonnage du camion doit être compris entre 2 et 150 tonnes.")]
    [Display(Name = "Tonnage maximal")]
    public int MaximumTonnage { get; set; }
    
    [MaxLength(500, ErrorMessage = "Le chemin d'accès à l'image du camion ne doit pas dépasser 500 caractères.")]
    public string? PictureTruckPath { get; set; }
    public ICollection<DeliveryModel> Deliveries {get; set; }
    
    public Truck()
    {
        Deliveries = new List<DeliveryModel>();
        PictureTruckPath ??= PictureService.DefaultTruckUser;
    }
    
    public static bool DriverHasNewLicense(List<DeliveryModel> deliveriesInProgress, TypeDrivingLicences newLicense)
    {
        foreach (var delivery in deliveriesInProgress)
        {
            var driver = delivery.LinkedTruckDriver;
            if (driver.Permis != newLicense)
            {
                return false;
            }
        }

        return true;
    }
}