using Microsoft.AspNetCore.Identity;

namespace WebUI.Models
{
    public class AppRole : IdentityRole
    {
        public int MyProperty { get; set; }
    }
}
