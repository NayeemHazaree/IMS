
using IMS.DataAccess.Data;
using IMS.DataAccess.Initializer;
using IMS.Models.Models.Authorize;
using IMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));


builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential = true;
});

//authorize area page here
builder.Services.AddRazorPages(Options =>
{
    Options.Conventions.AuthorizeAreaPage("Identity", "/Account/Register");
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthorizationHandler, AccessCheckerHandler>();

builder.Services.AddAuthorization(options =>
{
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    optionsBuilder.UseSqlServer(connectionString);
    ApplicationDbContext dbContext = new ApplicationDbContext(optionsBuilder.Options);

    options.AddPolicy("AccessChecker",
        policy => policy.Requirements.Add(new AccessCheckerHandler(dbContext)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
seedDatabase();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void seedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
//IHostingEnvironment _env=new HostingEnvironment();

//    RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)_env);

//IWebHostEnvironment _env = app.Environment;
//IWebHostEnvironment env = app.Environment;
//RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env, "Rotativa");


