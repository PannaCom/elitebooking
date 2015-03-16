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
    public class HotelRoomController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelRoom/

        public ActionResult Index()
        {
            return View(db.hotel_room.ToList());
        }
        class roomdetail { 
            public int? id {get;set;}
            public int? price{get;set;}
            public int idhotel { get; set; }
            public string breakfast{get;set;}
            public string hotelname { get; set; }
            public string roomname { get; set; }
            public byte? extrabed{get;set;}
            public int? square{get;set;}
            public int? noofroom{get;set;}
            public int? extrabedfee { get; set; }
            public int? extraotherfee { get; set; }
            public byte? showprice { get; set; }
        }
        [HttpPost]
        public string getHotelRoomListBasic(int idhotel,int fromdate) {
            string month = Config.getMonthFromDateId(fromdate);
            //var p=(from q in db.hotel_room where q.idhotel==idhotel && q.deleted==0 select q);
            //return JsonConvert.SerializeObject(p.ToList());
            string query = "select id,idhotel,roomname,price,breakfast,extrabed,square,noofroom,extrabedfee,extraotherfee,showprice from ";
            query += "(select id,idhotel,roomname,breakfast,extrabed,square,noofroom,extrabedfee,extraotherfee,showprice from hotel_room where deleted=0 and idhotel=" + idhotel + ") as A left join ";
            query += "(select idroom,price from hotel_room_price where deleted=0 and month like '%," + month + ",%' and idhotel=" + idhotel + ") as B on A.id=B.idroom order by price";

            var p = db.Database.SqlQuery<roomdetail>(query).ToList();
            return JsonConvert.SerializeObject(p);
        }
        [HttpPost]
        public string getHotelRoom(int idhotel)
        {
            var p = (from q in db.hotel_room where q.idhotel == idhotel && q.deleted == 0 select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string getHotelRoomList(int idhotel, int fromdate)
        {
            string month = Config.getMonthFromDateId(fromdate);
            //var p=(from q in db.hotel_room where q.idhotel==idhotel && q.deleted==0 select q);
            //return JsonConvert.SerializeObject(p.ToList());
            string query = "select id,idhotel,hotelname,roomname,price,breakfast,extrabed,square,noofroom,showprice from ";
            query += "(select id,idhotel,hotelname,roomname,breakfast,extrabed,square,noofroom,showprice from hotel_room where deleted=0 and idhotel=" + idhotel + ") as A left join ";
            query += "(select idroom,price from hotel_room_price where deleted=0 and month like '%," + month + ",%' and idhotel=" + idhotel + ") as B on A.id=B.idroom order by price";
            
            var p = db.Database.SqlQuery<roomdetail>(query).ToList();
            return JsonConvert.SerializeObject(p);
        }
        [HttpPost]
        public string getRoomDetail(int idroom) {
            var p = (from q in db.hotel_room where q.deleted == 0 && q.id == idroom select q).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /HotelRoom/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_room hotel_room = db.hotel_room.Find(id);
            if (hotel_room == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room);
        }

        //
        // GET: /HotelRoom/Create

        public ActionResult Create()
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
            ViewBag.idroom = Guid.NewGuid().ToString();
            return View();
        }

        //
        // POST: /HotelRoom/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_room hotel_room)
        {
            if (ModelState.IsValid)
            {
                hotel_room.deleted = 0;
                db.hotel_room.Add(hotel_room);
                db.SaveChanges();
                //int idhotel=hotel_room.idhotel;
                //try
                //{
                //    int minprice = (int)db.hotel_room.Where(o => o.idhotel == idhotel).Where(o => o.deleted == 0).Min(o => o.price);
                //    string query = "update hotel set minprice=" + minprice + " where id=" + idhotel;
                //    db.Database.ExecuteSqlCommand(query);
                //}
                //catch (Exception ex) { 
                //}
                try
                {
                    string keyidroom=Request.Form["idroom"];
                    //Cập nhật lại tên phòng và id của phòng, bởi đã âm thầm tạm thời cập nhật giá phòng trước đó mà chưa có id và tên
                    string query = "update hotel_room_price set idroom=" + hotel_room.id + ",roomname=N'" + hotel_room.roomname + "' where roomname=N'" + keyidroom + "'";
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex2) { 

                }
                return RedirectToAction("Create");
            }

            return View(hotel_room);
        }

        //
        // GET: /HotelRoom/Edit/5

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
            hotel_room hotel_room = db.hotel_room.Find(id);
            if (hotel_room == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room);
        }

        //
        // POST: /HotelRoom/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_room hotel_room)
        {
            if (ModelState.IsValid)
            {
                hotel_room.deleted = 0;
                db.Entry(hotel_room).State = EntityState.Modified;
                db.SaveChanges();
                int idhotel = hotel_room.idhotel;
                try
                {
                    int minprice = (int)db.hotel_room.Where(o => o.idhotel == idhotel).Where(o => o.deleted == 0).Min(o => o.price);
                    string query = "update hotel set minprice=" + minprice + " where id=" + idhotel;
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex) { 
                }
                return RedirectToAction("Create");
            }
            return View(hotel_room);
        }

        //
        // GET: /HotelRoom/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_room hotel_room = db.hotel_room.Find(id);
            if (hotel_room == null)
            {
                return HttpNotFound();
            }
            return View(hotel_room);
        }

        //
        // POST: /HotelRoom/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_room hotel_room = db.hotel_room.Find(id);
            hotel_room.deleted = 1;
            db.Entry(hotel_room).State = EntityState.Modified;
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