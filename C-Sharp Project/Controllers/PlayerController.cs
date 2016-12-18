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
        public ActionResult Create ()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Player player)
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
       
        
    }
}