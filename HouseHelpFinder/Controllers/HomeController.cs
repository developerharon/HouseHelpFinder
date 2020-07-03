using HouseHelpFinder.Models;
using HouseHelpFinder.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseHelpFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index(string searchParameter = null)
        {
            if (searchParameter != null)
                return View(_userManager.Users
                    .Where(user => user.isAvailable && (user.UserName.Contains(searchParameter) || user.Name.Contains(searchParameter) || user.Description.Contains(searchParameter))));
            return View(_userManager.Users.Where(user => user.isAvailable));
        }

        public IActionResult ReachOut(string houseHelpId) => View(new ReachOutViewModel() { HouseHelpId = houseHelpId });

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
                    return RedirectToAction("List");
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
