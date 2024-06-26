using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HELMO_bilite.Models;

public class HelmoBiliteDbContext : IdentityDbContext
{
    public HelmoBiliteDbContext(DbContextOptions<HelmoBiliteDbContext> options) : base(options)
    {
    }

    protected override async void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relation one to one comme dans la vidéo
        modelBuilder.Entity<Client>()
            .HasOne(c => c.ClientCompany)
            .WithOne(c => c.ClientOfTheCompany)
            .HasForeignKey<Company>(c => c.CompanyOfTheClientId);

        modelBuilder.Entity<IdentityUser>()
            .HasDiscriminator<UserType>("UserType")
            .HasValue<Client>(UserType.Client)
            .HasValue<User>(UserType.Admin)
            .HasValue<IdentityUser>(UserType.IdentityDefault)
            .HasValue<Dispatcher>(UserType.Dispatcher)
            .HasValue<TruckDriver>(UserType.TruckDriver);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }

    public DbSet<CompanyAdress> CompanyAddresses { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Client> Clients { get; set; }

    public DbSet<MemberPerson> MemberPersons { get; set; }
    public DbSet<TruckDriver> TruckDrivers { get; set; }
    public DbSet<Dispatcher> Dispatcher { get; set; }

    public DbSet<Truck> Truck { get; set; }
    public DbSet<DeliveryModel> Delivery { get; set; }
}