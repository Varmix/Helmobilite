namespace HELMO_bilite.Models.ViewModels;

public class StatisticsViewModel
{
    public StatisticsViewModel(List<ClientStatisticsViewModel> clientStatistics, List<TruckDriverStatisticsViewModel> truckDriversStatistics)
    {
        this.ClientStatistics = clientStatistics;
        this.TruckDriversStatistics = truckDriversStatistics;
    }
    
    public List<ClientStatisticsViewModel> ClientStatistics { get; set; }
    public List<TruckDriverStatisticsViewModel> TruckDriversStatistics { get; set; }
}