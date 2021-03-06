﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCeTicaret.Models;

namespace MVCeTicaret.Controllers
{
    public class LoginController : Controller
    {
        Context db;
        public LoginController()
        {
            db = new Context();
        }


        [HttpGet]
        public ActionResult Login()
        {
            TemporaryUserData.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();

            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            string kullaniciAdi = frm["username"];
            string sifre = frm["password"];

            Customer customer = db.Customers.FirstOrDefault(x => x.UserName == kullaniciAdi && x.Password == sifre);

            if (customer != null)
            {
                Session["OnlineKullanici"] = customer.UserName;
                TemporaryUserData.UserID = customer.CustomerID;

                if (TemporaryUserData.ReturnUrl.Contains("Register"))//Register dan geliyorsa ana sayfaya gidecek aksi halde registere gidecek
                    return RedirectToAction("Index", "Home");

                return Redirect(TemporaryUserData.ReturnUrl);
            }

            return View();
        }

        public ActionResult Logout()
        {
            db.Customers.Find(TemporaryUserData.UserID).LastLogin = DateTime.Now;
            db.SaveChanges();

            Session["OnlineKullanici"] = null;
            TemporaryUserData.UserID = 0;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            TemporaryUserData.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection frm)
        {
            string kullaniciAdi = frm["username"];
            Customer customer = db.Customers.FirstOrDefault(x => x.UserName == kullaniciAdi);

            if (customer != null)
                return View();

            else
            {
                customer = new Customer();
                customer.FirstName = frm["name"];
                customer.LastName = frm["surname"];
                customer.UserName = kullaniciAdi;
                customer.Password = frm["password"];
                customer.Gender = frm["gender"] == "on" ? true : false;
                customer.BirtDate = DateTime.Parse(frm["birthdate"]);
                customer.CreatedDate = DateTime.Now;
                customer.LastLogin = DateTime.Now;

                db.Customers.Add(customer);
                db.SaveChanges();

                Session["OnlineKullanici"] = kullaniciAdi;
                TemporaryUserData.UserID = customer.CustomerID;

                return Redirect(TemporaryUserData.ReturnUrl);
            }
        }
    }
}
//public ActionResult LogOut()
//{
//    db.Customers.Find(TemporaryUserData.UserID).LastLogin = DateTime.Now;
//    //kullannıcının son giris yaptigi id uzerinden son giris saatini kaydedicek
//    db.SaveChanges();

//    Session["OnlineKullanici"] = null;//kullanıcı offline ise
//    TemporaryUserData.UserID = 0;
//    //online olan kullanıcının id si asla sahip olamayacagi bir deger yaptim -1 yada 0
//    //return View();
//    return RedirectToAction("Index", "Home");
//}