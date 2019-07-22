using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeTicaret.Models;
//son using i ekledim

namespace MVCeTicaret.Controllers
{
    //browser açilinca view dir karşima gelen alt kategoriye bastigim anda model devreye girer model den controller a geciyor(veriyi cekip action view leri calistirip)

        //benim http post yapma sebebim 




    public class ShoppingController : Controller
    {
        // GET: Shopping
        Context db;
        public ShoppingController()
        {
            db = new Context();
        }



        public ActionResult Cart()//sepeti listeleme kismi
        {
            return View(db.OrderDetails.Where(x => x.IsCompleted == false && x.CustomerID == TemporaryUserData.UserID).ToList());
        }

        [HttpPost]
        public ActionResult ShoppingAddToCart(int id, FormCollection frm)// ürün detayinaki sepete ekle butona bastigimda olan islemler denilen kisim,//aslinda hocada bunun adi add tocart

            //fomcollection  da miktar verisi aldigimiz icin kulllaniyoruz
        {
            //Sepete ekle demek,formun icindeki verileri almak icin form collection frm dedik httppostu ogren

            if (Session["OnlineKullanici"] == null)
                return RedirectToAction("Login", "Login");

            int miktar = Convert.ToInt32(frm["quantity"]);

            ControlCart(id, miktar);//anlamadım

            return RedirectToAction("ProductDetail", "Product", new { id = id });
        }

        public ActionResult RemoveFromCart(int id)//sepetim sayfasindaki sepetten çıkar icon kismi
        {
            db.OrderDetails.Remove(db.OrderDetails.Find(id));
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());//aynı sayfada kalma nin diğer yolu redirect toaction daki gibi
        }



        public ActionResult AddToWishlistFromCart(int id)//sepetim sayfasindaki istek listeme ekle iconu bastigimda olacaklar kismi
        {

            int productId = db.OrderDetails.Find(id).ProductID;
            ControlWishlist(productId);

            db.OrderDetails.Remove(db.OrderDetails.Find(id));
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        
        public ActionResult AddToWishList(int id)//ürün detayindaki istek listesine ekle butonu kisminda yapilacakalr
        {

            if (Session["OnlineKullanici"] == null)
                return RedirectToAction("Login", "Login");

            ControlWishlist(id);//metot olusturuyorum asagida icini doldurdum


            return RedirectToAction("ProductDetail", "Product", new { id = id });
        }

        




        public ActionResult WishList()//istek listesinde ne varsa listelicem parametre ye gerek yok,navbardaki genel istek listem kisim
        {
            return View(db.WishLists.Where(x => x.IsActive == true && x.CustomerID == TemporaryUserData.UserID).ToList());
        }

        public ActionResult RemoveFromList(int id)//istek listem sayfasindaki listeden cikar icon kisminda yapilacaklar
        {
            WishList wishlist = db.WishLists.Find(id);
            wishlist.IsActive = false;

            db.SaveChanges();

            return RedirectToAction("WishList", "Shopping");
        }

        public ActionResult AddToCartFromWishlist(int id)//istek listem sayfasindaki sepete ekle iconu na bastigimda yapilacakalr kismi
        {
            int productId = db.WishLists.Find(id).ProductID;
            ControlCart(productId);

            WishList wishlist = db.WishLists.Find(id);
            wishlist.IsActive = false;
            db.SaveChanges();

            return RedirectToAction("WishList", "Shopping");
        }


        #region Metotlar

        public void ControlCart(int id, int miktar = 1)//anlamadım
        {
            OrderDetail od = db.OrderDetails.Where(x => x.ProductID == id && x.IsCompleted == false && x.CustomerID == TemporaryUserData.UserID).FirstOrDefault();

            if (od == null)//ilk kayit
            {
                od = new OrderDetail();
                od.ProductID = id;
                od.CustomerID = TemporaryUserData.UserID;//todo dinnamik hale getirilmedi
                od.IsCompleted = false;
                od.UnitPrice = db.Products.Find(id).UnitPrice;
                od.Discount = db.Products.Find(id).Discount;
                od.OrderDate = DateTime.Now;

                if (db.Products.Find(id).UnitsInStock >= miktar)
                    od.Quantity = miktar;
                else
                    od.Quantity = db.Products.Find(id).UnitsInStock;

                od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);
                db.OrderDetails.Add(od);

            }

            else//update
            {
                if (db.Products.Find(id).UnitsInStock > od.Quantity + miktar)
                {
                    od.Quantity += miktar;
                    od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);
                }
            }

