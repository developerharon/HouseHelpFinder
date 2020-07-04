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
    /// <summary>
    /// Contains all the househelp operations
    /// </summary>
    [Authorize]
    public class HouseHelpController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserValidator<ApplicationUser> _userValidator;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        /// <summary>
        /// Uses dependency injection to get the required services
        /// </summary>
        public HouseHelpController(UserManager<ApplicationUser> userManager, IUserValidator<ApplicationUser> userValidator, IPasswordValidator<ApplicationUser> passwordValidator, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Displays the user's profile including the reach out requests.
        /// </summary>
        public ViewResult Index()
        {
            return View(GetUsersData());
        }

        /// <summary>
        /// Displays the form used to create a new account for the househelp
        /// </summary>
        [AllowAnonymous]
        public ViewResult Create() => View();

        /// <summary>
        /// Creates the househelp account in the database
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                    return View(model);
                }

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

        /// <summary>
        /// Displays the form used to edit the househelp account
        /// </summary>
        public IActionResult Edit() => View(GetUsersData());

        /// <summary>
        /// Saves the account changes to the database
        /// </summary>
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

        /// <summary>
        /// Displays the form used to change the password
        /// </summary>
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel() { HouseHelpId = CurrentUser.Id });

        /// <summary>
        /// Saves the new password to the database if it is a valid password
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(model);
                }

                ApplicationUser user = await _userManager.FindByIdAsync(model.HouseHelpId);

                if (user == null)
                {
                    ModelState.AddModelError("", "User Not Found");
                    return View(model);
                }

                if (!await _userManager.CheckPasswordAsync(user, model.CurrentPassword))
                {
                    ModelState.AddModelError("", "Wrong current password! Change not authorized");
                    return View(model);
                } 

                IdentityResult validateNewPassword = await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);

                if (validateNewPassword.Succeeded)
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                else
                    AddErrorsFromResult(validateNewPassword);

                IdentityResult result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }

            return View(model);
        }

        /// <summary>
        /// Adds a list of erros to the model state to be returned to the user
        /// </summary>
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        /// <summary>
        /// Creates and returns a view model that represent the current user to displayed in the profile page
        /// </summary>
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

        /// <summary>
        /// Get's an object from the database that represents the current user
        /// </summary>
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
