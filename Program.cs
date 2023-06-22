using Microsoft.EntityFrameworkCore;
using Social_Media_Website.Services;
using Social_Media_Website.Data;
using Social_Media_Website.Helpers;
using Social_Media_Website.Interfaces;
using Social_Media_Website.Repository;
using Social_Media_Website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Social_Media_Website.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<SearchIndexer>();

//builder.Services.AddScoped<IUserRepository, UserRepository>();
// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Register UserRepository with connection string
builder.Services.AddScoped<IUserRepository>(provider =>
{
    var dbContext = provider.GetService<ApplicationDbContext>();
    var userManager = provider.GetService<UserManager<AppUser>>();
    return new UserRepository(dbContext, userManager, connectionString);
});


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddSignalRCore();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    await Seed.SeedUsersAndRolesAsync(app);
    Seed.SeedData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Posts/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHub<ChatHub>("/chathub");
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "livechat",
        pattern: "{controller=LiveChat}/{action=Index}/{id?}");
});



app.Run();