            db.SaveChanges();
        }

        private void ControlWishlist(int id)
        {
            WishList wishlist = db.WishLists.FirstOrDefault(x => x.ProductID == id && x.CustomerID == TemporaryUserData.UserID && x.IsActive == true);


            //isactive istek listemde mi diye evet dedim
            if (wishlist == null)
            {
                wishlist = new WishList();
                wishlist.ProductID = id;
                wishlist.CustomerID = TemporaryUserData.UserID;//dinamik hale getirilecek
                wishlist.IsActive = true;

                db.WishLists.Add(wishlist);
                db.SaveChanges();
            }
        }

        #endregion


        [HttpPost]
        public ActionResult UpdateQuantity(int id,FormCollection frm)//sepetim sayfasindaki miktar güncelle kisminda yapilacakalr 
        {

            OrderDetail od = db.OrderDetails.Find(id);
            od.Quantity = int.Parse(frm["quantity"]);
            od.TotalAmount = od.Quantity * od.UnitPrice * (1 - od.Discount);

            db.SaveChanges();
            
            
            return Redirect(Request.UrlReferrer.ToString());
        }





        // @* TODO: Siparişi Tamamla için action yazdık!.. *@
        public ActionResult GoToPayment()
        {
            List<OrderDetail> cart = db.OrderDetails.Where(x => x.IsCompleted == false && x.CustomerID == TemporaryUserData.UserID).ToList();

            foreach (var item in cart)
            {
                if (item.Product.UnitsInStock < item.Quantity)
                    return RedirectToAction("Cart","Shopping");
            }

            ViewBag.OrderDetails = cart;
            ViewBag.PaymentTypes = db.PaymentTypes.ToList();

            //kullanıcının bilgilerini gönderiyorum view a
            return View(db.Customers.Find(TemporaryUserData.UserID));
        }


        [HttpPost]
        //siparis tamamlandiysa
        public ActionResult CompleteShopping(FormCollection frm)
        {
            Payment payment = new Payment()
            {
                Type = int.Parse(frm["paymentType"]),
                Balance = 50000,
                CreditAmount = 50000,
                DebitAmount = 50000,
                PaymentDateTime = DateTime.Now
            };

            db.Payments.Add(payment);

            if(frm["update"]=="on")//burada on yapmamızın sebebi radio button da on geliyor ama neden 
            {
                Customer customer = db.Customers.Find(TemporaryUserData.UserID);

                customer.FirstName = frm["FirstName"];
                customer.LastName = frm["LastName"];
                customer.Address = frm["Address"];
                customer.City = frm["City"];
            }

            ShippingDetail sp = new ShippingDetail()
            {
                FirstName = frm["FirstName"],
                LastName = frm["LastName"],
                Address = frm["Address"],
                City = frm["City"]
            };

            db.ShippingDetails.Add(sp);
            db.SaveChanges();

            List<OrderDetail> cart = db.OrderDetails.Where(x => x.IsCompleted == false && x.CustomerID == TemporaryUserData.UserID).ToList();

            foreach (var item in cart)
            {
                item.IsCompleted = true;
                //sepetteki bir ürünün tipi order detail

                item.Product.UnitsInStock -= item.Quantity;

                Order order = new Order()
                {
                    PaymentID = payment.PaymentID,
                    ShippingID = sp.ShippingID,
                    OrderDetailID = item.OrderDetailID,
                    Discount = item.Discount,
                    TotalAmount = item.TotalAmount,
                    IsCompleted = true,
                    OrderDate = DateTime.Now,
                    Dispatched = false,
                    DispatchDate = DateTime.Now.AddDays(3),
                    Shipped = false,
                    ShippedDate = DateTime.Now.AddDays(4),
                    Deliver = false,
                    DeliveryDate = DateTime.Now.AddDays(5),
                    CancelOrder = false
                };

                db.Orders.Add(order);
            }

            db.SaveChanges();
            return RedirectToAction("FinishShopping", "Shopping");
        }

        public ActionResult FinishShopping()
        {
            return View();
        }
    }
}