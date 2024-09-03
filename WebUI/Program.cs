using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebUI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 //scoped olmasının nedeni her talepte aynı yapıyı çalıştırmak/aynı yapıya ulaşmak istiyorum. Yani IEmailSender çalıştığı an SmtpEmailSender çalışacak.
//Dikkat edilmesi gereken nokta ise şu. SmtpEmailSender ın parametre almayan bir versiyonu yok.
builder.Services.AddControllersWithViews();

//DbContext => Db servisi
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

//Identity servisi
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();


//burada IdentityOptions ı configure ederek opsiyonluyoruz(options)
builder.Services.Configure<IdentityOptions>(options => 
{
    options.Password.RequireDigit = true; // Parolanın en az bir rakam içermesini zorunlu kılar.
    options.Password.RequireLowercase = false; // Parolanın en az bir küçük harf içermesini zorunlu kılar.
    options.Password.RequireNonAlphanumeric = false; // Parolanın en az bir özel karakter içermesini zorunlu kılar.
    options.Password.RequireUppercase = false; // Parolanın en az bir büyük  harf içermesini zorunlu kılar.
    options.Password.RequiredLength = 5; // Parolanın en az 5 karakter uzunluğunda olmasını zorunlu kılar.
    options.Password.RequiredUniqueChars = 0; // Parolanın en az bir benzersiz karakter içermesini zorunlu kılar.

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Hesap kilitlenme süresi.
    options.Lockout.MaxFailedAccessAttempts = 10; // Maksimum hatalı giriş denemesi sayısı.

    options.Lockout.AllowedForNewUsers = true; // Yeni kullanıcılar için hesap kilitleme etkin mi?
    options.User.RequireUniqueEmail = true; // Her kulla    nıcının benzersiz bir e-posta adresine sahip olmasını zorunlu kılar.
         // Geçerli kullanıcı adı karakterleri.

    options.SignIn.RequireConfirmedEmail = true; // Kullanıcının oturum açabilmesi için e-postasını doğrulaması gerekir.
    options.SignIn.RequireConfirmedPhoneNumber = false; // Kullanıcının oturum açabilmesi için telefon numarasını doğrulaması gerekir.

});

//Giriş cookie opsiyonlama

//login opsiyonlama
builder.Services.ConfigureApplicationCookie(options =>
{
    //login kanalları ve alt yapı ayarları
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Index";

    options.SlidingExpiration = true; // Çerez süresini her işlemde uzatır, böylece oturum süresi uzatılır
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); //kimlik doğrulama çerezinin ömrü
    options.Cookie.HttpOnly = true; // Çerezlere JavaScript tarafından erişilmesini engeller, güvenliği artırır
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Çerezlerin sadece HTTPS üzerinden iletilmesini zorunlu kılar
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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityTestUser(app);

app.Run();
