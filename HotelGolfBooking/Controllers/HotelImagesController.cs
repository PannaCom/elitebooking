using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace HotelGolfBooking.Controllers
{
    public class HotelImagesController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelImages/

        public ActionResult Index()
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

            var idhotel=int.Parse(Session["idhotel"].ToString());
            var p = (from q in db.hotel_image where q.idhotel == idhotel select q);
            return View(p.ToList());
        }
        [HttpPost]
        public string getHotelListImagesBooking(int idhotel) {
            var p = (from q in db.hotel_image where q.idhotel == idhotel select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string saveCaption(int id,string caption) {
            try
            {
                db.Database.ExecuteSqlCommand("update hotel_image set caption=N'" + caption + "' where id=" + id);
            }
            catch (Exception ex) {
                return "0";
            }
            return "1";
        }
        [HttpPost]
        public string removeImage(int id) {
            try
            {
                hotel_image hotel_image = db.hotel_image.Find(id);
                db.hotel_image.Remove(hotel_image);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(hotel_image hotelImage, IEnumerable<HttpPostedFileBase> file, string filename, int maxID)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.HotelImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename);
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i <countFile; i++)
            {
                int indexImage=maxID+i;
                nameFile = String.Format("{0}.jpg", filename + "-"+indexImage.ToString());
                fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);                
                string ok = resizeImage(Config.imgWidthHotelRoom, Config.imgHeightHotelRoom, fullPath, Config.HotelImagePath + "/" + nameFile);
                hotelImage.idhotel = int.Parse(Session["idhotel"].ToString());
                hotelImage.name = Config.HotelImagePath + "/" + nameFile;
                db.hotel_image.Add(hotelImage);
                db.SaveChanges();
            }
            
            return "1";
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
        // GET: /HotelImages/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_image hotel_image = db.hotel_image.Find(id);
            if (hotel_image == null)
            {
                return HttpNotFound();
            }
            return View(hotel_image);
        }

        //
        // GET: /HotelImages/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /HotelImages/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_image hotel_image)
        {
            if (ModelState.IsValid)
            {
                db.hotel_image.Add(hotel_image);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_image);
        }

        //
        // GET: /HotelImages/Edit/5

        public ActionResult Edit(int id = 0)
        {
            hotel_image hotel_image = db.hotel_image.Find(id);
            if (hotel_image == null)
            {
                return HttpNotFound();
            }
            return View(hotel_image);
        }

        //
        // POST: /HotelImages/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_image hotel_image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel_image);
        }

        //
        // GET: /HotelImages/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotel_image hotel_image = db.hotel_image.Find(id);
            if (hotel_image == null)
            {
                return HttpNotFound();
            }
            return View(hotel_image);
        }

        //
        // POST: /HotelImages/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_image hotel_image = db.hotel_image.Find(id);
            db.hotel_image.Remove(hotel_image);
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