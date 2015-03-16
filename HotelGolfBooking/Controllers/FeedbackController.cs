using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using PagedList;
using Newtonsoft.Json;
using HotelGolfBooking.Views;
namespace HotelGolfBooking.Controllers
{
    public class FeedbackController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /Feedback/

        public ActionResult Index(string name, int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (name == null) name = "";
            ViewBag.name = name;
            var p = (from q in db.customer_feedback where q.cname.Contains(name) && q.cname != null select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public string updatestatus(int id, int type)
        {
            string query = "";
            if (type == 1)
            {
                query = "update customer_feedback set status=1 where id=" + id;
            }
            else {
                query = "delete from customer_feedback where id=" + id;
            }
            db.Database.ExecuteSqlCommand(query);
            return "1";
        }
        //
        // GET: /Feedback/Details/5

        public ActionResult Details(int id = 0)
        {
            customer_feedback customer_feedback = db.customer_feedback.Find(id);
            if (customer_feedback == null)
            {
                return HttpNotFound();
            }
            return View(customer_feedback);
        }
        public ActionResult Contact() {
            return View();
        }
        //
        // GET: /Feedback/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult Thanks() {
            return View();
        }
        //
        // POST: /Feedback/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(customer_feedback customer_feedback)
        {
            if (ModelState.IsValid)
            {
                customer_feedback.date = DateTime.Now;
                customer_feedback.status = 0;
                db.customer_feedback.Add(customer_feedback);
                db.SaveChanges();
                return RedirectToAction("Thanks");
            }

            return View(customer_feedback);
        }

        //
        // GET: /Feedback/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            customer_feedback customer_feedback = db.customer_feedback.Find(id);
            if (customer_feedback == null)
            {
                return HttpNotFound();
            }
            return View(customer_feedback);
        }

        //
        
        public string getFeedbackList()
        {
            string query = "select top 6 id,cname,caddress,fullcontent,hotelname,hotelimage,idhotel,golfname,golfimage,idgolf from ";
            query += "(select id,cname,caddress,fullcontent,hotelname,golfname from customer_feedback where status=1) as A ";
            query += "left join (select id as idhotel,image as hotelimage,name from hotel where deleted=0) as B on A.hotelname=B.name ";
            query += "left join (select id as idgolf,image as golfimage,name  from golf where deleted=0) as C on A.golfname=C.name ";
            query += " order by id desc";
            var rs = db.Database.SqlQuery<feedback>(query).ToList();
            return JsonConvert.SerializeObject(rs);
        }
        // POST: /Feedback/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(customer_feedback customer_feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer_feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer_feedback);
        }

        //
        // GET: /Feedback/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            customer_feedback customer_feedback = db.customer_feedback.Find(id);
            if (customer_feedback == null)
            {
                return HttpNotFound();
            }
            return View(customer_feedback);
        }

        //
        // POST: /Feedback/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer_feedback customer_feedback = db.customer_feedback.Find(id);
            db.customer_feedback.Remove(customer_feedback);
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