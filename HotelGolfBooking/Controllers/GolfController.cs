﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using PagedList;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace HotelGolfBooking.Controllers
{
    public class GolfController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /Golf/
        public ActionResult Index(string name, int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (name == null) name = "";
            ViewBag.name = name;
            var p = (from q in db.golves where q.name.Contains(name) && q.deleted == 0 select q).OrderBy(o => o.name).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult List(string provin,int? checkin, int? page,string name)
        {
            if (checkin == null)
            {
                ViewBag.fdate = Config.datetimeid();
            }
            else {
                ViewBag.fdate = checkin;
            }
            if (provin == null) provin = "";
            ViewBag.provin = provin;
            if (name == null) name = "";
            name = name.Replace("_", " ");
            ViewBag.name = name;
            string selectprovin = "<li class='active has-sub'><a >Chọn sân</a>";
            string provinpr = "";
            selectprovin += "<ul >";
            var ps = (from q in db.golves where q.deleted == 0 select q).OrderBy(o => o.provin).Take(100).ToList();
            for (int i = 0; i < ps.Count; i++) {
                if (provinpr != ps[i].provin)
                {

                    if (!provinpr.Equals("")) { selectprovin += "</ul></li>"; }
                    provinpr = ps[i].provin;
                    selectprovin += "<li class='has-sub'><a href='/Golf/List?provin=" + ps[i].provin + "&checkin=" + ViewBag.fdate + "'>" + ps[i].provin + "</a>";
                    selectprovin += "<ul >";
                }
                selectprovin += "<li><a href='/Golf/List?name=" + ps[i].name.Trim().Replace(" ","_")+ "&checkin=" + ViewBag.fdate + "'>" + ps[i].name + "</a></li>";
            }
            selectprovin += "</ul></li>";
            selectprovin += "</ul>";
            selectprovin += "</li>";
            ViewBag.selectprovin = selectprovin;
            var p = (from q in db.golves where q.deleted == 0 && q.name.Contains(name) select q).OrderBy(o=>o.name).ThenBy(o => o.minprice).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public string getListGolfService() {
            var p = (from q in db.golves where q.deleted == 0 select q).OrderBy(o=>o.minprice).Take(20);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.GolfImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename + "-" + Config.genCode());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                break;
            }
            //string ok = resizeImage(Config.imgWidthGolf, Config.imgHeightGolf, fullPath, Config.GolfImagePath + "/" + nameFile);
            return Config.GolfImagePath + "/" + nameFile;
        }
        public string resizeImage(int maxWidth, int maxHeight, string fullPath, string path)
        {

            var image = System.Drawing.Image.FromFile(fullPath);
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(image.Width * ratioX);
            var newHeight = (int)(image.Height * ratioY);
            var newImage = new Bitmap(newWidth, newHeight);
            Graphics thumbGraph = Graphics.FromImage(newImage);

            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
            //thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            thumbGraph.DrawImage(image, 0, 0, newWidth, newHeight);
            image.Dispose();

            string fileRelativePath = path;// "newsizeimages/" + maxWidth + Path.GetFileName(path);
            newImage.Save(HttpContext.Server.MapPath(fileRelativePath), newImage.RawFormat);
            return fileRelativePath;
        }
        public string getGolfListGallery() {
            var p = (from q in db.golves where q.deleted == 0 select q).OrderByDescending(o => o.id).Take(10);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //        
        public string getGolfCityList()
        {
            var p = (from q in db.golves where q.deleted == 0 orderby q.provin select q.provin).Distinct();
            return JsonConvert.SerializeObject(p.ToList());
        }
        //        
        public string getListGolf(string keyword)
        {
            var p = (from q in db.golves where q.name.Contains(keyword) && q.deleted == 0 select q.name).Take(10);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getIdGolfByName(string keyword)
        {
            try
            {
                int p = (int)db.golves.Where(o => o.deleted == 0).Where(o => o.name.Contains(keyword)).Max(o => o.id);
                return p.ToString();
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }
        // GET: /Golf/Details/5
        public ActionResult Details(int id = 0)
        {
            golf golf = db.golves.Find(id);
            if (golf == null)
            {
                return HttpNotFound();
            }
            return View(golf);
        }

        //
        // GET: /Golf/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /Golf/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(golf golf)
        {
            if (ModelState.IsValid)
            {
                golf.deleted = 0;
                db.golves.Add(golf);
                db.SaveChanges();
                int newid = golf.id;
                return RedirectToAction("Index");
                //return RedirectToAction("Edit", new { id = newid });
            }

            return View(golf);
        }

        //
        // GET: /Golf/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf golf = db.golves.Find(id);
            if (golf == null)
            {
                return HttpNotFound();
            }
            //Session["idgolf"] = id;
            //Session["golfname"] = golf.name;
            ViewBag.idgolf = id;
            ViewBag.golfname = golf.name;
            Config.setCookie("idgolf", id.ToString());
            Config.setCookie("golfname", golf.name);
            return View(golf);
        }

        //
        // POST: /Golf/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(golf golf)
        {
            if (ModelState.IsValid)
            {
                golf.deleted = 0;
                db.Entry(golf).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(golf);
        }

        //
        // GET: /Golf/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf golf = db.golves.Find(id);
            if (golf == null)
            {
                return HttpNotFound();
            }
            return View(golf);
        }

        //
        // POST: /Golf/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            golf golf = db.golves.Find(id);
            golf.deleted = 1;
            db.Entry(golf).State = EntityState.Modified;
            db.SaveChanges();
            //db.golves.Remove(golf);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}