using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Controllers
{
    /// <summary>
    /// Contains all the operations that can be done by an anonymous user looking for househelp services
    /// </summary>
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Uses dependency injection to get the required services
        /// </summary>
        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Lists all the available househelps that fit the passed search parameter
        /// </summary>
        public IActionResult Index(string searchParameter = null)
        {
            if (searchParameter != null)
                return View(_userManager.Users
                    .Where(user => user.isAvailable && (user.UserName.Contains(searchParameter) || user.Name.Contains(searchParameter) || user.Description.Contains(searchParameter))));
            return View(_userManager.Users.Where(user => user.isAvailable));
        }

        /// <summary>
        /// Displays the reach out form
        /// </summary>
        public IActionResult ReachOut(string houseHelpId) => View(new ReachOutViewModel() { HouseHelpId = houseHelpId });

        /// <summary>
        /// Saves the reach out request to the database if the data passed is valid
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ReachOut(ReachOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.HouseHelpId);

                if (user != null)
                {
                    ReachOutModel reachOut = new ReachOutModel
                    {
                        Name = model.Name,
                        Phone = model.Phone,
                        Description = model.Description
                    };

                    user.ReachOuts.Add(reachOut);
                    _context.Update(user);
                    _context.SaveChanges();
                    TempData["message"] = $"Reach out request sent to {user.UserName}";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }
            return View(model);
        }
    }
}
