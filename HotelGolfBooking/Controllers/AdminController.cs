using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using System.Data.Entity;
using System.Data;
using System.Xml;
using System.Text;
namespace HotelGolfBooking.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private hotelbookingEntities db = new hotelbookingEntities();
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login","Admin");
            return View();
        }
        public ActionResult BackUp() {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult SiteMap() {

            return View();
        }
        public string SiteMapProcess() {
            try
            {
                var p = (from q in db.hotels where q.deleted == 0 select q).OrderByDescending(o => o.id).Take(10000).ToList();
                XmlWriterSettings settings = null;
                string xmlDoc = null;
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";
                float percent = 0.85f;
                int datetimeid = Config.datetimeid();
                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://elitehotel.com.vn/HotelChoupon/List");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://elitehotel.com.vn/Golf/List");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "0.99");
                    writer.WriteEndElement();
                   
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://elitehotel.com.vn/news/all-page1");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "0.94");
                    writer.WriteEndElement();

                    for (int i = 0; i < p.Count;i++)
                    {
                      
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = Config.domain + "hotel/" + Config.unicodeToNoMark(p[i].name) + "-" + Config.datetimeidaddday(1) + "-" + Config.datetimeidaddday(2) + "-" + p[i].id + "-0";
                            writer.WriteElementString("loc", urllink);
                            writer.WriteElementString("changefreq", "hourly");
                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                
            }
            catch (Exception ex) {
                return "0";
            }
            return "1";
        }
        public string backupdb() {
            try
            {
                var dbPath = @"D:\Elite\elitehotel.bak.rar";

                using (var data = new hotelbookingEntities())
                {
                    var cmd = String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, MEDIANAME='hotelbooking', MEDIADESCRIPTION='Media set for {0} database';"
                        , "hotelbooking", dbPath);
                    db.Database.ExecuteSqlCommand(cmd);
                }
            }
            catch (Exception ex) {
                return "0";
            }
            return "1";
        }
        public ActionResult CheckLogin()
        {
            if (Session["admin"] == "admin") 
                return RedirectToAction("Index");
            else
                return RedirectToAction("Login");
            //return View();
        }
        public ActionResult Logout() {
            if (Request.Cookies["logged"] != null)
            {
                Response.Cookies["logged"].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Abandon();
            return View();
        }
        public ActionResult Login()
        {
            
            return View();
        }

    }
}
