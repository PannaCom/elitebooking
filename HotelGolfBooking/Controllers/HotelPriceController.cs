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
    public class HotelPriceController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelPrice/

        public ActionResult Index()
        {
            return View(db.hotel_room_price.ToList());
        }

        //
        // GET: /HotelPrice/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_room_price hotel_room_price = db.hotel_room_price.Find(id);
            if (hotel_room_price == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room_price);
        }

        //
        // GET: /HotelPrice/Create

        public ActionResult Create(int? idroom)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (Session["idhotel"] == null)
            {
                Session["idhotel"] = Config.getCookie("idhotel");
            }
            if (Session["hotelname"] == null)
            {
                Session["hotelname"] = Config.getCookie("hotelname");
            }
            if (idroom == null) idroom = -1;
            ViewBag.idroom = idroom;
            return View();
        }

        //
        // POST: /HotelPrice/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_room_price hotel_room_price)
        {
            if (ModelState.IsValid)
            {

                db.hotel_room_price.Add(hotel_room_price);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_room_price);
        }
        public string getHotelPriceList(int idhotel) {
            var p = (from q in db.hotel_room_price where q.deleted == 0 && q.idhotel==idhotel select q).OrderBy(o=>o.roomname).ThenBy(o=>o.price);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string addNewItem(int idhotel, string hotelname,int idroom,string roomname, int totalitem) {
            try
            {
                for (int i = 0; i < totalitem; i++)
                {
                    if (Request.Form["price" + i] != null)
                    {
                        int price = int.Parse(Request.Form["price" + i].ToString());
                        string month = Request.Form["month" + i].ToString();
                        string query = "insert into hotel_room_price(idhotel,hotelname,idroom,roomname,price,month,deleted) values(" + idhotel + ",N'" + hotelname + "'," + idroom + ",N'" + roomname + "'," + price + ",N'" + month + "',0)";
                        db.Database.ExecuteSqlCommand(query);
                    }
                }
                //Cap nhat minprice cho hotels
                //Lay ra minprice
                try
                {
                    int minprice = (int)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Min(o => o.price);
                    string query = "update hotel set minprice=" + minprice + " where id=" + idhotel;
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex) { 
                }
            }
            catch (Exception ex) {
                return "0";
            }
            return "1";
        }
        //
        // GET: /HotelPrice/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (Session["idhotel"] == null)
            {
                Session["idhotel"] = Config.getCookie("idhotel");
            }
            if (Session["hotelname"] == null)
            {
                Session["hotelname"] = Config.getCookie("hotelname");
            }
            hotel_room_price hotel_room_price = db.hotel_room_price.Find(id);
            if (hotel_room_price == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room_price);
        }

        //
        // POST: /HotelPrice/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_room_price hotel_room_price)
        {
            if (ModelState.IsValid)
            {
                hotel_room_price.deleted = 0;
                db.Entry(hotel_room_price).State = EntityState.Modified;
                db.SaveChanges();
                //Cap nhat minprice cho hotels
                //Lay ra minprice
                try
                {
                    int minprice = (int)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == hotel_room_price.idhotel).Min(o => o.price);
                    string query = "update hotel set minprice=" + minprice + " where id=" + hotel_room_price.idhotel;
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex)
                {
                }
                return RedirectToAction("Create");
            }
            return View(hotel_room_price);
        }

        //
        // GET: /HotelPrice/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotel_room_price hotel_room_price = db.hotel_room_price.Find(id);
            if (hotel_room_price == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room_price);
        }

        //
        // POST: /HotelPrice/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_room_price hotel_room_price = db.hotel_room_price.Find(id);
            db.hotel_room_price.Remove(hotel_room_price);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}