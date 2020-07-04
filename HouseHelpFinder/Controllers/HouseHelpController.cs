using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public HouseHelpController(UserManager<ApplicationUser> userManager, IUserValidator<ApplicationUser> userValidator, IPasswordValidator<ApplicationUser> passwordValidator, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
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

        public IActionResult ChangePassword() => View(new ChangePasswordViewModel() { HouseHelpId = CurrentUser.Id });

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Checks that all the fields are filled before being submitted
            if (ModelState.IsValid)
            {
                // Checks that the user has entered the correct password
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(model);
                }

                // Get's the user object from the database
                ApplicationUser user = await _userManager.FindByIdAsync(model.HouseHelpId);

                // If the user is not found render the default view to show the error.
                if (user == null)
                {
                    ModelState.AddModelError("", "User Not Found");
                    return View(model);
                }

                // Checks the user has entered the right current password
                if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                {
                    ModelState.AddModelError("", "Wrong current password! Change not authorized");
                    return View(model);
                } 

                // Validates that the new password is strong.
                IdentityResult validateNewPassword = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);

                // If the new password is strong, hash it and save it to the user object else add errors from the operation above
                if (validateNewPassword.Succeeded)
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                else
                    AddErrorsFromResult(validateNewPassword);

                // Update the user object in the database to reflect the new password.
                IdentityResult result = await _userManager.UpdateAsync(user);

                // If the operation is successful redirect the user to the profile page else add errors to the model state
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }

            // Render the default view to show the errors
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
