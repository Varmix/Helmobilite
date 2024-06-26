using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Models;

public class DataInitializer
{
    
    private static readonly TruckDriver[] truckDrivers = new TruckDriver[]
    {
        new TruckDriver
        {
            UserName = "truckDriverOne@gmail.com", Email = "truckDriverOne@gmail.com", LastName = "Mahy", FirstName = "Francis", Matricule = "Q210208", Permis = TypeDrivingLicences.CE, PicturePath = "~/mylib/truckDriver/driver1.png"
        },
        new TruckDriver
        {
            UserName = "truckDriverTwo@gmail.com", Email = "truckDriverTwo@gmail.com", LastName = "Dupont", FirstName = "Jean", Matricule = "S210486", Permis = TypeDrivingLicences.C, PicturePath = "~/mylib/truckDriver/driver2.png"
        },
        new TruckDriver
        {
            UserName = "truckDriverThree@gmail.com", Email = "truckDriverThree@gmail.com", LastName = "De Vlegelaer", FirstName = "Edwin", Matricule = "B548945", Permis = TypeDrivingLicences.CE, PicturePath = "~/mylib/truckDriver/driver3.png"
        },
        new TruckDriver
        {
            UserName = "truckDriverFor@gmail.com", Email = "truckDriverFor@gmail.com", LastName = "Franchina", FirstName = "Loïc", Matricule = "Q489345", Permis = TypeDrivingLicences.C, PicturePath = "~/mylib/truckDriver/driver4.png"
        },
        new TruckDriver
        {
            UserName = "truckDriverFive@gmail.com", Email = "truckDriverFive@gmail.com", LastName = "Ucci", FirstName = "Anthony", Matricule = "E879485", Permis = TypeDrivingLicences.CE, PicturePath = "~/mylib/truckDriver/driver5.png"
        }
    };

    private static Dispatcher[] dispatchers = new Dispatcher[]
    {
        new Dispatcher
        {
            UserName = "dispatcherOne@gmail.com", Email = "dispatcherOne@gmail.com",LastName = "Montana", FirstName = "Tony", Matricule = "C666966", Permis = TypeDrivingLicences.B, StudyLevel = StudyLevel.Licencier, PicturePath = "~/mylib/truckDriver/driver6.png" 
                
        },
        new Dispatcher
        {
            UserName = "dispatcherTwo@gmail.com", Email = "dispatcherTwo@gmail.com", LastName = "Wick", FirstName = "John", Matricule = "M857896", Permis = TypeDrivingLicences.B, StudyLevel = StudyLevel.Bachelier, PicturePath = "~/mylib/truckDriver/driver7.png"
        }
    };

    private static Client[] clients = new Client[]
    {
        new Client
        {
            ClientCompany = new Company
            {
                NumberCompany = "123456789", CompanyName = "Cisco System Belgium",
                CompanyAdress = new CompanyAdress
                {
                    Street = "De Kleetlaan", Number = "6", Locality = "Machelen", PostalCode = 1831, Coutry = "Belgique"
                }
            },
            Email = "clientOne@gmail.com",
            UserName = "clientOne@gmail.com",
            PicturePath = "~/mylib/company/Cisco.png"
            
        },

        new Client
        {
            ClientCompany = new Company
            {
                NumberCompany = "987654321", CompanyName = "Joskin",
                CompanyAdress = new CompanyAdress
                {
                    Street = "J.F. Kennedy", Number = "10", Locality = "Liège", PostalCode = 4020, Coutry = "Belgique"
                }
            },
            Email = "clientTwo@gmail.com",
            UserName = "clientTwo@gmail.com",
            PicturePath = "~/mylib/company/logo_joskin_small.png"
        },

        new Client
        {
            ClientCompany = new Company
            {
                NumberCompany = "123123123", CompanyName = "Nike",
                CompanyAdress = new CompanyAdress
                {
                    Street = "Chaussure", Number = "9", Locality = "Bruxelles", PostalCode = 8542, Coutry = "Belgique"
                }
            },
            Email = "clientThree@gmail.com",
            UserName = "clientThree@gmail.com",
            PicturePath = "~/mylib/company/Nike.png"
        },
        
        new Client
        {
            ClientCompany = new Company
            {
                NumberCompany = "456456456", CompanyName = "Adidas",
                CompanyAdress = new CompanyAdress
                {
                    Street = "Casquette", Number = "25", Locality = "Anvers", PostalCode = 9745, Coutry = "Belgique"
                }
            },
            Email = "clientFor@gmail.com",
            UserName = "clientFor@gmail.com",
            PicturePath = "~/mylib/company/Adidas.png"
        },
        
        new Client
        {
            ClientCompany = new Company
            {
                NumberCompany = "789789789", CompanyName = "Lacoste",
                CompanyAdress = new CompanyAdress
                {
                    Street = "Pull shirt", Number = "95", Locality = "Arlon", PostalCode = 2548, Coutry = "Belgique"
                }
            },
            Email = "clientFive@gmail.com",
            UserName = "clientFive@gmail.com",
            PicturePath = "~/mylib/company/lacoste.png"
        }

    };

