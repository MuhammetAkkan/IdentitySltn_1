using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var result = _roleManager.Roles;
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppRole roleModel)
        {
            if (ModelState.IsValid) 
            {
                if (roleModel.Name is not null)
                    roleModel.Name = roleModel.Name.ToUpper();

                var result = await _roleManager.CreateAsync(roleModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(roleModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (id is not null && role is not null && id == role.Id)
            {
                //ViewBag aracılığıyla Users ı taşıdık.
                if(role is not null && role.Name is not null)
                    ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                
                return View(role);
            }
            return RedirectToAction("Index");
        }

        //tekrar et, biraz ağır.
        [HttpPost]
        public async Task<IActionResult> Edit(string id, AppRole newRole)
        {
            if(id is not null && ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);

                if(role is not null)
                {
                    role.Name = newRole.Name;
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }

                    //yazılan aşağıda
                    /*
                    if (role.Name != null)
                        ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                    */
                    //yazılan kod yukarıda

                    ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
                    if(ViewBag.Users is null || ViewBag.Users.Count() == 0)
                    {
                        ViewBag.Users = string.Empty;
                    }
                }
            }
            //sadee modelden gelen veriyi değil ayrıca Get teki ViewBag i burada sayfaya tekrar göndermemiz lazım.
            
            
            return View(newRole);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {   
            if(string.IsNullOrEmpty(id))
                return View();

            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return View();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("role", err.Description);
            }

            return View();
        }

        
    }
}
