using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using WebUI.Models;

namespace WebUI.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")]
    public class RoleUsersTagHelper : TagHelper
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HtmlAttributeName("asp-role-users")]
        public string RoleId { get; set; } = null!;

        //override yazdıktan sonra boşluk yap, çoğu geldi içinden seçtim.
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role is not null && role.Name is not null)
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {   
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userNames.Add(user.UserName ?? "");
                    }
                }
                //yapılabilir bir yöntem aşağıdaki.
                //output.Content.SetContent(userNames.Count == 0 ? "Bu rolde kayıtlı kullanıcı yok." : string.Join(" - ", userNames));

                output.Content.SetHtmlContent(userNames.Count == 0 ? "Bu rolde kayıtlı kullanıcı yok." : setHtml(userNames));
            }
        }

        private string setHtml(List<string> userNames)
        {
            var html = "<ul class=''>";
            foreach(var item in userNames)
            {
                html += "<li class='text-dark'>" + item + "</li>";
            }
            html += "</ul>";
            return html;
        }
    }
}
