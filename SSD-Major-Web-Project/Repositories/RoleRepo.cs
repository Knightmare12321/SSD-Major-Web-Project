using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class RoleRepo
    {
        private readonly NovaDbContext _db;

        public RoleRepo(NovaDbContext context)
        {
            this._db = context;
            CreateInitialRole();
        }

        public IEnumerable<RoleVM> GetAllRoles()
        {
            var roles =
                _db.UserTypes.Select(r => new RoleVM
                {
                    Id = r.PkUserTypeId.ToString(),
                    RoleName = r.UserType1
                });

            return roles;
        }

        public RoleVM GetRole(string roleName)
        {


            var role = _db.UserTypes.Where(r => r.UserType1 == roleName)
                                .FirstOrDefault();

            if (role != null)
            {
                return new RoleVM() { RoleName = role.UserType1 };
            }
            return null;
        }

        public bool CreateRole(string roleName, string id)
        {
            bool isSuccess = true;

            try
            {
                _db.UserTypes.Add(new UserType
                {
                    PkUserTypeId = Int32.Parse(id),
                    UserType1 = roleName.ToLower(),
                });
                _db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public SelectList GetRoleSelectList()
        {
            var roles = GetAllRoles().Select(r => new
            SelectListItem
            {
                Value = r.RoleName,
                Text = r.RoleName
            });

            var roleSelectList = new SelectList(roles,
                                               "Value",
                                               "Text");
            return roleSelectList;
        }

        public void CreateInitialRole()
        {
            const string ADMIN = "Admin";
            const string ID = "Ad";

            var role = GetRole(ADMIN);

            if (role == null)
            {
                CreateRole(ADMIN, ID);
            }
        }

        // Logic for role deletion can be included here.
        public string Delete(string roleName)
        {
            var role = _db.UserTypes.FirstOrDefault(r => r.UserType1 == roleName);
            if (role == null)
            {
                return "Role does not exist";
            }
            if (_db.UserTypes.Any(ur => ur.UserType1 == role.UserType1.ToLower()))
            {
                return "this role is currently in use";
            }
            _db.UserTypes.Remove(role);
            _db.SaveChanges();
            return "Deleted successfully";
        }
    }
}
