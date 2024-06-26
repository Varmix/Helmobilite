using Newtonsoft.Json;

namespace HELMO_bilite.Models.ViewModels;

public class TruckDriverStatisticsViewModel
{
    public TruckDriverStatisticsViewModel(TruckDriver truckDriver, List<DateTime> finishedDeliveriesDate)
    {
        this.TruckDriver = truckDriver;
        this.finishedDeliveriesDate = finishedDeliveriesDate;
    }
    
    public TruckDriver TruckDriver { get; set; }
    
    public List<DateTime> finishedDeliveriesDate { get; set; }

}