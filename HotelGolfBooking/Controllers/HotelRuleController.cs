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
    public class HotelRuleController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelRule/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.hotel_rule.ToList());
        }

        //
        // GET: /HotelRule/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_rule hotel_rule = db.hotel_rule.Find(id);
            if (hotel_rule == null)
            {
                return HttpNotFound();
            }
            return View(hotel_rule);
        }

        //
        // GET: /HotelRule/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /HotelRule/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_rule hotel_rule)
        {
            if (ModelState.IsValid)
            {
                db.hotel_rule.Add(hotel_rule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_rule);
        }

        //
        // GET: /HotelRule/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_rule hotel_rule = db.hotel_rule.Find(id);
            if (hotel_rule == null)
            {
                return HttpNotFound();
            }
            return View(hotel_rule);
        }

        //
        // POST: /HotelRule/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_rule hotel_rule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_rule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel_rule);
        }

        //
        // GET: /HotelRule/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotel_rule hotel_rule = db.hotel_rule.Find(id);
            if (hotel_rule == null)
            {
                return HttpNotFound();
            }
            return View(hotel_rule);
        }

        //
        // POST: /HotelRule/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_rule hotel_rule = db.hotel_rule.Find(id);
            db.hotel_rule.Remove(hotel_rule);
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