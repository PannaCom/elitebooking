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
    public class CompanyBankingController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CompanyBanking/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.company_banking.ToList());
        }
       
        public string getCompanyBanking() {
            var p = (from q in db.company_banking select q.fullcontent).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /CompanyBanking/Details/5

        public ActionResult Details(int id = 0)
        {
            company_banking company_banking = db.company_banking.Find(id);
            if (company_banking == null)
            {
                return HttpNotFound();
            }
            return View(company_banking);
        }

        //
        // GET: /CompanyBanking/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /CompanyBanking/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(company_banking company_banking)
        {
            if (ModelState.IsValid)
            {
                db.company_banking.Add(company_banking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company_banking);
        }

        //
        // GET: /CompanyBanking/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            company_banking company_banking = db.company_banking.Find(id);
            if (company_banking == null)
            {
                return HttpNotFound();
            }
            return View(company_banking);
        }

        //
        // POST: /CompanyBanking/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(company_banking company_banking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company_banking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company_banking);
        }

        //
        // GET: /CompanyBanking/Delete/5

        public ActionResult Delete(int id = 0)
        {
            company_banking company_banking = db.company_banking.Find(id);
            if (company_banking == null)
            {
                return HttpNotFound();
            }
            return View(company_banking);
        }

        //
        // POST: /CompanyBanking/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            company_banking company_banking = db.company_banking.Find(id);
            db.company_banking.Remove(company_banking);
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