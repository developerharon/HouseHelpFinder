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
    /// <summary>
    /// Contains all the administrative functions of the system
    /// </summary>
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Uses dependency injection to get the required services
        /// </summary>
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        /// Displays the system summary
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await GetSystemSummaryAsync());
        }

        /// <summary>
        /// Lists all the administrators of the system.
        /// </summary>
        public async Task<IActionResult> ListAdmins()
        {
            return View(await GetAdminsAsync());
        }

        /// <summary>
        /// Lists all the househelp registered with the system
        /// </summary>
        public async Task<IActionResult> ListHouseHelps(string searchParameter = null)
        {
            var houseHelps = await GetHouseHelpsAsync();

            if (searchParameter != null)
                return View(houseHelps.Where(user => user.UserName.Contains(searchParameter)));

            return View(houseHelps);
        }

        /// <summary>
        /// Renders the view used to create an administrative account.
        /// </summary>
        public IActionResult Create() => View();

        /// <summary>
        /// Creates a new administrator account in the database
        /// </summary>
        [HttpPost]
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

                    if (!await _roleManager.RoleExistsAsync("Administrators"))
                    {
                        result = await _roleManager.CreateAsync(new IdentityRole("Administrators"));

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Error occured when creating the account");
                            return View(model);
                        }
                    }

                    result = await _userManager.AddToRoleAsync(user, "Administrators");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Error occured when creating the account");
                        return View(model);
                    }

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
        /// Delete the user/admin account associated with the particular ID
        /// </summary>
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

        /// <summary>
        /// Returns a list of the administrators of the system
        /// </summary>
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

        /// <summary>
        /// Returns a list of all the househelps registered with the system.
        /// </summary>
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

        /// <summary>
        /// Returns an object representing the system summary
        /// </summary>
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
