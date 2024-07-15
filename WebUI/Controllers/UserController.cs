using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> _userManager;
        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users);
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
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Kullanıcı başarıyla oluşturulmuşsa
                    return RedirectToAction("Index");
                }

                // Kullanıcı oluşturulamazsa, hataları ModelState'e ekleyip kullanıcıya göster
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
