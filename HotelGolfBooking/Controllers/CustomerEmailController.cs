using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;

namespace HotelGolfBooking.Controllers
{
    public class CustomerEmailController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CustomerEmail/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.customer_list_email.OrderByDescending(o=>o.id).ToList());
        }

        //
        // GET: /CustomerEmail/Details/5
        [HttpPost]
        public string newsletter(customer_list_email customer_list_email,string cemail) {

            customer_list_email.cemail = cemail;
            customer_list_email.date = DateTime.Now;
            db.customer_list_email.Add(customer_list_email);
            db.SaveChanges();
            return "1";
        }
        public ActionResult Details(int id = 0)
        {
            customer_list_email customer_list_email = db.customer_list_email.Find(id);
            if (customer_list_email == null)
            {
                return HttpNotFound();
            }
            return View(customer_list_email);
        }

        //
        // GET: /CustomerEmail/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /CustomerEmail/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(customer_list_email customer_list_email)
        {
            if (ModelState.IsValid)
            {
                customer_list_email.date = DateTime.Now;
                db.customer_list_email.Add(customer_list_email);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer_list_email);
        }

        //
        // GET: /CustomerEmail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            customer_list_email customer_list_email = db.customer_list_email.Find(id);
            if (customer_list_email == null)
            {
                return HttpNotFound();
            }
            return View(customer_list_email);
        }

        //
        // POST: /CustomerEmail/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(customer_list_email customer_list_email)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer_list_email).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer_list_email);
        }

        //
        // GET: /CustomerEmail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            customer_list_email customer_list_email = db.customer_list_email.Find(id);
            if (customer_list_email == null)
            {
                return HttpNotFound();
            }
            return View(customer_list_email);
        }

        //
        // POST: /CustomerEmail/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer_list_email customer_list_email = db.customer_list_email.Find(id);
            db.customer_list_email.Remove(customer_list_email);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}