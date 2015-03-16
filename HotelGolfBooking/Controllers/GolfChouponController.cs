using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using Newtonsoft.Json;
using System.Linq;
namespace HotelGolfBooking.Controllers
{
    public class GolfChouponController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /GolfChoupon/

        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View(db.golf_choupon.ToList());
        }
        [HttpGet]
        public string test()
        {
            return "1";
        }
        [HttpPost]
        public string getChouponListGolf(int id)
        {
            //return "1";
            var p = (from q in db.golf_choupon where q.idgolf == id && q.deleted == 0 select q);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /GolfChoupon/Details/5
        public ActionResult Details(int id = 0)
        {
            golf_choupon golf_choupon = db.golf_choupon.Find(id);
            if (golf_choupon == null)
            {
                return HttpNotFound();
            }
            return View(golf_choupon);
        }

        //
        // GET: /GolfChoupon/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        struct golfprovin
        {
            public string provin;
            public string namegolf;
        }
        golfprovin[] arrgolfprovin = new golfprovin[100];
        int lengthGolfProvin=0;
        public ActionResult List(string provin, int? page, int? checkin,string name)
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
                var cp = (from q in db.golf_choupon where q.deleted == 0 select q).OrderBy(o => o.chouponprice);
                var rscp = cp.ToList();
                int totalitem = 0;
                //string linkPage = "/HotelChoupon/List?provin=" + provin + "&checkin="+ViewBag.fdate+"-page";
                int prePage = (int)page - 1;
                int nextPage = prePage + 2;
                int curPage = prePage + 1;
                lengthGolfProvin=rscp.Count;
                arrgolfprovin = new golfprovin[lengthGolfProvin];
                string content = "";
                
                for (int i = 0; i < rscp.Count; i++)
                {
                    int idgolf = (int)rscp[i].idgolf;
                    var cp2 = (from q2 in db.golves where q2.deleted == 0 && q2.id == idgolf select q2).OrderBy(o => o.id).Take(1);//q2.provin.Contains(provin) && 
                    try
                    {
                        if (cp2.ToList() != null && cp2.ToList().Count > 0)
                        {
                            var rscp2 = cp2.ToList()[0];
                            //if (!selectprovin.Contains(rscp2.provin))
                            //{
                            //    selectprovin += "<li><a href=\"/GolfChoupon/List?provin="+rscp2.provin+"\" style=\"font-weight:bold;color:#5e5e5e;\" onclick=\"showMenu("+i+");\">"+rscp2.provin+"<b class=\"caretnew\"></b></a>";//"<option value=\"" + rscp2.provin + "\">" + rscp2.provin + "</option> ";
                            //}
                            arrgolfprovin[i].namegolf = rscp2.name;
                            arrgolfprovin[i].provin = rscp2.provin;
                            if (provin != null && !rscp2.provin.Contains(provin)) continue;
                            if (name != null && !rscp2.name.Contains(name)) continue;
                            
                            //selectprovin +="<ul style=\"display:none;position:relative;margin-bottom:2px;margin-top:2px;\" id=\"menuchild1\">";
                            //<li><a href="/GolfChoupon/List" style="font-weight:bold;color:#5e5e5e;">Link</a></li>
                            //</ul>  
                            //Tim gia goc
                            string month = "," + DateTime.Now.Month + ",";
                            if (checkin != null) {
                                month = "," + Config.getMonthFromDateId((int)checkin) + ",";
                            }
                            totalitem++;
                            int tempPage = (totalitem - 1) / HotelGolfBooking.Config.PageSize + 1;
                            if (tempPage < curPage || tempPage > curPage) { continue; }

                            int oldprice = (int)(int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains(month)).Min(o => o.price);
                            content += "<!-- Room -->";
                            content += "<div class=\"col-sm-4 single\" style=\"height:401px;\">";
                            content += " <div > <img src=\"" + Config.domain + "/" + rscp2.image + "\" alt=\"" + rscp2.name + "\" class=\"img-responsive\"/>";
                            content += "      <div class=\"mask\">";
                            content += "        <div class=\"main\">";
                            content += "          <h4 style=\"color:#067C02;\"><span class=\"CityName\">" + rscp2.provin + "</span> - " + rscp2.name + "</h6>";
                            content += "          <h5><span style=\"text-decoration: line-through;\">Giá gốc: " + Config.formatNumber((int)oldprice) + "</span><span style=\"margin-left:7px;\">Giá coupon:</span><span style=\"color:#00B08F;font-size:15px;\"><b>" + Config.formatNumber((int)rscp[i].chouponprice) + "</b></span></h5>";
                            content += "        </div>";
                            content += "        <div class=\"content\" style=\"height:62px;\">";
                            content += "          <a href=\"/GolfBooking/Contact?idgolf=" + rscp[i].id + "&checkin=" + ViewBag.fdate + "&typebook="+Config.typebook2+"\" class=\"btn btn-info\" style=\"bottom:0;display:block;position:relative;\">Mua ngay</a>";
                            content += "          <p style=\"font-size:14;color:#5e5e5e;font-weight: bold;\">" + rscp[i].facility1;
                            if (rscp[i].facility2 != null && rscp[i].facility2 != "") content += ", " + rscp[i].facility2;
                            if (rscp[i].facility3 != null && rscp[i].facility3 != "") content += ", " + rscp[i].facility3;
                            if (rscp[i].facility4 != null && rscp[i].facility4 != "") content += ", " + rscp[i].facility4;
                            if (rscp[i].facility5 != null && rscp[i].facility5 != "") content += ", " + rscp[i].facility5;
                            if (rscp[i].facility6 != null && rscp[i].facility6 != "") content += ", " + rscp[i].facility6;
                            content += "</p>";
                            content += "      </div></div>";
                            content += " </div>";
                            content += "</div>";
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //Sap xep tinh thanh
                //for (int k1 = 0; k1 < lengthGolfProvin-1; k1++) {
                //    for (int k2 = k1+1; k2 < lengthGolfProvin; k2++)
                //    if (string.Compare(arrgolfprovin[k2].provin,arrgolfprovin[k1].provin)>0)
                //    {
                //        golfprovin temp = arrgolfprovin[k1];
                //        arrgolfprovin[k1] = arrgolfprovin[k2];
                //        arrgolfprovin[k2] = temp;

                //    }
                //}
                //arrgolfprovin.OrderBy(i => i.provin);
               
                
                Array.Sort(arrgolfprovin, delegate(golfprovin x, golfprovin y)
                {
                    return string.Compare(x.provin, y.provin);
                });
                string provinpr = "";
                string selectprovin = "<li class='active has-sub'><a href=\"#\">Chọn sân</a>";
                selectprovin += "<ul >";
                //selectprovin += "<li class='has-sub'><a href=\"/GolfChoupon/List?provin=" + arrgolfprovin[0].provin + "\" style=\"font-weight:bold;color:#5e5e5e;\" onclick=\"show(" + Config.unicodeToNoMarkCat(arrgolfprovin[0].provin) + ");\">" + arrgolfprovin[0].provin + "</a>";
                for (int k = 0; k < lengthGolfProvin; k++)
                {
                    if (provinpr != arrgolfprovin[k].provin)
                    {

                        if (!provinpr.Equals("")) selectprovin += "</ul></li>";
                        provinpr = arrgolfprovin[k].provin;
                        selectprovin += "<li class='has-sub'><a href=\"/GolfChoupon/List?provin=" + arrgolfprovin[k].provin + "&checkin=" + ViewBag.fdate + "\" >" + arrgolfprovin[k].provin + "</a>";
                        selectprovin += "<ul>";
                    }

                    selectprovin += "<li><a href=\"/GolfChoupon/List?name=" + arrgolfprovin[k].namegolf + "&checkin=" + ViewBag.fdate + "\">" + arrgolfprovin[k].namegolf + "</a></li>";
                    
                        
                }
                selectprovin += "</ul></li>";
                //selectprovin += "</ul>";
                //selectprovin += "</li>";
                selectprovin += "</ul>";
                selectprovin += "</li>"; 
                ViewBag.totalitem = totalitem;
                ViewBag.content = content;
                ViewBag.selectprovin = selectprovin;
                ViewBag.provin = provin;
                ViewBag.name = name;
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public ActionResult Booking(int id,int? checkin)
        {
            golf_choupon gp = db.golf_choupon.Find(id);
            int idgolf = gp.idgolf;
            int totalprice = (int)gp.chouponprice;
            golf golf = db.golves.Find(idgolf);
            ViewBag.idgolf = golf.id;
            ViewBag.golfname = golf.name;
            ViewBag.totalprice = totalprice;
            ViewBag.provin = golf.provin;
            ViewBag.des = golf.des;
            if (checkin != null)
            {
                ViewBag.checkin = checkin;
            }
            else
            {
                ViewBag.checkin = Config.datetimeid();
            }
            var p = (from q in db.golves where q.deleted == 0 select q).OrderByDescending(o => o.id).Take(10);
            var rs = p.ToList();
            string gallery = "";
            for (int i = 0; i < rs.Count; i++)
            {
                gallery += "<div class=\"item\"><a href=\"" + rs[i].website + "\" target=\"_blank\"><img src=\"" + rs[i].image + "\" width=\"800\" height=\"504\" alt=\"" + rs[i].name + "\"><i class=\"fa fa-search\"></i></a></div>";
            }
            ViewBag.gallery = gallery;
            return View();
        }
        //
        // POST: /GolfChoupon/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(golf_choupon golf_choupon)
        {
            if (ModelState.IsValid)
            {
                golf_choupon.deleted = 0;
                db.golf_choupon.Add(golf_choupon);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(golf_choupon);
        }

        //
        // GET: /GolfChoupon/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf_choupon golf_choupon = db.golf_choupon.Find(id);
            if (golf_choupon == null)
            {
                return HttpNotFound();
            }
            return View(golf_choupon);
        }

        //
        // POST: /GolfChoupon/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(golf_choupon golf_choupon)
        {
            if (ModelState.IsValid)
            {
                golf_choupon.deleted = 0;
                db.Entry(golf_choupon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(golf_choupon);
        }

        //
        // GET: /GolfChoupon/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf_choupon golf_choupon = db.golf_choupon.Find(id);
            if (golf_choupon == null)
            {
                return HttpNotFound();
            }
            return View(golf_choupon);
        }

        //
        // POST: /GolfChoupon/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            golf_choupon golf_choupon = db.golf_choupon.Find(id);
            golf_choupon.deleted = 1;
            db.Entry(golf_choupon).State = EntityState.Modified;
            db.SaveChanges();
            //db.golf_choupon.Remove(golf_choupon);
            //db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}