using LaundryApp.Data;
using LaundryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Penting untuk Identity UI

//create connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found!");
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseNpgsql(connectionString));

// Add configure Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Atau true sesuai kebutuhan
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true; // Sesuaikan kebijakan password
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Ditambahkan untuk UI Identity bawaan

//setting cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Durasi sesi login
    options.LoginPath = "/Identity/Account/Login";     // Path ke halaman login
    options.LogoutPath = "/Identity/Account/Logout";   // Path ke halaman logout
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Path jika akses ditolak
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Initialize the database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Initializing the database...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Pertimbangkan untuk hanya menjalankan migrasi saat development atau saat startup pertama
        //context.Database.Migrate(); // Pastikan ini tidak berjalan setiap saat di produksi tanpa strategi
        logger.LogInformation("Database migration check/application completed (if any).");

        logger.LogInformation("Seeding identity data...");
        // Pastikan IdentityDataInitializer sudah didefinisikan
        await IdentityDataInitializer.SeedData(services, builder.Configuration);
        logger.LogInformation("Identity data seeding completed (if applicable).");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database or seeding data.");
        Console.WriteLine($"An error occurred during initialization: {ex.Message}");
        // Pertimbangkan untuk menghentikan aplikasi jika inisialisasi kritis gagal
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); // Aktifkan untuk produksi setelah HTTPS dikonfigurasi
}

// app.UseHttpsRedirection(); // Aktifkan jika Anda telah setup sertifikat SSL

app.UseStaticFiles(); // Menyajikan file dari wwwroot

app.UseRouting();

app.UseAuthentication(); // Middleware autentikasi
app.UseAuthorization();  // Middleware otorisasi

app.MapRazorPages(); // Map endpoint untuk halaman Identity UI dan Razor Pages lainnya

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();