using C_Sharp_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace C_Sharp_Project.Controllers
{
    public class PlayerController : Controller
    {
        public ActionResult Index()
        {
            
          return RedirectToAction("List");
        }   

        public ActionResult List()
        {
            using (var database = new ApplicationDbContext())
            {
                var players = database.Players
                    .Include(a => a.Author)
                    .ToList();

                return View(players);
            }
            
        }

        public ActionResult Details(int ?id)
        {
            using (var database = new ApplicationDbContext())
            {
                var players = database.Players
                    .Where(p => p.Id == id)
                    .ToList().First();

                return View(players);
            }
        }

        [Authorize]
        public ActionResult Upload ()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult Upload(Player player)
        {
            if(ModelState.IsValid)
            {
                using (var database = new ApplicationDbContext())
                {
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    player.AuthorId = authorId;

                    database.Players.Add(player);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(player);
        }

        public ActionResult Delete (int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ApplicationDbContext())
            {
                // Get player from database
                var player = database.Players
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

               
                //Check if player exists
                if (player == null)
                {
                    return HttpNotFound();
                }
                //Pass player to view
                return View(player);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed (int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ApplicationDbContext())
            {
                var player = database.Players
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (player == null)
                {
                    return HttpNotFound();
                }

                database.Players.Remove(player);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ApplicationDbContext())
            {
                var player = database.Players
                    .Where(a => a.Id == id)
                    .First();

               

                if (player == null)
                {
                    return HttpNotFound();
                }

                var model = new PlayerViewModel();
                model.Id = player.Id;
                model.Name = player.Name;
                model.Content = player.Content;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit (PlayerViewModel model)
        {
            if(ModelState.IsValid)
            {
                using (var database = new ApplicationDbContext())
                {
                    var player = database.Players
                        .FirstOrDefault(a => a.Id == model.Id);

                    player.Name = model.Name;
                    player.Content = model.Content;

                    database.Entry(player).State = EntityState.Modified;
                    database.SaveChanges();
                    
                }
                
            }
            return RedirectToAction("Index");
        }
       
      
        
    }
}