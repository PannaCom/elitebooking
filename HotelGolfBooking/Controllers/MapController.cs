using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
namespace HotelGolfBooking.Controllers
{
    public class MapController : Controller
    {
        //
        // GET: /Map/
        private hotelbookingEntities db = new hotelbookingEntities();
        public ActionResult Index(int idhotel)
        {
            var p = db.hotels.Find(idhotel);
            ViewBag.hotelname = p.name;
            ViewBag.lat = p.lat;
            ViewBag.lon = p.lon;
            ViewBag.address = p.address;
            ViewBag.image = Config.domain+p.image;
            ViewBag.des = p.name+", "+p.address;
            ViewBag.title = "Bản đồ khách sạn " + p.name;
            ViewBag.url = Config.domain + "/Map/Index?idhotel=" + p.id;
            return View();
        }

    }
}
