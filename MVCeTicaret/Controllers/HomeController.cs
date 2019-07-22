using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeTicaret.Models;

namespace MVCeTicaret.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //anasayfaya random ürün getirme take 4 tane getirecek her birinden 4 tane guid benzersiz farklı 4 tane getirecek
            Context db = new Context();

            TempData["Meyve"] = db.Products.Where(x => x.SubCategoryID == 2).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            TempData["Sebze"] = db.Products.Where(x => x.SubCategoryID == 3).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            TempData["Pet"] = db.Products.Where(x => x.SubCategoryID == 4).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            return View();
        }
    }
}