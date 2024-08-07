using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebUI.Models;

namespace WebUI.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users", ParentTag = "")]
    public class RoleUsersTagHelper : TagHelper
    {
        private readonly RoleManager<AppUser> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleUsersTagHelper(RoleManager<AppUser> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HtmlAttributeName("asp-role-users")]
        public string RoleId { get; set; } = null!;

        //override yazdıktan sonra çoğu geldi içinden seçtim.
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(RoleId);
            
            if (role is not null)
            {
                foreach (var user in _userManager.Users)
                {
                    //if (await _userManager.IsInRoleAsync(user, role.Name))
                    //{
                    //    userNames.Add(user.UserName);
                    //}
                }
            }

        }

    }
}
