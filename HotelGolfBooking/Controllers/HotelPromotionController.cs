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
    public class HotelPromotionController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelPromotion/

        public ActionResult Index()
        {
            return View(db.hotel_promotion.ToList());
        }
        public string getHotelPromotionList(int? idhotel)
        {

            var p = (from q in db.hotel_promotion where q.idhotel == idhotel select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getHotelPromotion(int idhotel, int? fromdate,int? todate)
        {
            var p = (from q in db.hotel_promotion where q.idhotel == idhotel && ((q.fdate <= fromdate && q.tdate >= fromdate) || (q.fdate <= todate && q.tdate >= todate)) select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getTopDeal(int fromdate)
        {
            string month = DateTime.Now.Month.ToString();
            var p = (from q in db.hotel_promotion
                     where q.fdate <= fromdate && q.tdate >= fromdate
                     select new
                     {
                         hotelname = q.hotelname,
                         idhotel = q.idhotel,
                         fdate = q.fdate,
                         tdate = q.tdate,
                         discount = q.discount,
                         rate=(int?)db.hotels.Where(o => o.deleted == 0).Where(o => o.id == q.idhotel).Min(o=>o.rate),
                         price = (int?)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == q.idhotel).Where(o=>o.month.Contains(","+month+",")).Min(o => o.price)
                     }
                ).OrderByDescending(o=>o.rate).ThenBy(o=>o.price).Take(8);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getTopDealByProvince(int fromdate,string provin)
        {
            string month = DateTime.Now.Month.ToString();
            var p = (from q in db.hotel_promotion
                     where q.fdate <= fromdate && q.tdate >= fromdate
                     select new
                     {
                         hotelname = q.hotelname,
                         idhotel = q.idhotel,
                         fdate = q.fdate,
                         tdate = q.tdate,
                         discount = q.discount,
                         rate = (int?)db.hotels.Where(o => o.deleted == 0).Where(o => o.id == q.idhotel).Min(o => o.rate),
                         price = (int?)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == q.idhotel).Where(o => o.month.Contains("," + month + ",")).Min(o => o.price),
                         provin=db.hotels.Where(o => o.deleted == 0).Where(o => o.id == q.idhotel).FirstOrDefault().provin,
                     }
                ).Where(o=>o.provin.Contains(provin)).OrderByDescending(o => o.rate).ThenBy(o => o.price).Take(20);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getTopDealViews() {
            var p = (from q in db.hotels where q.deleted == 0 select q).OrderByDescending(o => o.totalviews).Take(20);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /HotelPromotion/Details/5

        public ActionResult Details(int idhotel,int fdate,int tdate)
        {
            string rs="";
            var p=(from q in db.hotel_promotion where q.idhotel==idhotel && q.fdate<=fdate && q.tdate>=fdate select q).FirstOrDefault();
            rs="<h2>Chi tiết khuyến mãi "+ p.hotelname+"</h2>";
            rs+="Giảm giá "+p.discount+"% từ ngày "+Config.convertFromDateIdToDateString(p.fdate.ToString())+" đến ngày "+Config.convertFromDateIdToDateString(p.tdate.ToString());
            rs+=p.details;
            ViewBag.content=rs;
            return View();
        }

        //
        // GET: /HotelPromotion/Create

        public ActionResult Create(int? id)
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
            try
            {
                hotel ht = db.hotels.Find(id);
                ViewBag.idhotel = ht.id;
                ViewBag.hotelname = ht.name;
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        //
        // POST: /HotelPromotion/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_promotion hotel_promotion)
        {
            if (ModelState.IsValid)
            {
                //hotel_promotion.tdate = int.Parse(Config.convertToDateTimeId(hotel_promotion.todate.ToString()));
                //hotel_promotion.fdate = int.Parse(Config.convertToDateTimeId(hotel_promotion.fromdate.ToString()));
                db.hotel_promotion.Add(hotel_promotion);
                db.SaveChanges();
                int idhotel = hotel_promotion.idhotel;
                int discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Max(o => o.discount);
                if (discount != 0)
                {
                    string query = "update hotel set haspromotion=1 where id=" + idhotel;
                    db.Database.ExecuteSqlCommand(query);
                }
                return RedirectToAction("Create", new { id=idhotel});
            }

            return View(hotel_promotion);
        }

        //
        // GET: /HotelPromotion/Edit/5

        public ActionResult Edit(int id = 0)
        {
            hotel_promotion hotel_promotion = db.hotel_promotion.Find(id);
            if (hotel_promotion == null)
            {
                return HttpNotFound();
            }
            return View(hotel_promotion);
        }

        //
        // POST: /HotelPromotion/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_promotion hotel_promotion)
        {
            if (ModelState.IsValid)
            {
                //hotel_promotion.tdate = int.Parse(Config.convertToDateTimeId(hotel_promotion.todate.ToString()));
                //hotel_promotion.fdate = int.Parse(Config.convertToDateTimeId(hotel_promotion.fromdate.ToString()));
                db.Entry(hotel_promotion).State = EntityState.Modified;
                db.SaveChanges();
                int idhotel = hotel_promotion.idhotel;
                int discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Max(o => o.discount);
                if (discount != 0)
                {
                    string query = "update hotel set haspromotion=1 where id=" + idhotel;
                    db.Database.ExecuteSqlCommand(query);
                }
                return RedirectToAction("Create", new {id=idhotel});
            }
            return View(hotel_promotion);
        }

        //
        // GET: /HotelPromotion/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotel_promotion hotel_promotion = db.hotel_promotion.Find(id);
            if (hotel_promotion == null)
            {
                return HttpNotFound();
            }
            return View(hotel_promotion);
        }

        //
        // POST: /HotelPromotion/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            hotel_promotion hotel_promotion = db.hotel_promotion.Find(id);
            int? idhotel = hotel_promotion.idhotel;
            db.hotel_promotion.Remove(hotel_promotion);
            db.SaveChanges();
            return RedirectToAction("Create",new {id=idhotel});
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}