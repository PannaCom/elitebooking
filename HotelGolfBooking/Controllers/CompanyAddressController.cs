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
    public class CompanyAddressController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /CompanyAddress/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.company_address.ToList());
        }
        public string getCompanyAddress() {
            var p = (from q in db.company_address select q.fullcontent).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /CompanyAddress/Details/5

        public ActionResult Details(int id = 0)
        {
            company_address company_address = db.company_address.Find(id);
            if (company_address == null)
            {
                return HttpNotFound();
            }
            return View(company_address);
        }

        //
        // GET: /CompanyAddress/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /CompanyAddress/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(company_address company_address)
        {
            if (ModelState.IsValid)
            {
                db.company_address.Add(company_address);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company_address);
        }

        //
        // GET: /CompanyAddress/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            company_address company_address = db.company_address.Find(id);
            if (company_address == null)
            {
                return HttpNotFound();
            }
            return View(company_address);
        }

        //
        // POST: /CompanyAddress/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(company_address company_address)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company_address).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company_address);
        }

        //
        // GET: /CompanyAddress/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            company_address company_address = db.company_address.Find(id);
            if (company_address == null)
            {
                return HttpNotFound();
            }
            return View(company_address);
        }

        //
        // POST: /CompanyAddress/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            company_address company_address = db.company_address.Find(id);
            db.company_address.Remove(company_address);
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