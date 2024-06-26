using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HELMO_bilite.Models;

public class DeliveryModel
{
    [Key]
    public int IdDelivery { get; set; }
    
    [Required(ErrorMessage = "Le lieu de chargement est requis.")]
    [MaxLength(75, ErrorMessage = "Le lieu de chargement ne doit pas dépasser 75 caractères")]
    public string PlaceLoadingDelivery { get; set; }
    
    [Required(ErrorMessage = "Le lieu de déchargmement est requis.")]
    [MaxLength(75, ErrorMessage = "Le lieu de déchargement ne doit pas dépasser 75 caractères")]
    public string PlaceUnLoadingDeliver { get; set; }
    
    [Required(ErrorMessage = "Le contenu de la livraison est requis.")]
    [MaxLength(50,  ErrorMessage = "Le contenu de la livraison ne peut pas dépasser les 50 caractères")]
    public string Content { get; set; }
    
    [Required(ErrorMessage = "La date de chargement est requise.")]
    [DataType(DataType.DateTime, ErrorMessage = "La date de chargement est invalide. Elle doit respecter le format dd/mm/yyyy.")]
    public DateTime DateAndTimeOfLoading { get; set; }
    
    [Required(ErrorMessage = "La date de déchargement est requise.")]
    [DataType(DataType.DateTime, ErrorMessage = "La date de déchargement est invalide. Elle doit respecter le format dd/mm/yyyy.")]
    public DateTime DateAndTimeOfUnLoading { get; set; }
    public Boolean IsFinish { get; set; }
    public Boolean IsSucces { get; set; }
    public Client? LinkedClient { get; set; }
    
    public TruckDriver? LinkedTruckDriver { get; set; }
    public Truck? LinkedTruck { get; set; }
    
    [MaxLength(500, ErrorMessage = "Votre commentaire ne peut dépasser 500 caractères.")]
    public string? Comment { get; set; }
    
    public bool UnloadingDateIsLowerThanTheLoadingDate(DeliveryModel delivery)
    {
        return delivery.DateAndTimeOfUnLoading <= delivery.DateAndTimeOfLoading;
    }

    public bool LoadingDateIsLowerThanToday(DeliveryModel delivery)
    {
        return delivery.DateAndTimeOfLoading < DateTime.Now;
}
    
}