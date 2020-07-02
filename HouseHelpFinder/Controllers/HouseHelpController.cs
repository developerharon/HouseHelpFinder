using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Controllers
{
    [Authorize]
    public class HouseHelpController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserValidator<ApplicationUser> _userValidator;

        public HouseHelpController(UserManager<ApplicationUser> userManager, IUserValidator<ApplicationUser> userValidator)
        {
            _userManager = userManager;
            _userValidator = userValidator;
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

        public IActionResult Edit() => View(GetUsersData());

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(CurrentUser.Id);

            if (user != null)
            {
                if (model.ProfilePicture != null)
                {
                    using (var profilePicStream = new MemoryStream())
                    {
                        await model.ProfilePicture.CopyToAsync(profilePicStream);
                        user.ProfilePicture = profilePicStream.ToArray();
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.Username))
                    user.UserName = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Name))
                    user.Name = model.Name;

                if (!string.IsNullOrWhiteSpace(model.Description))
                    user.Description = model.Description;

                if (!string.IsNullOrWhiteSpace(model.Email))
                    user.Email = model.Email;

                if (model.isAvailable || !model.isAvailable)
                    user.isAvailable = model.isAvailable;

                IdentityResult validUserUpdate = await _userValidator.ValidateAsync(_userManager, user);

                if (!validUserUpdate.Succeeded)
                    AddErrorsFromResult(validUserUpdate);

                IdentityResult result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
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
            return View(model);
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
            UserId = CurrentUser.Id,
            ProfilePictureUrl = this.ProfilePictureUrl,
            Username = CurrentUser.UserName,
            Name = CurrentUser.Name,
            Description = CurrentUser.Description,
            Email = CurrentUser.Email,
            isAvailable = CurrentUser.isAvailable,
            ReachOuts = CurrentUser.ReachOuts
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
