using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using PagedList;
namespace HotelGolfBooking.Controllers
{
    public class CustomerRequestController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CustomerRequest/

        public ActionResult Index(string phone, int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (phone == null) phone = "";
            ViewBag.phone = phone;
            var p = (from q in db.customer_hotel_price where q.phone.Contains(phone) select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /CustomerRequest/Details/5

        public ActionResult Details(int id = 0)
        {
            customer_hotel_price customer_hotel_price = db.customer_hotel_price.Find(id);
            if (customer_hotel_price == null)
            {
                return HttpNotFound();
            }
            return View(customer_hotel_price);
        }

        //
        // GET: /CustomerRequest/Create

        public ActionResult Create()
        {
            if (Request.Browser.IsMobileDevice)
            {
                ViewBag.ismobile = 1;
            }
            else
            {
                ViewBag.ismobile = 0;
            }
            return View();
        }
        public ActionResult Confirm() {

            return View();
        }
        //
        // POST: /CustomerRequest/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(customer_hotel_price customer_hotel_price)
        {
            if (ModelState.IsValid)
            {
                db.customer_hotel_price.Add(customer_hotel_price);
                db.SaveChanges();
                return RedirectToAction("Confirm");
            }

            return View(customer_hotel_price);
        }

        //
        // GET: /CustomerRequest/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            customer_hotel_price customer_hotel_price = db.customer_hotel_price.Find(id);
            if (customer_hotel_price == null)
            {
                return HttpNotFound();
            }
            return View(customer_hotel_price);
        }

        //
        // POST: /CustomerRequest/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(customer_hotel_price customer_hotel_price)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer_hotel_price).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer_hotel_price);
        }

        //
        // GET: /CustomerRequest/Delete/5

        public ActionResult Delete(int id = 0)
        {
            customer_hotel_price customer_hotel_price = db.customer_hotel_price.Find(id);
            if (customer_hotel_price == null)
            {
                return HttpNotFound();
            }
            return View(customer_hotel_price);
        }

        //
        // POST: /CustomerRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer_hotel_price customer_hotel_price = db.customer_hotel_price.Find(id);
            db.customer_hotel_price.Remove(customer_hotel_price);
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