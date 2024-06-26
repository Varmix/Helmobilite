using Newtonsoft.Json;

namespace HELMO_bilite.Models.ViewModels;

public class ClientStatisticsViewModel
{
    public ClientStatisticsViewModel(Client client, List<DateTime> finishedDeliveriesDate)
    {
        this.Client = client;
        this.finishedDeliveriesDate = finishedDeliveriesDate;
    }
    
    public Client Client { get; set; }
    
    public List<DateTime> finishedDeliveriesDate { get; set; }
}