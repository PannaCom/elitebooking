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
    public class GolfPromotionController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /GolfPromotion/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.golf_promotion.ToList());
        }

        //
        // GET: /GolfPromotion/Details/5

        public ActionResult Details(int id = 0)
        {
            golf_promotion golf_promotion = db.golf_promotion.Find(id);
            if (golf_promotion == null)
            {
                return HttpNotFound();
            }
            ViewBag.golf = golf_promotion.golfname;
            ViewBag.url = "/GolfPromotion/Details?id=" + id;
            return View(golf_promotion);
        }
        public string getGolfPromotionList(int idgolf) {
            var p = (from q in db.golf_promotion where q.idgolf == idgolf select q).OrderBy(o=>o.fdate);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getGolfPromotion(int idgolf,int fdate)
        {
            var p = (from q in db.golf_promotion where q.idgolf == idgolf && q.fdate<=fdate && q.tdate>=fdate select q).OrderByDescending(o=>o.discount).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        
        //
        // GET: /GolfPromotion/Create

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

        //
        // POST: /GolfPromotion/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(golf_promotion golf_promotion)
        {
            if (ModelState.IsValid)
            {
                db.golf_promotion.Add(golf_promotion);
                db.SaveChanges();
                int idgolf = golf_promotion.idgolf;
                int discount = (int)db.golf_promotion.Where(o => o.idgolf == idgolf).Max(o => o.discount);
                if (discount != 0)
                {
                    string query = "update golf set haspromotion=1 where id=" + idgolf;
                    db.Database.ExecuteSqlCommand(query);
                }
                return RedirectToAction("Create");
            }

            return View(golf_promotion);
        }

        //
        // GET: /GolfPromotion/Edit/5

        public ActionResult Edit(int id = 0)
        {
            golf_promotion golf_promotion = db.golf_promotion.Find(id);
            if (golf_promotion == null)
            {
                return HttpNotFound();
            }
            return View(golf_promotion);
        }

        //
        // POST: /GolfPromotion/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(golf_promotion golf_promotion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(golf_promotion).State = EntityState.Modified;
                db.SaveChanges();
                int idgolf = golf_promotion.idgolf;
                int discount = (int)db.golf_promotion.Where(o => o.idgolf == idgolf).Max(o => o.id);
                if (discount != 0)
                {
                    string query = "update golf set haspromotion=1 where id=" + idgolf;
                    db.Database.ExecuteSqlCommand(query);
                }
                return RedirectToAction("Create");
            }
            return View(golf_promotion);
        }

        //
        // GET: /GolfPromotion/Delete/5

        public ActionResult Delete(int id = 0)
        {
            golf_promotion golf_promotion = db.golf_promotion.Find(id);
            if (golf_promotion == null)
            {
                return HttpNotFound();
            }
            return View(golf_promotion);
        }

        //
        // POST: /GolfPromotion/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            golf_promotion golf_promotion = db.golf_promotion.Find(id);
            db.golf_promotion.Remove(golf_promotion);
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