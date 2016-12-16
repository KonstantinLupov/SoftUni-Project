using C_Sharp_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
       
        
    }
}