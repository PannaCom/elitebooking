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
    public class ListServicesController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /ListServices/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.list_services.Where(o=>o.deleted==0).ToList());
        }

        //
        // GET: /ListServices/Details/5

        public ActionResult Details(int id = 0)
        {
            list_services list_services = db.list_services.Find(id);
            if (list_services == null)
            {
                return HttpNotFound();
            }
            return View(list_services);
        }

        //
        // GET: /ListServices/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /ListServices/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(list_services list_services)
        {
            if (ModelState.IsValid)
            {
                list_services.deleted = 0;
                db.list_services.Add(list_services);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(list_services);
        }

        //
        // GET: /ListServices/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_services list_services = db.list_services.Find(id);
            if (list_services == null)
            {
                return HttpNotFound();
            }
            return View(list_services);
        }

        //
        // POST: /ListServices/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(list_services list_services)
        {
            if (ModelState.IsValid)
            {
                list_services.deleted = 0;
                db.Entry(list_services).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(list_services);
        }

        //
        // GET: /ListServices/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_services list_services = db.list_services.Find(id);
            if (list_services == null)
            {
                return HttpNotFound();
            }
            return View(list_services);
        }

        //
        // POST: /ListServices/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            list_services list_services = db.list_services.Find(id);
            list_services.deleted = 1;
            db.Entry(list_services).State = EntityState.Modified;
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