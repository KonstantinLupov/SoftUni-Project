using C_Sharp_Project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace C_Sharp_Project.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
      

        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            using (var database = new ApplicationDbContext())
            {
                var users = database.Users
                    .ToList();

                var admins = GetAdminUserNames(users, database);
                ViewBag.Admins = admins;

                return View(users);
            }
        }

        private HashSet<string> GetAdminUserNames (List<ApplicationUser> users, ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if(userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }
            }

            return admins;  
        }

        public ActionResult Edit(string id)
        {
            // Validate Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ApplicationDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id == id)
                    .First();

                // Check if user exists
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Create a view model
                var viewModel = new EditUserViewModel();
                viewModel.User = user;
                viewModel.Roles = GetUserRoles(user, database);

                // Pass the model to the view
                return View(viewModel);

            }  
        }

        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {

            // Check if model is valid
            if (ModelState.IsValid)
            {
                using (var database = new ApplicationDbContext())
                {
                    var user = database.Users.FirstOrDefault(u => u.Id == id);

                    if(user == null)
                    {
                        return HttpNotFound();
                    }

                    if(!string.IsNullOrEmpty(viewModel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var passworhHash = hasher.HashPassword(viewModel.Password);
                        user.PasswordHash = passworhHash;
                    }

                    user.Email = viewModel.User.Email;
                    user.FullName = viewModel.User.FullName;
                    user.UserName = viewModel.User.Email;
                    this.SetUserRoles(user, database, viewModel);
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(viewModel);
        }

        public ActionResult Delete(string id)
        {
            // Validate Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ApplicationDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Check if user exists
                if (user == null)
                {
                    return HttpNotFound();
                }
    
               
                return View(user);

            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new ApplicationDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Get user players from database
                var userPlayers = database.Players
                    .Where(a => a.Author.Id == user.Id);

                // Delete user players
                foreach (var player in userPlayers)
                {
                    database.Players.Remove(player);
                }

                // Delete user and save changes;
                database.Users.Remove(user);
                database.SaveChanges();


                return RedirectToAction("List");
            }   
        }

        private void SetUserRoles(ApplicationUser user, ApplicationDbContext database, EditUserViewModel model)
        {
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            foreach (var role in model.Roles)
            {
                if(role.IsSelected)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else if (!role.IsSelected)
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private List<Role> GetUserRoles(ApplicationUser user, ApplicationDbContext database)
        {
            // Create user manager
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            // Get all application roles
            var roles = database.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            // For each application role, check if the user has it.
            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                var role = new Role { Name = roleName };

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            // Return a list with all roles
            return userRoles;
        }
    }
}