    private static Truck[] trucks = new Truck[]
    {
        new Truck
        {
            Brand = "SCANIA", Model = "S650 V8 Highline", NumberPlate = "1-ABC-123",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 32, PictureTruckPath = "~/mylib/trucks/scania.png"
        },
        new Truck
        {
            Brand = "VOLVO", Model = "FH16 750 Globetrotter", NumberPlate = "1-JPN-485",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 32, PictureTruckPath = "~/mylib/trucks/volvo.png"
        },
        new Truck
        {
            Brand = "MERCEDES", Model = "Actros 1853 LS", NumberPlate = "1-POA-487",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 32, PictureTruckPath = "~/mylib/trucks/mercedes.png"
        },
        new Truck
        {
            Brand = "DAF", Model = "XF 530 FTG", NumberPlate = "1-CXF-843",
            RequiredDrivingLiscence = TypeDrivingLicences.C, MaximumTonnage = 5, PictureTruckPath = "~/mylib/trucks/daf.png"
        },
        new Truck
        {
            Brand = "MAN", Model = "TGX 18.560", NumberPlate = "1-CHE-618",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 32, PictureTruckPath = "~/mylib/trucks/man.png"
        },
        new Truck
        {
            Brand = "IVECO", Model = "XP 570", NumberPlate = "1-FIO-982",
            RequiredDrivingLiscence = TypeDrivingLicences.C, MaximumTonnage = 16, PictureTruckPath = "~/mylib/trucks/iveco.png"
        },
        new Truck
        {
            Brand = "RENAULT", Model = "T 480", NumberPlate = "1-OIE-486",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 48, PictureTruckPath = "~/mylib/trucks/renault.png"
        },
        new Truck
        {
            Brand = "PETERBILT", Model = "389", NumberPlate = "1-IOH-478",
            RequiredDrivingLiscence = TypeDrivingLicences.C, MaximumTonnage = 10, PictureTruckPath = "~/mylib/trucks/peterbilt.png"
        },
        new Truck
        {
            Brand = "KENWORTH", Model = "W990", NumberPlate = "1-OPK-354",
            RequiredDrivingLiscence = TypeDrivingLicences.C, MaximumTonnage = 15, PictureTruckPath = "~/mylib/trucks/kenworth.png"
        },
        new Truck
        {
            Brand = "MACK", Model = "Anthem", NumberPlate = "1-TRD-784",
            RequiredDrivingLiscence = TypeDrivingLicences.CE, MaximumTonnage = 28, PictureTruckPath = "~/mylib/trucks/mack.png"
        }
    };

