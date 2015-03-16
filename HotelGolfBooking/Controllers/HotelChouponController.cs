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
    public class HotelChouponController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelChoupon/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.hotel_choupon.ToList());
        }
        [HttpGet]
        public string getChouponListGolf()
        {
            return "1";
        }
        [HttpPost]
        public string getHotelChouponList(int idhotel)
        {
            var p = (from q in db.hotel_choupon where q.idhotel == idhotel && q.deleted == 0 select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        [HttpPost]
        public string getHotelChouponDetail(int idhotel,int idroom)
        {
            //var p = db.hotel_choupon.Where(o => o.idhotel == idhotel).Where(o => o.total > 0).Where(o => o.idroom == idroom).Min(o => o.chouponprice);
            var p = db.hotel_choupon.Where(o => o.idhotel == idhotel).Where(o => o.total > 0).Where(o => o.idroom == idroom).OrderBy(o=>o.chouponprice).Take(1);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /HotelChoupon/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_choupon hotel_choupon = db.hotel_choupon.Find(id);
            if (hotel_choupon == null)
            {
                return HttpNotFound();
            }
            return View(hotel_choupon);
        }
        public ActionResult List(string provin,int? page,int? checkin)
        {
            try
            {
                if (page == null) page = 1;
                ViewBag.page = page; 
                if (checkin == null)
                {
                    ViewBag.fdate = Config.datetimeid();
                }
                else
                {
                    ViewBag.fdate = checkin;
                }
                if (provin == null) provin = "";
                var cp = (from q in db.hotel_choupon where q.deleted == 0 && q.total>0 select q).OrderBy(o => o.chouponprice);
                var rscp = cp.ToList();
                int totalitem = 0;

                //string linkPage = "/HotelChoupon/List?provin=" + provin + "&checkin="+ViewBag.fdate+"-page";
                int prePage = (int)page - 1;
                int nextPage = prePage + 2;
                int curPage = prePage + 1;

                string content = "";
                string selectprovin = "<option selected=\"selected\" value=\"\">Tất cả</option>";
                for (int i = 0; i < rscp.Count; i++)
                {
                    
                    int idhotel = (int)rscp[i].idhotel;
                    int idroom = (int)rscp[i].idroom;
                    var cp2 = (from q2 in db.hotels where q2.id == idhotel && q2.deleted == 0 select q2).OrderBy(o => o.id).Take(1);//q2.provin.Contains(provin) && 
                    try
                    {
                        if (cp2.ToList() != null && cp2.ToList().Count > 0)
                        {
                            var rscp2 = cp2.ToList()[0];
                            if (!selectprovin.Contains(rscp2.provin))
                            {
                                selectprovin += "<option value=\"" + rscp2.provin + "\">" + rscp2.provin + "</option> ";
                            }
                            if (!rscp2.provin.Contains(provin)) continue;
                            //Tim gia goc
                            string month = "," + DateTime.Now.Month + ",";
                            if (checkin != null)
                            {
                                month = "," + Config.getMonthFromDateId((int)checkin) + ",";
                            }
                            totalitem++;
                            int tempPage = (totalitem - 1) / HotelGolfBooking.Config.PageSize + 1;
                            if (tempPage < curPage || tempPage > curPage) { continue; }
                            //Tim gia goc
                            int oldprice = (int)(int)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.idroom == idroom).Where(o => o.month.Contains(month)).Min(o => o.price);
                            content += "<!-- Room -->";
                            content += "<div class=\"col-sm-4 single\" style=\"height:401px;\">";
                            content += " <div > <img src=\"" + Config.domain + "/" + rscp2.image + "\" alt=\"" + rscp[i].roomname + "\" class=\"img-responsive\"/>";
                            content += "   <div class=\"mask\">";
                            content += "        <div class=\"main\">";
                            content += "          <h4><span class=\"CityName\">" + rscp2.provin + "</span> - " + rscp2.name + "</h4>";
                            content += "          <h5><span style=\"text-decoration: line-through;\">Giá gốc: " + Config.formatNumber((int)oldprice) + "</span><span style=\"margin-left:7px;\">Giá coupon:</span><span style=\"color:#00B08F;font-size:15px;\"><b>" + Config.formatNumber((int)rscp[i].chouponprice) + "</b></span></h5>";
                            content += "        </div>";
                            content += "        <div class=\"content\" style=\"height:62px;\">";
                            content += "          <a href=\"/HotelChoupon/Booking?id=" + rscp[i].id + "&checkin=" + ViewBag.fdate + "\" class=\"btn btn-info\" style=\"top:2;display:block;position:relative;\">Mua ngay</a>";
                            content += "          <p style=\"font-size:14;color:#5e5e5e;font-weight: bold;\">" + rscp[i].facility1;
                            if (rscp[i].facility2 != null && rscp[i].facility2 != "") content += ", " + rscp[i].facility2;
                            if (rscp[i].facility3 != null && rscp[i].facility3 != "") content += ", " + rscp[i].facility3;
                            if (rscp[i].facility4 != null && rscp[i].facility4 != "") content += ", " + rscp[i].facility4;
                            if (rscp[i].facility5 != null && rscp[i].facility5 != "") content += ", " + rscp[i].facility5;
                            if (rscp[i].facility6 != null && rscp[i].facility6 != "") content += ", " + rscp[i].facility6;
                            content += "</p>";
                            content += "     </div></div>";
                            content += "    </div>";
                            content += "  </div>";
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                ViewBag.totalitem = totalitem;
                ViewBag.content = content;
                ViewBag.selectprovin = selectprovin;
                ViewBag.provin = provin;
            }
            catch (Exception ex)
            {
                
            }
            return View();
        }
        public ActionResult Booking(int id,int? checkin) {
            var p = (from q in db.hotel_choupon where q.id == id select q).Take(1);
            var rs=p.ToList()[0];
            ViewBag.idroom = rs.idroom;
            ViewBag.idhotel = rs.idhotel;
            ViewBag.hotelname = rs.hotelname;
            ViewBag.provin = rs.provin;
            ViewBag.roomname = rs.roomname;
            ViewBag.chouponprice = rs.chouponprice;
            if (checkin != null)
            {
                ViewBag.checkin = checkin;
            }
            else
            {
                ViewBag.checkin = Config.datetimeid();
            }
            int idhotel=(int)rs.idhotel;
            var p2 = (from q2 in db.hotels where q2.id == idhotel select q2).Take(1);
            var rs2 = p2.ToList()[0];
            ViewBag.provin = rs2.provin;
            return View();
        }
        //
        // GET: /HotelChoupon/Create

        public ActionResult Create()
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
            return View();
        }

        //
        // POST: /HotelChoupon/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_choupon hotel_choupon)
        {
            if (ModelState.IsValid)
            {
                hotel_choupon.deleted = 0;
                db.hotel_choupon.Add(hotel_choupon);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(hotel_choupon);
        }

        //
        // GET: /HotelChoupon/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_choupon hotel_choupon = db.hotel_choupon.Find(id);
            if (hotel_choupon == null)
            {
                return HttpNotFound();
            }
            return View(hotel_choupon);
        }

        //
        // POST: /HotelChoupon/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_choupon hotel_choupon)
        {
            if (ModelState.IsValid)
            {
                hotel_choupon.deleted = 0;
                db.Entry(hotel_choupon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(hotel_choupon);
        }

        //
        // GET: /HotelChoupon/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_choupon hotel_choupon = db.hotel_choupon.Find(id);
            if (hotel_choupon == null)
            {
                return HttpNotFound();
            }
            return View(hotel_choupon);
        }

        //
        // POST: /HotelChoupon/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_choupon hotel_choupon = db.hotel_choupon.Find(id);
            hotel_choupon.deleted = 1;
            db.Entry(hotel_choupon).State = EntityState.Modified;
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