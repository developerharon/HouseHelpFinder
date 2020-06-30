using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Controllers
{
    [Authorize]
    public class HouseHelpController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HouseHelpController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ViewResult Index()
        {
            return View(GetUsersData());
        }

        [AllowAnonymous]
        public ViewResult Create() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return RedirectToAction("Index");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private ProfileViewModel GetUsersData() => new ProfileViewModel
        {
            ProfilePictureUrl = this.ProfilePictureUrl,
            Username = CurrentUser.UserName,
            Name = CurrentUser.Name,
            Description = CurrentUser.Description,
            Email = CurrentUser.Email,
            isAvailable = CurrentUser.isAvailable,
        };

        private ApplicationUser CurrentUser => _userManager.Users.Single(user => user.UserName == HttpContext.User.Identity.Name);
        private string ProfilePictureUrl
        {
            get
            {
                if (CurrentUser.ProfilePicture == null) return null;

                return String.Format("data:image/jpg;base64, {0}", Convert.ToBase64String(CurrentUser.ProfilePicture));
            }
        }
}
}
