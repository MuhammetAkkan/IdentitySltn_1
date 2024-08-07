using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager; //db işlemleri için ıdentity referansı
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                var result = await _userManager.CreateAsync(user, model.Password);
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


        public async Task<IActionResult> Edit(string id)
        {
            if (id is null)
            {
                return View();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return View();

            ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
            //rolleri sayfaya taşıdık.

            var editViewModel = new EditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                SelectedRoles = await _userManager.GetRolesAsync(user),
                //kullanıcının rolünü taşıdık sayfaya
            };
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {
            if (id is null || id != model.Id)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user is not null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        //şifreyi sil ve şifre oluştur
                        await _userManager.RemovePasswordAsync(user);
                        await _userManager.AddPasswordAsync(user, model.Password); //user a model e girdiği passwo

                    }

                    if (result.Succeeded)
                    {
                        var userRole = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, userRole);

                        if (model.SelectedRoles is not null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        }
                        return RedirectToAction("Index");

                    }

                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }



    }
}
