using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.Repositories;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public ActionResult Index(string message)
        {
            ViewBag.message = message ?? "";

            RoleRepo roleRepo = new RoleRepo(_db);
            return View(roleRepo.GetAllRoles());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                RoleRepo roleRepo = new RoleRepo(_db);
                bool isSuccess =
                    roleRepo.CreateRole(roleVM.RoleName);

                if (isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState
                    .AddModelError("", "Role creation failed.");
                    ModelState
                    .AddModelError("", "The role may already" +
                                       " exist.");
                }
            }

            return View(roleVM);
        }

        [HttpGet]
        public ActionResult Delete(string roleName)
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            RoleVM roleVM = roleRepo.GetRole(roleName);
            return View(roleVM);
        }

        [HttpPost]
        public ActionResult Delete(RoleVM roleVM)
        {
            RoleRepo roleRepo = new RoleRepo(_db);
            string message = roleRepo.Delete(roleVM.RoleName);

            if (message.Contains("success"))
            {
                return RedirectToAction("Index", new { message = message });
            }
            else
            {
                ModelState
                .AddModelError("", "Role deletion failed.");
                ModelState
                .AddModelError("", message);

                return View(roleVM);
            }

        }
    }
}
