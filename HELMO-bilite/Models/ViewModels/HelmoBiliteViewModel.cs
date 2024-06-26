namespace HELMO_bilite.Models.ViewModels;

public class HelmoBiliteViewModel
{
    public HelmoBiliteViewModel(List<MemberPerson> memberPersons, List<Client> clients, List<Truck> trucks)
    {
        this.MemberPersons = memberPersons;
        this.Clients = clients;
        this.Trucks = trucks;
    }
    
    public List<MemberPerson> MemberPersons { get; set; }
    
    public List<Client> Clients { get; set; }
    
    public List<Truck> Trucks { get; set; }
}