using Microsoft.AspNetCore.Identity;

namespace WebUI.Models
{
    public class AppUser : IdentityUser
    {
        //diğer kullanıcı bilgilerini almak için oluşturduk.
        public string? FullName { get; set; }
        
    }
}
