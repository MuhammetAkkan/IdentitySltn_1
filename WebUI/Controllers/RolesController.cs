using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class RolesController : Controller
    {
        private RoleManager<AppRole> _roleManager;
        public RolesController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
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

        public IActionResult Edit(string id)
        {
            if (id is null)
                return View();

            //burada kaldım.

            return View();
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
