namespace HELMO_bilite.Models.ViewModels;

public class AssignDriverViewModel
{
    public AssignDriverViewModel(DeliveryModel DeliveryPending, List<TruckDriver> AvailableDrivers, List<Truck> AvailableTrucks)
    {
        this.DeliveryPending = DeliveryPending;
        this.AvailableDrivers = AvailableDrivers;
        this.AvailableTrucks = AvailableTrucks;
    }
    public DeliveryModel DeliveryPending { get; set; }
    public List<TruckDriver> AvailableDrivers { get; set; }
    public List<Truck> AvailableTrucks { get; set; }
}