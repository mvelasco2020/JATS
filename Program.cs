using JATS.Data;
using JATS.Factory;
using JATS.Models;
using JATS.Services;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");


builder.Services.AddIdentity<JTUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<JTUserClaimsPrincipalFactory>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//Database
// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(connectionString));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(DataUtility.GetConnectionString(builder.Configuration),
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
///

//Services
builder.Services.AddScoped<IRolesService, RoleService>();
builder.Services.AddScoped<ICompanyInfoService, CompanyInfoService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketHistoryService, TicketHistoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IInviteService, InviteService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ILookupService, LookupService>();
///

//Mail Services/Setttings
var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddScoped<IEmailSender, EmailService>();
///

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


var app = builder.Build();

//Allow datetime wihtouth TZ to work
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
///

//Run Datautility 
await DataUtility.ManageDataAsync(app);
///

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
