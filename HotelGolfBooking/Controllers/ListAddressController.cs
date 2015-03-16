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
    public class ListAddressController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /ListAddress/

        public ActionResult Index(string dis,int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (dis == null) dis = "";
            ViewBag.dis = dis;
            var p = (from q in db.list_address where q.dis.Contains(dis) && q.deleted == 0 select q).OrderBy(o => o.provin).ThenBy(o=>o.dis).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public class itemlistprovin{
            public string provin{get;set;}
        }
        public string getAddressList() { 
            //var p=(from q in db.hotels where q.deleted==0 && q.minprice>0 select q).OrderBy(o=>o.provin).ThenBy(o=>o.dis);
            string query = "select provin from hotel where deleted=0 and minprice>0 group by provin order by provin";
            var p = db.Database.SqlQuery<itemlistprovin>(query);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getDisList(string keyword)
        {
            var p = (from q in db.list_address where q.deleted == 0 && q.dis.Contains(keyword) orderby q.dis select q.dis);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /ListAddress/Details/5

        public ActionResult Details(int id = 0)
        {
            list_address list_address = db.list_address.Find(id);
            if (list_address == null)
            {
                return HttpNotFound();
            }
            return View(list_address);
        }

        //
        // GET: /ListAddress/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /ListAddress/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(list_address list_address)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    list_address.deleted = 0;
                    db.list_address.Add(list_address);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(list_address);
            }
            catch (Exception ex) {
                return View();
            }
        }

        //
        // GET: /ListAddress/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_address list_address = db.list_address.Find(id);
            if (list_address == null)
            {
                return HttpNotFound();
            }
            return View(list_address);
        }

        //
        // POST: /ListAddress/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(list_address list_address)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    list_address.deleted = 0;
                    db.Entry(list_address).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex) {
                return View();
            }
            return View(list_address);
        }

        //
        // GET: /ListAddress/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            list_address list_address = db.list_address.Find(id);
            if (list_address == null)
            {
                return HttpNotFound();
            }
            return View(list_address);
        }

        //
        // POST: /ListAddress/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            list_address list_address = db.list_address.Find(id);
            //db.list_address.Remove(list_address);
            list_address.deleted = 1;
            db.Entry(list_address).State = EntityState.Modified;
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