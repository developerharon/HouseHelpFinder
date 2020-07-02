using HouseHelpFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Components
{
    public class LoginSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LoginSummaryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var user = _context.Users.Single(user => user.UserName == User.Identity.Name);
            string profilePictureUrl = string.Format("data:image/jpg;base64, {0}", Convert.ToBase64String(user.ProfilePicture));
            return View("Default", profilePictureUrl);
        }
    }
}
