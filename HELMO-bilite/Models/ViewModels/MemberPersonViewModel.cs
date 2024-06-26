namespace HELMO_bilite.Models.ViewModels;

public class MemberPersonViewModel
{
    public MemberPersonViewModel(List<Dispatcher> dispatchers, List<TruckDriver> truckDrivers)
    {
        this.Dispatchers = dispatchers;
        this.TruckDrivers = truckDrivers;
    }
    
    public List<Dispatcher> Dispatchers { get; set; }
    public List<TruckDriver> TruckDrivers { get; set; }
}