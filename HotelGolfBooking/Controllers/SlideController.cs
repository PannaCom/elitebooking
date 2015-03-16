using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HotelGolfBooking.Controllers
{
    public class SlideController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /Slide/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.slides.Where(o=>o.deleted==0).ToList());
        }

        //
        // GET: /Slide/Details/5

        public ActionResult Details(int id = 0)
        {
            slide slide = db.slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        //
        // GET: /Slide/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            string filename = Guid.NewGuid().ToString();
            ViewBag.filename = filename;
            filename = Guid.NewGuid().ToString();
            ViewBag.filenamelink=filename;
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.SlideImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename);
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
            string ok = resizeImage(Config.imgWidthBigSlide, Config.imgHeightBigSlide, fullPath, Config.SlideImagePath + "/" + nameFile);
            return Config.SlideImagePath + "/" + nameFile;
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcessContent(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.ChouponImagePath + "\\");
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
            string ok = resizeImage(Config.imgWidthSmallSlide, Config.imgHeightSmallSlide, fullPath, Config.ChouponImagePath + "/" + nameFile);
            return Config.ChouponImagePath + "/" + nameFile;
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
        //
        // POST: /Slide/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(slide slide)
        {
            if (ModelState.IsValid)
            {
                slide.deleted = 0;
                db.slides.Add(slide);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(slide);
        }

        //
        // GET: /Slide/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            slide slide = db.slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            string filename = Guid.NewGuid().ToString();
            ViewBag.filename = filename;
            filename = Guid.NewGuid().ToString();
            ViewBag.filenamelink = filename;
            return View(slide);
        }

        //
        // POST: /Slide/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(slide slide)
        {
            if (ModelState.IsValid)
            {
                slide.deleted = 0;
                db.Entry(slide).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slide);
        }

        //
        // GET: /Slide/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            slide slide = db.slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        //
        // POST: /Slide/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            slide slide = db.slides.Find(id);
            slide.deleted = 1;
            db.Entry(slide).State = EntityState.Modified;
            db.SaveChanges();
            //db.slides.Remove(slide);
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