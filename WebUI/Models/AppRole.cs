using Microsoft.AspNetCore.Identity;

namespace WebUI.Models
{
    public class AppRole : IdentityRole
    {
        public string Name { get; set; } = null!;
    }
}