    private static DeliveryModel[] deliveries = new DeliveryModel[]
    {
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Liège", PlaceUnLoadingDeliver = "Anvers", Content = "Chaussure",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-10T10:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-11T18:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Anvers", PlaceUnLoadingDeliver = "Liège", Content = "Eau",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-10T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-11T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Bruxelles", PlaceUnLoadingDeliver = "Paris", Content = "Papier",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-15T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-20T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Paris", PlaceUnLoadingDeliver = "Bruxelles", Content = "Tacos",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-18T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-23T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Hainaut", PlaceUnLoadingDeliver = "Arlon", Content = "Pickel Rick",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-22T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-24T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Arlon", PlaceUnLoadingDeliver = "Hainaut", Content = "Souris",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-24T14:50:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-25T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Paris", PlaceUnLoadingDeliver = "Moscou", Content = "Vodka",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-10T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-30T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Moscou", PlaceUnLoadingDeliver = "Paris", Content = "Bonnet",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-30T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-06-10T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Stuttgart", PlaceUnLoadingDeliver = "Marseille", Content = "Voiture",
            DateAndTimeOfLoading = DateTime.Parse("2023-05-20T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-05-28T14:30:00")
        },
        new DeliveryModel
        {
            PlaceLoadingDelivery = "Marseille", PlaceUnLoadingDeliver = "Stuttgart", Content = "Moto",
            DateAndTimeOfLoading = DateTime.Parse("2023-06-1T14:30:00"), DateAndTimeOfUnLoading = DateTime.Parse("2023-06-6T14:30:00")
        }
    };
    
    public static void SeedRole(RoleManager<IdentityRole> roleManager)
    {
        if (roleManager.RoleExistsAsync("Client").Result == false)
        {
            IdentityRole admin = new IdentityRole() { Name = "Client" };
            var result = roleManager.CreateAsync(admin);
            result.Wait();
        }
        if (roleManager.RoleExistsAsync("TruckDriver").Result == false)
        {
            IdentityRole user = new IdentityRole() { Name = "TruckDriver" };
            var result = roleManager.CreateAsync(user);
            result.Wait();
        }
        if (roleManager.RoleExistsAsync("Dispatcher").Result == false)
        {
            IdentityRole user = new IdentityRole() { Name = "Dispatcher" };
            var result = roleManager.CreateAsync(user);
            result.Wait();
        }
        if (roleManager.RoleExistsAsync("Admin").Result == false)
        {
            IdentityRole user = new IdentityRole() { Name = "Admin" };
            var result = roleManager.CreateAsync(user);
            result.Wait();
        }
    }
    
    
    public static async Task SeedUsers(UserManager<User> _userManager)
    {
        if (_userManager.Users.Count() != 0)
            return;

        foreach (var client in clients)
        {
            var result =  await _userManager.CreateAsync(client, "Azerty!28");
            
            if (result.Succeeded)
            {
                var result2 = _userManager.AddToRoleAsync(client, "Client").Result;
            }
        }
        
        foreach (var dispatcher in dispatchers)
        {
            var result = await _userManager.CreateAsync(dispatcher, "Azerty!28");
        
            if (result.Succeeded)
            {
                var result2 = _userManager.AddToRoleAsync(dispatcher, "Dispatcher").Result;
            }
        }
       

        foreach (var truckDriver in truckDrivers)
        {
            var result = await _userManager.CreateAsync(truckDriver, "Azerty!28");
        
            if (result.Succeeded)
            {
                var result2 = _userManager.AddToRoleAsync(truckDriver, "TruckDriver").Result;
            }
        }

        var admin = new User()
        {
            Email = "admin@gmail.com",
            UserName = "admin@gmail.com"
        };
        
        var resultAdmin = await _userManager.CreateAsync(admin, "Azerty!28");
        
        if (resultAdmin.Succeeded)
        {
            var result2 = _userManager.AddToRoleAsync(admin, "Admin").Result;
        }
        
    }

    public static async Task SeedItems(HelmoBiliteDbContext context)
    {
        if (!context.Truck.Any())
        {
            foreach (var truck in trucks)
            {
                await context.AddAsync(truck);
                await context.SaveChangesAsync();
            }
        }

        if (!context.Delivery.Any())
        {
            foreach (var delivery in deliveries)
            {
                List<Client> clients = await context.Clients.ToListAsync();
                Random random = new Random();
                var client = clients[random.Next(clients.Count)];
                delivery.LinkedClient = client;
                
                await context.AddAsync(delivery);
                await context.SaveChangesAsync();
            }
        }
       
    }
}