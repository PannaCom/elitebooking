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
    public class GolfPriceController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /GolfPrice/

        public ActionResult Index()
        {
            return View(db.golf_price.ToList());
        }

        //
        // GET: /GolfPrice/Details/5

        public ActionResult Details(int id = 0)
        {
            golf_price golf_price = db.golf_price.Find(id);
            if (golf_price == null)
            {
                return HttpNotFound();
            }
            return View(golf_price);
        }

        //
        // GET: /GolfPrice/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (Session["idgolf"] == null)
            {
                Session["idgolf"] = Config.getCookie("idgolf");
            }
            if (Session["golfname"] == null)
            {
                Session["golfname"] = Config.getCookie("golfname");
            }
            return View();
        }
        public string getGolfPriceList(int idgolf) {
            var p = (from q in db.golf_price where q.deleted == 0 && q.idgolf == idgolf select new { id = q.id, idgolf = q.idgolf, price = q.price, pricebuggy = q.pricebuggy, priceweekend = q.priceweekend, golfname = q.golfname, month = q.month }).OrderBy(o => o.price);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getGolfPrice(int idgolf,int dateid)
        {
            try
            {
                string month = Config.getMonthFromDateId(dateid);
                //int price = 0;
                //if (!Config.isWeekendDate(dateid))
                //{
                //    price = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains("," + month + ",")).Min(o => o.price);
                //}
                //else {
                //    price = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains("," + month + ",")).Min(o => o.priceweekend);
                //}
                //return price.ToString();
                var p = (from q in db.golf_price where q.idgolf == idgolf && q.month.Contains("," + month + ",") select new { id = q.id, idgolf = q.idgolf, price = q.price, pricebuggy = q.pricebuggy, priceweekend = q.priceweekend, golfname = q.golfname, month = q.month }).Take(1);
                return JsonConvert.SerializeObject(p.ToList());
            }
            catch (Exception ex) {
                return "0";
            }
        }
        public string addNewItem(int idgolf, string golfname, int totalitem)
        {
            try
            {
                for (int i = 0; i < totalitem; i++)
                {
                    if (Request.Form["price" + i] != null)
                    {
                        int price = int.Parse(Request.Form["price" + i].ToString());
                        int pricewk = int.Parse(Request.Form["pricewk" + i].ToString());
                        int pricebuggy = int.Parse(Request.Form["pricebuggy" + i].ToString());
                        string month = Request.Form["month" + i].ToString();
                        //string query = "insert into golf_price(idgolf,golfname,price,priceweekend,month,deleted) values(" + idgolf + ",N'" + golfname + "'," + price + ","+pricewk+",N'" + month + "',0)";
                        //db.Database.ExecuteSqlCommand(query);
                        golf_price gp = new golf_price();
                        gp.idgolf = idgolf;
                        gp.golfname = golfname;
                        gp.price = price;
                        gp.priceweekend = pricewk;
                        gp.pricebuggy = pricebuggy;
                        gp.month = month;
                        gp.deleted = 0;
                        db.golf_price.Add(gp);
                        db.SaveChanges();
                    }
                }
                //Cap nhat minprice cho golf
                //Lay ra minprice
                try
                {
                    int minprice = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Min(o => o.price);
                    string query = "update golf set minprice=" + minprice + " where id=" + idgolf;
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        //
        // POST: /GolfPrice/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(golf_price golf_price)
        {
            if (ModelState.IsValid)
            {
                db.golf_price.Add(golf_price);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(golf_price);
        }

        //
        // GET: /GolfPrice/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (Session["idgolf"] == null)
            {
                Session["idgolf"] = Config.getCookie("idgolf");
            }
            if (Session["golfname"] == null)
            {
                Session["golfname"] = Config.getCookie("golfname");
            }
            golf_price golf_price = db.golf_price.Find(id);
            if (golf_price == null)
            {
                return HttpNotFound();
            }
            return View(golf_price);
        }

        //
        // POST: /GolfPrice/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(golf_price golf_price)
        {
            if (ModelState.IsValid)
            {
                golf_price.deleted = 0;
                db.Entry(golf_price).State = EntityState.Modified;
                db.SaveChanges();
                //Cap nhat minprice cho golf
                //Lay ra minprice
                try
                {
                    int minprice = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == golf_price.idgolf).Min(o => o.price);
                    string query = "update golf set minprice=" + minprice + " where id=" + golf_price.idgolf;
                    db.Database.ExecuteSqlCommand(query);
                }
                catch (Exception ex)
                {
                }
                return RedirectToAction("Create");
            }
            return View(golf_price);
        }

        //
        // GET: /GolfPrice/Delete/5

        public ActionResult Delete(int id = 0)
        {
            golf_price golf_price = db.golf_price.Find(id);
            if (golf_price == null)
            {
                return HttpNotFound();
            }
            return View(golf_price);
        }

        //
        // POST: /GolfPrice/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            golf_price golf_price = db.golf_price.Find(id);
            db.golf_price.Remove(golf_price);
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