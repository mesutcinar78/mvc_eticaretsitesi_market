using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeTicaret.Models;

namespace MVCeTicaret.Controllers
{
    public class ProductController : Controller
    {
        Context db;

        public ProductController() //ctor yaptim db nesnesi yarattim 
        {
            db = new Context();
        }



        // GET: Product asagidaki index i product yaptim product view olacak
        public ActionResult Product(int id)
        {
            return View(db.Products.Where(x=> x.SubCategoryID==id).ToList());//liste olarak view in icine attim
        }
        //asagidaki satirlari ben yazdim product detail i product sayfasinda action olarak olusturdum burda kullaniyorum
        public ActionResult ProductDetail (int id)
        {
            ViewData["Reviews"] = db.Reviews.Where(x => x.ProductID == id && x.IsDeleted==false).ToList();
            //burada yorumlari çekmek istiyorum ve silindimi sorusunun cevabi false olanlari göster dedim sql tarafinda reviews kisminda customer id ve product idleri düzenledim oradaki isdeleted i false yaptim
            return View(db.Products.Find(id));
        }

        [HttpPost]
        public ActionResult AddReview(int id,FormCollection frm)
        {

            Review review = new Review()
            {
                Comment = frm["review"],
                CustomerID = TemporaryUserData.UserID,
                DateTime = DateTime.Now,
                ProductID = id,
                Name = frm["name"] == "" ? "Anonymous" : frm["name"],
                Rate = int.Parse(frm["rate"])

            };

            db.Reviews.Add(review);
            db.SaveChanges();
            return RedirectToAction("ProductDetail", new { id = id });
            //ayni sayfada kalacagi icin redirect to action dedim,product detail sayfasinda yorum ekle kismi yapiyorum
        }

        //yorum ekle kismina bastigimizda neler olacak onu da simdi yapiyoruz action olusturarak
        
       
    }
}