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
    public class GolfRuleController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /GolfRule/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.golf_rule.ToList());
        }

        //
        // GET: /GolfRule/Details/5

        public ActionResult Details(int id = 0)
        {
            golf_rule golf_rule = db.golf_rule.Find(id);
            if (golf_rule == null)
            {
                return HttpNotFound();
            }
            return View(golf_rule);
        }

        //
        // GET: /GolfRule/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /GolfRule/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(golf_rule golf_rule)
        {
            if (ModelState.IsValid)
            {
                db.golf_rule.Add(golf_rule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(golf_rule);
        }

        //
        // GET: /GolfRule/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf_rule golf_rule = db.golf_rule.Find(id);
            if (golf_rule == null)
            {
                return HttpNotFound();
            }
            return View(golf_rule);
        }

        //
        // POST: /GolfRule/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(golf_rule golf_rule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(golf_rule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(golf_rule);
        }

        //
        // GET: /GolfRule/Delete/5

        public ActionResult Delete(int id = 0)
        {
            golf_rule golf_rule = db.golf_rule.Find(id);
            if (golf_rule == null)
            {
                return HttpNotFound();
            }
            return View(golf_rule);
        }

        //
        // POST: /GolfRule/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            golf_rule golf_rule = db.golf_rule.Find(id);
            db.golf_rule.Remove(golf_rule);
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