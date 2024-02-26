using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    [Authorize(Roles = "Admin, Manager")]

    public class UserRoleController : Controller
    {
        private readonly NovaDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRoleController(NovaDbContext context,
                                 UserManager<IdentityUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            UserRepo userRepo = new UserRepo(_db);
            IEnumerable<UserVM> customers = userRepo.GetAllUsers();

            return View(customers);
        }

        public async Task<IActionResult> Detail(string userName,
                                                string message = "")
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);


            var roles = await userRoleRepo.GetUserRolesAsync(userName);
            CustomerRepo customerRepo = new CustomerRepo(_db);
            var userFullName = customerRepo.GetUsername(userName);

            ViewBag.UserName = userFullName;
            ViewBag.Message = message;
            ViewBag.Email = userName;


            return View(roles);
        }

        public ActionResult Create()
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            ViewBag.RoleSelectList = roleRepo.GetRoleSelectList();


            UserRepo userRepo = new UserRepo(_db);
            ViewBag.UserSelectList = userRepo.GetUserSelectList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleVM userRoleVM)
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);

            if (ModelState.IsValid)
            {
                try
                {
                    var addUR =
                    await userRoleRepo.AddUserRoleAsync(userRoleVM.Email,
                                                        userRoleVM.RoleName);

                    string message = $"{userRoleVM.RoleName} permissions" +
                                     $" successfully added to " +
                                     $"{userRoleVM.Email}.";

                    return RedirectToAction("Detail", "UserRole",
                                      new
                                      {
                                          userName = userRoleVM.Email,
                                          message = message
                                      });
                }
                catch
                {
                    ModelState.AddModelError("", "UserRole creation failed.");
                    ModelState.AddModelError("", "The Role may exist " +
                                                 "for this user.");
                }
            }

            RoleRepo roleRepo = new RoleRepo(_db);
            ViewBag.RoleSelectList = roleRepo.GetRoleSelectList();

            UserRepo userRepo = new UserRepo(_db);
            ViewBag.UserSelectList = userRepo.GetUserSelectList();

            return View();
        }

        [HttpGet]
        public ActionResult Delete(string email, string roleName)
        {
            UserRoleVM userRoleVM = new UserRoleVM() { RoleName = roleName, Email = email };
            return View(userRoleVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(UserRoleVM userRoleVM)
        {
            UserRoleRepo userRoleRepo = new UserRoleRepo(_userManager);

            if (ModelState.IsValid)
            {
                try
                {
                    var addUr = await userRoleRepo.RemoveUserRoleAsync(userRoleVM.Email, userRoleVM.RoleName);

                    string message = "Role has been successfully removed!";

                    return RedirectToAction("Detail", "UserRole",
                                    new
                                    {
                                        userName = userRoleVM.Email,
                                        message = message
                                    });
                }
                catch
                {
                    ModelState.AddModelError("", "Role deletion failed.");
                }
            }
            return RedirectToAction("Detail", "UserRole",
                        new
                        {
                            message = "Role could not be removed"
                        });
        }
    }
}
