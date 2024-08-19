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
                    //Login işlemini signInManager sağlamakta
                    //ayrıca parolayı test etmek zorunda değiliz zaten bunu bizim yerimize sigInManager yapıyor.
                    var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, isPersistent:true, lockoutOnFailure: false); //password ile giriş yap metotu diyebiliriz.

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        //cookie ayarlarında yanlış girme sayısı vardı biz şimdi onu sıfırladık.
                        await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(30));
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
    }
}
