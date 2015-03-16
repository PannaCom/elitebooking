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
    public class CompanyIntroController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CompanyIntro/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.company_introduction.ToList());
        }
        public string getCompanyIntro() {
            var p = (from q in db.company_introduction select q.fullcontent).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /CompanyIntro/Details/5

        public ActionResult Details(int id = 0)
        {
            company_introduction company_introduction = db.company_introduction.Find(id);
            if (company_introduction == null)
            {
                return HttpNotFound();
            }
            return View(company_introduction);
        }

        //
        // GET: /CompanyIntro/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /CompanyIntro/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(company_introduction company_introduction)
        {
            if (ModelState.IsValid)
            {
                db.company_introduction.Add(company_introduction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company_introduction);
        }

        //
        // GET: /CompanyIntro/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            company_introduction company_introduction = db.company_introduction.Find(id);
            if (company_introduction == null)
            {
                return HttpNotFound();
            }
            return View(company_introduction);
        }

        //
        // POST: /CompanyIntro/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(company_introduction company_introduction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company_introduction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company_introduction);
        }

        //
        // GET: /CompanyIntro/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            company_introduction company_introduction = db.company_introduction.Find(id);
            if (company_introduction == null)
            {
                return HttpNotFound();
            }
            return View(company_introduction);
        }

        //
        // POST: /CompanyIntro/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            company_introduction company_introduction = db.company_introduction.Find(id);
            db.company_introduction.Remove(company_introduction);
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