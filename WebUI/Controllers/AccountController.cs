using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);
                //bu email e göre kayıtlı veriyi bul.

                

                if (user is not null)
                {
                    var resultMail = await _userManager.IsEmailConfirmedAsync(user);
                    //email onaylaması yapıldı mı?
                    if(resultMail is false)
                    {
                        ModelState.AddModelError("", "Email hesabınızı onaylayın.");
                        return View(loginModel);
                    }
                    
                    //Login işlemini signInManager sağlamakta
                    //ayrıca parolayı test etmek zorunda değiliz zaten bunu bizim yerimize sigInManager yapıyor.
                    var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, isPersistent:true, lockoutOnFailure: false); //password ile giriş yap metotu diyebiliriz.

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        //cookie ayarlarında yanlış girme sayısı vardı biz şimdi onu sıfırladık.
                        await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(1));
                        //yanlış giriş yapma durumunda olan kilit mekanizmasını 30 dakika sonra aktive edeceğim.
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut) //kullanıcının oturumu kilitli ise
                    {
                        var lockoutEndDate = await _userManager.GetLockoutEndDateAsync(user); //kalan kilitleme süresini aldım.

                        if (lockoutEndDate.HasValue)
                        {
                            var timeLeft = lockoutEndDate.Value - DateTime.Now;

                            if (timeLeft > TimeSpan.Zero)
                            {
                                // Kilitlenme süresi dolmamış, kalan süreyi hesapla ve göster
                                ModelState.AddModelError("", $"Kalan süre: {timeLeft.TotalMinutes.ToString("F")} dakika");
                            }
                            else
                            {
                                // Kilitlenme süresi dolmuş
                                ModelState.AddModelError("", "Hesap artık kilitlenmiş değil.");
                            }
                        }
                        else
                        {
                            // Kullanıcının kilitleme süresi yok
                            Console.WriteLine("Kullanıcı hesap kilitleme özelliğine sahip değil.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hatalı email ya da parola"); //burası
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Hatalı emial veya parola!");
                }
            }
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Formda bir hata yok ise bu blok çalışır
                var user = new AppUser
                {
                    UserName = model.FullName.Replace(" ", "").ToLower(),
                    Email = model.Email,
                    FullName = model.FullName,
                };

                // Kullanıcı oluşturma işlemi
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //ilgili kullanıcı için token bilgisi üretilecek.
                    //bu token ise bir url olmalı
                    var url = Url.Action("ConfirmEmail", "Account", new { Id = user.Id, token = token });
                    //ConfirmEmail metotuda ve Account Controllarda kimlik kazanan bir link oluşuyor. Bir örneği aşağıda yazmakta.
                    ///Account/ConfirmEmail/9d310ed0-fad5-414d-9caa-fd71e9cc8a30?token=CfDJ8PmCJ8Q5AtVHkthk1O0UusxH%2FaUdNCgejMT4E4%2FHUqy8zOLbi8spThA2Czi%2BECYDdxCVJ8cvnSYfn59ENmnxGaHwvvUHGlPD%2BKMLvOQVrEOMiGmxHvbyi60Yx%2BovEU2dtu%2BMbCKpXTGGfjWKmgiCLki8y8PnB0Ta6OkJLHd1UhBhIBY1jXgaCxyQkozODAPe80vK0b2eKT0W4HQ4mjkWE4jTNXpxUllgK8HFZr5Dsq2nQEhBapbS5rhRA4PbOD5itQ%3D%3D
                    ///



                    Console.WriteLine(url.ToString());
                    // Kullanıcı başarıyla oluşturulmuşsa
                    TempData["message"] = "Mail adresinize gelen bağlantıya tıklayınız.";
                    return RedirectToAction("Login", "Account");
                }

                // Kullanıcı oluşturulamazsa, hataları ModelState'e ekleyip kullanıcıya göster
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            //ıd ve token kontrolü
            
            //eğer ki token geçersiz veya yok ise
            if(Id is null || token is null)
            {
                TempData["message"] = "Token geçersiz!";
                return View();
            }

            var user = await _userManager.FindByIdAsync(Id);
            if(user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded) 
                {
                    TempData["message"] = "Hesabınız onaylandı";
                    return View();
                }
            }
            TempData["message"] = "Kullanıcı bulunumadı";

            return View();
        }
    }
}
