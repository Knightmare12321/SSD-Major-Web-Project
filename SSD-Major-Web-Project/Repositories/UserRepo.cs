using Microsoft.AspNetCore.Mvc.Rendering;
using SSD_Major_Web_Project.Data;
using SSD_Major_Web_Project.Models;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Repositories
{
    public class UserRepo
    {
        private readonly NovaDbContext _db;

        public UserRepo(NovaDbContext db)
        {
            this._db = db;
        }

        public IEnumerable<UserVM> GetAllUsers()
        {
            IEnumerable<UserVM> users =
            _db.Customers.Select(u => new UserVM { Email = u.PkCustomerId });

            return users;
        }

        public SelectList GetUserSelectList()
        {
            IEnumerable<SelectListItem> users =
                GetAllUsers().Select(u => new SelectListItem
                {
                    Value = u.Email,
                    Text = u.Email
                });

            SelectList roleSelectList = new SelectList(users,
                                                      "Value",
                                                      "Text");
            return roleSelectList;
        }
    }

}
