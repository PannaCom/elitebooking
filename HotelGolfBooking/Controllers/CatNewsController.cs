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
    public class CatNewsController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CatNews/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.catnews.Where(o=>o.deleted==0).ToList());
        }

        //
        // GET: /CatNews/Details/5

        public ActionResult Details(int id = 0)
        {
            catnew catnew = db.catnews.Find(id);
            if (catnew == null)
            {
                return HttpNotFound();
            }
            return View(catnew);
        }

        //
        // GET: /CatNews/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /CatNews/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(catnew catnew)
        {
            if (ModelState.IsValid)
            {
                catnew.deleted = 0;
                db.catnews.Add(catnew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catnew);
        }

        //
        // GET: /CatNews/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            catnew catnew = db.catnews.Find(id);
            if (catnew == null)
            {
                return HttpNotFound();
            }
            return View(catnew);
        }

        //
        // POST: /CatNews/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(catnew catnew)
        {
            if (ModelState.IsValid)
            {
                catnew.deleted = 0;
                db.Entry(catnew).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(catnew);
        }

        //
        // GET: /CatNews/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            catnew catnew = db.catnews.Find(id);
            if (catnew == null)
            {
                return HttpNotFound();
            }
            return View(catnew);
        }

        //
        // POST: /CatNews/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            catnew catnew = db.catnews.Find(id);
            catnew.deleted = 1;
            db.Entry(catnew).State = EntityState.Modified;
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