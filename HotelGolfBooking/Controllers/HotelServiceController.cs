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
    public class HotelServiceController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelService/

        public ActionResult Index(int? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            //int idhotel = int.Parse(Session["idhotel"].ToString());
            var p = (from q in db.hotel_service where q.idhotel == id select q);
            try
            {
                hotel ht = db.hotels.Find(id);
                ViewBag.idhotel = ht.id;
                ViewBag.hotelname = ht.name;
            }
            catch (Exception ex)
            {

            }
            return View(p.ToList());
        }
        [HttpPost]
        public string getHotelServices(int idhotel) {
            var p = (from q in db.hotel_service where q.idhotel==idhotel select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string insertServices(int idhotel, string service)
        {
            try
            {
                string query = "insert into hotel_service(idhotel,service) values(" + idhotel + ",N'" + service + "')";
                db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        [HttpPost]
        public string getServices()
        {
            var p = (from q in db.list_services where q.deleted == 0 select q).OrderBy(o => o.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string removeService(int id)
        {
            try
            {
                hotel_service hotel_service = db.hotel_service.Find(id);
                db.hotel_service.Remove(hotel_service);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        //
        // GET: /HotelService/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_service hotel_service = db.hotel_service.Find(id);
            if (hotel_service == null)
            {
                return HttpNotFound();
            }
            return View(hotel_service);
        }

        //
        // GET: /HotelService/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /HotelService/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_service hotel_service)
        {
            if (ModelState.IsValid)
            {
                db.hotel_service.Add(hotel_service);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_service);
        }

        //
        // GET: /HotelService/Edit/5

        public ActionResult Edit(int id = 0)
        {
            hotel_service hotel_service = db.hotel_service.Find(id);
            if (hotel_service == null)
            {
                return HttpNotFound();
            }
            return View(hotel_service);
        }

        //
        // POST: /HotelService/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_service hotel_service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel_service);
        }

        //
        // GET: /HotelService/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotel_service hotel_service = db.hotel_service.Find(id);
            if (hotel_service == null)
            {
                return HttpNotFound();
            }
            return View(hotel_service);
        }

        //
        // POST: /HotelService/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_service hotel_service = db.hotel_service.Find(id);
            db.hotel_service.Remove(hotel_service);
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