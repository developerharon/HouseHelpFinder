using HouseHelpFinder.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HouseHelpFinder.Components
{
    /// <summary>
    /// Renders the profile summary component when the user logs in and provides navigation to account specific functions.
    /// </summary>
    public class LoginSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LoginSummaryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the current user from the database and generates a link for the profile picture else uses the default one, and passes it to the view
        /// </summary>
        /// <returns>The view component for the profile summary</returns>
        public IViewComponentResult Invoke()
        {
            var user = _context.Users.Single(user => user.UserName == User.Identity.Name);
            string profilePictureUrl = null;

            if (user.ProfilePicture != null)
                profilePictureUrl = string.Format("data:image/jpg;base64, {0}", Convert.ToBase64String(user.ProfilePicture));
            else
                profilePictureUrl = "/images/profile-pic.png";

            return View("Default", profilePictureUrl);
        }
    }
}
