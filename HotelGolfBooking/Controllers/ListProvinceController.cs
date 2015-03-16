using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using Newtonsoft.Json;
using PagedList;
namespace HotelGolfBooking.Controllers
{
    public class ListProvinceController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /ListProvince/

        public ActionResult Index(string provin,int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (provin == null) provin = "";
            ViewBag.provin = provin;
            var p = (from q in db.list_provin where q.provin.Contains(provin) && q.deleted == 0 select q).OrderBy(o => o.provin).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        public string getList() {
            return JsonConvert.SerializeObject(db.list_provin.Where(o=>o.deleted==0).ToList());
        }
        [HttpPost]
        public string getDisList(string provin)
        {
            return JsonConvert.SerializeObject(db.list_address.Where(o => o.provin == provin).Where(o => o.deleted == 0).ToList());
        }
        //
        // GET: /ListProvince/Details/5

        public ActionResult Details(int id = 0)
        {
            list_provin list_provin = db.list_provin.Find(id);
            if (list_provin == null)
            {
                return HttpNotFound();
            }
            return View(list_provin);
        }

        //
        // GET: /ListProvince/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ListProvince/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(list_provin list_provin)
        {
            if (ModelState.IsValid)
            {
                list_provin.deleted = 0;
                db.list_provin.Add(list_provin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(list_provin);
        }

        //
        // GET: /ListProvince/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_provin list_provin = db.list_provin.Find(id);
            if (list_provin == null)
            {
                return HttpNotFound();
            }
            return View(list_provin);
        }

        //
        // POST: /ListProvince/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(list_provin list_provin)
        {
            if (ModelState.IsValid)
            {
                list_provin.deleted = 0;
                db.Entry(list_provin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(list_provin);
        }

        //
        // GET: /ListProvince/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_provin list_provin = db.list_provin.Find(id);
            if (list_provin == null)
            {
                return HttpNotFound();
            }
            return View(list_provin);
        }

        //
        // POST: /ListProvince/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            list_provin list_provin = db.list_provin.Find(id);
            //db.list_provin.Remove(list_provin);
            //db.SaveChanges();
            list_provin.deleted = 1;
            db.Entry(list_provin).State = EntityState.Modified;
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