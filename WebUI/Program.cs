using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityContext>();


//burada IdentityOptions ı configure ederek opsiyonluyoruz(options)
builder.Services.Configure<IdentityOptions>(options => 
{
    options.Password.RequireDigit = true; // Parolanın en az bir rakam içermesini zorunlu kılar.
    options.Password.RequireLowercase = true; // Parolanın en az bir küçük harf içermesini zorunlu kılar.
    options.Password.RequireNonAlphanumeric = false; // Parolanın en az bir özel karakter içermesini zorunlu kılar.
    options.Password.RequireUppercase = true; // Parolanın en az bir büyük harf içermesini zorunlu kılar.
    options.Password.RequiredLength = 6; // Parolanın en az 6 karakter uzunluğunda olmasını zorunlu kılar.
    options.Password.RequiredUniqueChars = 0; // Parolanın en az bir benzersiz karakter içermesini zorunlu kılar.

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Hesap kilitlenme süresi.
    options.Lockout.MaxFailedAccessAttempts = 5; // Maksimum hatalı giriş denemesi sayısı.
    options.Lockout.AllowedForNewUsers = true; // Yeni kullanıcılar için hesap kilitleme etkin mi.

    options.User.RequireUniqueEmail = true; // Her kullanıcının benzersiz bir e-posta adresine sahip olmasını zorunlu kılar.
    options.User.AllowedUserNameCharacters = "abcçdefghıijklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+"; // Geçerli kullanıcı adı karakterleri.

    options.SignIn.RequireConfirmedEmail = false; // Kullanıcının oturum açabilmesi için e-postasını doğrulaması gerekir.
    options.SignIn.RequireConfirmedPhoneNumber = false; // Kullanıcının oturum açabilmesi için telefon numarasını doğrulaması gerekir.

});

//cookie opsiyonlama

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true; // Şifrede en az bir rakam gereksinimi
    options.Password.RequiredLength = 5; // Minimum şifre uzunluğu
    options.Password.RequireNonAlphanumeric = false; // Alfanümerik olmayan karakter gereksinimi yok
    options.Password.RequireUppercase = false; // Şifrede en az bir büyük harf gereksinimi
    options.Password.RequireLowercase = false; // Şifrede en az bir küçük harf gereksinimi

    options.Lockout.MaxFailedAccessAttempts = 5; // Yanlış giriş denemesi limiti

    options.User.RequireUniqueEmail = true; // E-posta adresi benzersiz olmalı
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Kullanıcı adı için izin verilen karakterler


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

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);

app.Run();
