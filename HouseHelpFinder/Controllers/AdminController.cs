using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await GetSystemSummaryAsync());
        }

        /// <summary>
        /// Loops through all the users in the database and passes only the administrators to the default view
        /// </summary>
        /// <returns>A view to display all the administrator accounts</returns>
        public async Task<IActionResult> ListAdmins()
        {
            return View(await GetAdminsAsync());
        }

        public async Task<IActionResult> ListHouseHelps(string searchParameter = null)
        {
            var houseHelps = await GetHouseHelpsAsync();

            if (searchParameter != null)
                return View(houseHelps.Where(user => user.UserName.Contains(searchParameter)));

            return View(houseHelps);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            // Check if the model has all the properties filled
            if (ModelState.IsValid)
            {
                // Create a new Application User model
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                // Create the Account in the database
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                // Check if the result operation completed successfully and if not add the errors to the model state.
                if (result.Succeeded)
                {
                    // Check if the administrator role exists and if not create one
                    if (!await _roleManager.RoleExistsAsync("Administrators"))
                    {
                        result = await _roleManager.CreateAsync(new IdentityRole("Administrators"));

                        // If there's an error creating the role, add a user friendly error to the user
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Error occured when creating the account");
                            return View(model);
                        }
                    }

                    // Add the user to the administrator role
                    result = await _userManager.AddToRoleAsync(user, "Administrators");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Error occured when creating the account");
                        return View(model);
                    }

                    // Redirect the user to this action method if all the operations complete successfully
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

            // Render the default view to show the errors to the user.
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["message"] = "Operation Successful! User Removed From Database";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Error when deleting the account";
                }
            }
            else
            {
                TempData["message"] = "User Not Found";
            }
            return RedirectToAction("Index");
        }

        private async Task<List<ApplicationUser>> GetAdminsAsync()
        {
            List<ApplicationUser> admins = new List<ApplicationUser>();
            foreach (ApplicationUser user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Administrators"))
                {
                    admins.Add(user);
                }
            }
            return admins;
        }

        private async Task<List<ApplicationUser>> GetHouseHelpsAsync()
        {
            List<ApplicationUser> houseHelps = new List<ApplicationUser>();
            foreach (ApplicationUser user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Administrators"))
                {
                    houseHelps.Add(user);
                }
            }
            return houseHelps;
        }

        private async Task<SystemSummaryViewModel> GetSystemSummaryAsync()
        {
            SystemSummaryViewModel systemSummary = new SystemSummaryViewModel();
            var houseHelps = await GetHouseHelpsAsync();

            systemSummary.TotalAdmins = (await GetAdminsAsync()).Count();
            systemSummary.TotalHouseHelps = houseHelps.Count();
            systemSummary.TotalUsers = _userManager.Users.Count();

            systemSummary.TotalReachOutsSent = 0;
            foreach (var user in houseHelps)
            {
                systemSummary.TotalReachOutsSent += user.ReachOuts.Count();
            }

            return systemSummary;
        }
    }
}
