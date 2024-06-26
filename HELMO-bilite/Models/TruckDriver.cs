namespace HELMO_bilite.Models;


public class TruckDriver : MemberPerson
{
   public ICollection<DeliveryModel> Deliveries {get; set; }
   
   public TruckDriver()
   {
      Deliveries = new List<DeliveryModel>();
   }
}