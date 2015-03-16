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
    public class ListFacilitiesController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /ListFacilities/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.list_facilities.Where(o=>o.deleted==0).ToList());
        }

        //
        // GET: /ListFacilities/Details/5

        public ActionResult Details(int id = 0)
        {
            list_facilities list_facilities = db.list_facilities.Find(id);
            if (list_facilities == null)
            {
                return HttpNotFound();
            }
            return View(list_facilities);
        }

        //
        // GET: /ListFacilities/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /ListFacilities/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(list_facilities list_facilities)
        {
            if (ModelState.IsValid)
            {
                list_facilities.deleted = 0;
                db.list_facilities.Add(list_facilities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(list_facilities);
        }

        //
        // GET: /ListFacilities/Edit/5

        public ActionResult Edit(int id = 0)
        {
            list_facilities list_facilities = db.list_facilities.Find(id);
            if (list_facilities == null)
            {
                return HttpNotFound();
            }
            return View(list_facilities);
        }

        //
        // POST: /ListFacilities/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(list_facilities list_facilities)
        {
            if (ModelState.IsValid)
            {
                list_facilities.deleted = 0;
                db.Entry(list_facilities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(list_facilities);
        }

        //
        // GET: /ListFacilities/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_facilities list_facilities = db.list_facilities.Find(id);
            if (list_facilities == null)
            {
                return HttpNotFound();
            }
            return View(list_facilities);
        }

        //
        // POST: /ListFacilities/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            list_facilities list_facilities = db.list_facilities.Find(id);
            list_facilities.deleted = 1;
            db.Entry(list_facilities).State = EntityState.Modified;
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