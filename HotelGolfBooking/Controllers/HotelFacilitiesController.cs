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
    public class HotelFacilitiesController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelFacilities/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            int idhotel = int.Parse(Session["idhotel"].ToString());
            var p = (from q in db.hotel_facility where q.idhotel == idhotel select q);
            return View(p.ToList());
        }
        [HttpPost]
        public string getFacilities()
        {            
            var p = (from q in db.list_facilities where q.deleted==0 select q).OrderBy(o=>o.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string getHotelFacilities(int idhotel)
        {
            var p = (from q in db.hotel_facility where q.idhotel == idhotel select q).OrderBy(o => o.facility);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string insertFacilities(int idhotel,string facility) {
            try
            {
                string query = "insert into hotel_facility(idhotel,facility) values(" + idhotel + ",N'" + facility + "')";
                db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        //
        // GET: /HotelFacilities/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_facility hotel_facility = db.hotel_facility.Find(id);
            if (hotel_facility == null)
            {
                return HttpNotFound();
            }
            return View(hotel_facility);
        }

        //
        // GET: /HotelFacilities/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /HotelFacilities/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_facility hotel_facility)
        {
            if (ModelState.IsValid)
            {
                db.hotel_facility.Add(hotel_facility);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_facility);
        }

        //
        // GET: /HotelFacilities/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_facility hotel_facility = db.hotel_facility.Find(id);
            if (hotel_facility == null)
            {
                return HttpNotFound();
            }
            return View(hotel_facility);
        }

        //
        // POST: /HotelFacilities/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_facility hotel_facility)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_facility).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel_facility);
        }

        //
        // GET: /HotelFacilities/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_facility hotel_facility = db.hotel_facility.Find(id);
            if (hotel_facility == null)
            {
                return HttpNotFound();
            }
            return View(hotel_facility);
        }

        //
        // POST: /HotelFacilities/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_facility hotel_facility = db.hotel_facility.Find(id);
            db.hotel_facility.Remove(hotel_facility);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public string removeFacility(int id) {
            try{
                hotel_facility hotel_facility = db.hotel_facility.Find(id);
                db.hotel_facility.Remove(hotel_facility);
                db.SaveChanges();
            }catch(Exception ex){
                return "0";
            }
            return "1";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}