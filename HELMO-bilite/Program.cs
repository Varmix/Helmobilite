using HELMO_bilite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddEntityFrameworkStores<HelmoBiliteDbContext>(); 


builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<HelmoBiliteDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders(); 

builder.Services.AddScoped<IPictureService, PictureService>();
// Ajouter la configuration de l'environnement d'hébergement
var env = builder.Environment;

// Profil de développement

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<HelmoBiliteDbContext>(options 
        => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsDev")));
}
else
{
    builder.Services.AddDbContext<HelmoBiliteDbContext>(options 
        => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsProd")));
}
var app = builder.Build();



// Vérifier si l'application est en mode développement
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}   

// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var context= scope.ServiceProvider.GetRequiredService<HelmoBiliteDbContext>();
    DataInitializer.SeedRole(roleManager);
    await DataInitializer.SeedUsers(userManager);
    await DataInitializer.SeedItems(context);
}



app.UseHttpsRedirection();
app.UseStaticFiles(
    new StaticFileOptions { OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=" + durationInSeconds;
    }}
);

app.UseRouting();

// Ajout de l'authentification
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Register}/{action=Index}/{id?}");
// app.MapRazorPages();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Anonymous}/{action=Index}");
    endpoints.MapRazorPages();

});


app.Run();