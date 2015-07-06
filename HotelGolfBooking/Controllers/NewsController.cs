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
using Newtonsoft.Json;
using System.IO;
using PagedList;
namespace HotelGolfBooking.Controllers
{
    public class NewsController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /News/

        public ActionResult Index(string keyword, int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (keyword == null || keyword == "all") keyword = "";
            ViewBag.keyword = keyword;
            var p = (from q in db.news where q.title.Contains(keyword) || q.des.Contains(keyword) || q.fullcontent.Contains(keyword) || q.keyword.Contains(keyword) select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult List(string keyword, int? page)
        {
            
            if (keyword == null || keyword=="all") keyword = "";
            ViewBag.name = keyword;
            var p = (from q in db.news where q.title.Contains(keyword) || q.des.Contains(keyword) || q.fullcontent.Contains(keyword) || q.keyword.Contains(keyword) select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Cat(string keyword, int catid, int? page)
        {
            if (keyword == null || keyword == "all") keyword = "";
            ViewBag.name = keyword;
            var p = (from q in db.news where q.catid == catid && (q.title.Contains(keyword) || q.des.Contains(keyword) || q.fullcontent.Contains(keyword) || q.keyword.Contains(keyword)) select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        //
        // GET: /News/Details/5
        public class listSumCat {
            public string catname { get; set; }
            public int catid { get; set; }
            public int total { get; set; }
        }
        public string getListSumCat() {
            string query = "select catname,catid,count(*) as total from news group by catname,catid";
            var rs = db.Database.SqlQuery<listSumCat>(query).Distinct();
            return JsonConvert.SerializeObject(rs.ToList());
        }
        public ActionResult Details(string url,int id = 0)
        {
            news news = db.news.Find(id);
            ViewBag.url = Config.domain + "detail/" + Config.unicodeToNoMark(news.title.Trim())+"-"+id;
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        public string getNewsGolfList()
        {
            var p = (from q in db.news where q.catid==2 select q).Distinct();
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCatNewsList() {
            var p = (from q in db.catnews where q.deleted == 0 select q).Distinct();
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
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
            //string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, fullPath, Config.NewsImagePath + "/" + nameFile);
            return Config.NewsImagePath + "/" + nameFile;
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcessContent(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename+"-"+Config.genCode());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            string content = "";
            for (int i = 0; i < countFile; i++)
            {
                nameFile = String.Format("{0}.jpg", filename + "-" + Guid.NewGuid().ToString());
                fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
                content += "<img src=\"" + Config.NewsImagePath + "/" + nameFile + "\" width=200 height=126>";
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                //break;
            }
            //string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, fullPath, Config.NewsImagePath + "/" + nameFile);
            //return Config.NewsImagePath + "/" + nameFile;
            return content;
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
        // GET: /News/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /News/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(news news)
        {
            if (ModelState.IsValid)
            {
                news.datetime = DateTime.Now;
                news.isHot = 0;
                db.news.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        //
        // GET: /News/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        public string getImageShowContentList(string url){
            string rs = "";
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            DirectoryInfo d = new DirectoryInfo(physicalPath);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(url+"-*.jpg"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                rs += "<img src=\"" + Config.NewsImagePath + "/"+file.Name + "\" width=200 height=126>";
            }
            return rs;
        }
        //
        // POST: /News/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(news news)
        {
            if (ModelState.IsValid)
            {
                news.datetime = DateTime.Now;
                news.isHot = 0;
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        //
        // GET: /News/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // POST: /News/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            news news = db.news.Find(id);
            db.news.Remove(news);
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