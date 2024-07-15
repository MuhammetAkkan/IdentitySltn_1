using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Admin_123";

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var user = await userManager.FindByNameAsync(adminUser);
            if (user == null) 
            {
                user = new AppUser 
                {
                    FullName = "Muhammet Akkan",
                    UserName = adminUser,
                    Email = "admin@akkan.com",
                    PhoneNumber = "5445907690",

                };
                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
