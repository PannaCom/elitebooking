using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using HotelGolfBooking.Models;
using PagedList;
using System.IO;
using Newtonsoft.Json;
namespace HotelGolfBooking.Controllers
{
    public class HotelsController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /Hotels/

        public ActionResult Index(string name,string provin,int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (name == null) name = "";
            if (provin == null) provin = "";
            ViewBag.page = (page ?? 1);
            ViewBag.name = name;
            ViewBag.provin = provin;
            var p = (from q in db.hotels where (q.name.Contains(name) && q.provin.Contains(provin)) && q.deleted == 0 select q).OrderByDescending(o=>o.id).Take(5000);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Gallery(int? page)
        {  
            var p = (from q in db.hotels where q.deleted == 0 select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public string getListHotel(string keyword) {
            var p = (from q in db.hotels where q.name.Contains(keyword) && q.deleted == 0 select q.name).Take(10);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getIdHotelByName(string keyword) {
            try
            {
                int p = (int)db.hotels.Where(o => o.deleted == 0).Where(o => o.name.Contains(keyword)).Max(o => o.id);
                return p.ToString();
            }
            catch (Exception ex) {
                return "-1";
            }
        }
        public string getListProvince()
        {
            var p = (from q in db.hotels where q.deleted == 0 orderby q.provin select q.provin).Distinct().Take(64);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string updateTotalViews(int idhotel) {
            string query = "update hotel set totalviews=totalviews+1 where id="+idhotel;
            db.Database.ExecuteSqlCommand(query);
            return "1";
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.HotelImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename+"-"+Config.genCode());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath)) {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                break;
            }
            string ok = resizeImage(Config.imgWidthHotel, Config.imgHeightHotel, fullPath, Config.HotelImagePath + "/" + nameFile);
            return Config.HotelImagePath+"/"+nameFile;
        }
        public string resizeImage(int maxWidth, int maxHeight, string fullPath,string path)
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
        // GET: /Hotels/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel hotel = db.hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        //
        // GET: /Hotels/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /Hotels/Create        
       
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel hotel)
        {
            //hotel.ruleextra = Server.HtmlEncode(hotel.ruleextra);
            //hotel.ruleroom = Server.HtmlEncode(hotel.ruleroom);
            if (ModelState.IsValid)
            {
                hotel.deleted = 0;
                hotel.date = DateTime.Now;
                hotel.dateid = Config.datetimeid();
                hotel.geocode = Config.CreatePoint(hotel.lat, hotel.lon);
                hotel.totalviews = 0;
                hotel.minprice = 0;
                db.hotels.Add(hotel);
                db.SaveChanges();
                int newid=hotel.id;
                string query = "insert into hotel_facility(idhotel,facility) select " + newid + " as idhotel,name as facility  from  list_facilities where deleted=0";
                db.Database.ExecuteSqlCommand(query);
                string query2 = "insert into hotel_service(idhotel,service) select " + newid + " as idhotel,name as service  from  list_services where deleted=0";
                db.Database.ExecuteSqlCommand(query2);
                string query3="if not exists(select * from list_address where dis like N'"+hotel.dis+"')";
	                    query3+="begin";
	                    query3+=" insert into list_address(dis,provin) values(N'"+hotel.dis+"',N'"+hotel.provin+"')";
                        query3+= "end";
                db.Database.ExecuteSqlCommand(query3);
                return RedirectToAction("Index");
                //return RedirectToAction("Edit", new {id=newid});
            }

            return View(hotel);
        }

        //
        // GET: /Hotels/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel hotel = db.hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            Session["idhotel"] = id;
            Session["hotelname"] = hotel.name;
            Session["dis"] = hotel.dis;
            ViewBag.idhotel = id;
            ViewBag.hotelname = hotel.name;
            ViewBag.dis = hotel.dis;
            Config.setCookie("idhotel", id.ToString());
            Config.setCookie("hotelname", hotel.name);
            Config.setCookie("dis", hotel.dis);
            return View(hotel);
        }

        //
        // POST: /Hotels/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel hotel)
        {
            if (ModelState.IsValid)
            {
                hotel.deleted = 0;
                hotel.date = DateTime.Now;
                hotel.dateid = Config.datetimeid();
                hotel.geocode = Config.CreatePoint(hotel.lat, hotel.lon);
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                string query3 = "if not exists(select * from list_address where dis like N'" + hotel.dis + "')";
                query3 += "begin";
                query3 += " insert into list_address(dis,provin) values(N'" + hotel.dis + "',N'" + hotel.provin + "')";
                query3 += "end";
                db.Database.ExecuteSqlCommand(query3);
                return RedirectToAction("Index");
            }
            return View(hotel);
        }

        //
        // GET: /Hotels/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel hotel = db.hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        //
        // POST: /Hotels/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel hotel = db.hotels.Find(id);
            hotel.deleted = 1;
            hotel.date = DateTime.Now;
            hotel.dateid = Config.datetimeid();
            db.Entry(hotel).State = EntityState.Modified;
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