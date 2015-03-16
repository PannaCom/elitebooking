using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using Newtonsoft.Json;
namespace HotelGolfBooking.Controllers
{
    public class HotLineController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotLine/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.hotlines.ToList());
        }
        public string getCompanyHotLine()
        {
            var p = (from q in db.hotlines select q).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /HotLine/Details/5

        public ActionResult Details(int id = 0)
        {
            hotline hotline = db.hotlines.Find(id);
            if (hotline == null)
            {
                return HttpNotFound();
            }
            return View(hotline);
        }

        //
        // GET: /HotLine/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /HotLine/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotline hotline)
        {
            if (ModelState.IsValid)
            {
                db.hotlines.Add(hotline);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotline);
        }

        //
        // GET: /HotLine/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotline hotline = db.hotlines.Find(id);
            if (hotline == null)
            {
                return HttpNotFound();
            }
            return View(hotline);
        }

        //
        // POST: /HotLine/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotline hotline)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotline).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotline);
        }

        //
        // GET: /HotLine/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotline hotline = db.hotlines.Find(id);
            if (hotline == null)
            {
                return HttpNotFound();
            }
            return View(hotline);
        }

        //
        // POST: /HotLine/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotline hotline = db.hotlines.Find(id);
            db.hotlines.Remove(hotline);
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