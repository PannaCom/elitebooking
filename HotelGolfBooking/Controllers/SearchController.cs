using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using HotelGolfBooking.Views;
using System.IO;
namespace HotelGolfBooking.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        private hotelbookingEntities db = new hotelbookingEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SearchHotel(int fromdate, int todate, string name, string rate, string dis,int? page)
        {
            try
            {
                if (page == null) page = 1;
                if (dis == null) dis = "";
                if (rate == null) rate = "0";
                if (name == "all") name = "";
                if (dis == "all") dis = "";
                dis = dis.Replace("_", " ");
                name = name.Replace("_", " ");
                rate = rate.Replace("rate", "");
                string query = "";
                query = " select top 400 id,name,provin,image,address,rate,minprice,invisibleprice from hotel where deleted=0 ";
                if (dis != "")
                {
                    query += " and (dis like N'" + dis + "' or provin like N'" + dis + "')  ";
                }
                if (rate != "0")
                {
                    query += " and rate=" + rate;
                }
                if (name != "")
                {
                    name = name.Replace(" ", "%");
                    query += " and name like N'%" + name + "%' ";
                }
                query += " order by rate desc,minprice";
                ViewBag.fdate = fromdate;
                ViewBag.tdate = todate;
                ViewBag.name = name;
                ViewBag.rate = rate;
                ViewBag.dis = dis;
                ViewBag.page = page;
                var rs = db.Database.SqlQuery<viewHotelSearchManager>(query).Distinct();
                var rl = rs.ToList();
                string near="";
                int iddiff = -1;
                if (rl.Count > 0) {
                    near = rl[0].provin;
                    iddiff = rl[0].id;
                }
                //ViewBag.Title = keyword;
                var viewModel = new ModelClassViewHotelSearchManager { ieViewNews = rl };
                try
                {
                    if (name != "" || (rl.Count<=1 && dis!=""))
                    {
                        var p = (from q in db.hotels where q.deleted == 0 && q.minprice > 0 && q.id != iddiff && (q.dis.Contains(near) || q.provin.Contains(near)) select q).OrderByDescending(o => o.rate).ThenBy(o => o.minprice).Take(10);
                        var rs2 = p.ToList();
                        string gallery = "";
                        //if (rs2.Count < 5)
                        //{
                        //    p = (from q in db.hotels where q.deleted == 0 && q.minprice > 0 && q.id != iddiff select q).OrderByDescending(o => o.totalviews).Take(100);
                        //    rs2 = p.ToList();
                        //    gallery = "";
                        //    for (int i = 0; i < rs2.Count; i++)
                        //    {
                        //        gallery += "<div class=\"item\"><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"800\" height=\"504\" alt=\"" + rs2[i].name + "\"><i class=\"fa fa-search\"></i><span>" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span></a></div>";
                        //    }
                        //    ViewBag.gallery = gallery;
                        //    ViewBag.gallerytitle = "Khách sạn nhiều người quan tâm";
                        //}
                        //else
                        //{
                        //    for (int i = 0; i < rs2.Count; i++)
                        //    {
                        //        gallery += "<div class=\"item\"><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"800\" height=\"504\" alt=\"" + rs2[i].name + "\"><i class=\"fa fa-search\"></i><span>" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span></a></div>";
                        //    }
                        //    ViewBag.gallery = gallery;
                        //    ViewBag.gallerytitle = "Khách sạn cùng khu vực";
                        //}
                        if (rs2.Count < 5)
                        {
                            p = (from q in db.hotels where q.deleted == 0 && q.minprice > 0 && q.id != iddiff select q).OrderByDescending(o => o.totalviews).Take(10);
                            rs2 = p.ToList();
                            gallery = "";
                            for (int i = 0; i < rs2.Count; i++)
                            {
                                gallery += "<div id=itemhotelsearch class=itemhotelsearch><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><div style=\"float:left;display:block;position:relative;width:25%;\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"95%\" height=\"75%\" alt=\"" + rs2[i].name + "\"></div><div style=\"float:left;display:block;position:relative;width:55%;\"><span><span style=\"color:#002060;font-weight:bold;\">" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span><br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: <span class=pricedetail>" + Config.formatNumber((int)rs2[i].minprice) + "</span></span></div></a></div>";
                            }
                            ViewBag.gallery = gallery;
                            ViewBag.gallerytitle = "Khách sạn tương tự";
                        }
                        else
                        {
                            for (int i = 0; i < rs2.Count; i++)
                            {
                                //gallery += "<div id=itemhotelsearch style=\"width:50%;float:left;display:block;position:relative;\"><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"35%\" height=\"45%\" alt=\"" + rs2[i].name + "\"><span>" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "<br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: " + Config.formatNumber((int)rs2[i].minprice) + "</span></a></div>";
                                gallery += "<div id=itemhotelsearch class=itemhotelsearch><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><div style=\"float:left;display:block;position:relative;width:25%;\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"95%\" height=\"75%\" alt=\"" + rs2[i].name + "\"></div><div style=\"float:left;display:block;position:relative;width:55%;\"><span><span style=\"color:#002060;font-weight:bold;\">" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span><br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: <span class=pricedetail>" + Config.formatNumber((int)rs2[i].minprice) + "</span></span></div></a></div>";
                            }
                            ViewBag.gallery = gallery;
                            ViewBag.gallerytitle = "Khách sạn cùng khu vực";
                        }
                    }
                   
                }
                catch (Exception ex) { 

                }
                return View(viewModel);
            }
            catch (Exception ex) {
                return View();
            }
        }
        public ActionResult GoodPrice(int fromdate, int todate, string name, string rate, string provin,int? page)
        {
            try
            {
                if (page == null) page = 1;
                if (provin == null) provin = "";
                if (rate == null) rate = "0";
                if (name == "all") name = "";
                if (provin == "all") provin = "";
                provin = provin.Replace("_", " ");
                name = name.Replace("_", " ");
                rate = rate.Replace("rate", "");
                string query = "";
                query = " select top 1000 id,name,provin,image,address,rate,minprice,invisibleprice from hotel where deleted=0 ";
                if (provin != "")
                {
                    query += " and provin like N'" + provin + "' ";
                }
                if (rate != "0")
                {
                    query += " and rate=" + rate;
                }
                if (name != "")
                {
                    query += " and name like N'%" + name + "%' ";
                }
                query += " order by rate desc,minprice";
                ViewBag.fdate = fromdate;
                ViewBag.tdate = todate;
                ViewBag.name = name;
                ViewBag.rate = rate;
                ViewBag.provin = provin;
                ViewBag.page = page;
                var rs = db.Database.SqlQuery<viewHotelSearchManager>(query).Distinct();
                var rl = rs.ToList();
                string near = "";
                int iddiff = -1;
                if (rl.Count > 0)
                {
                    near = rl[0].provin;
                    iddiff = rl[0].id;
                }

                //ViewBag.Title = keyword;
                var viewModel = new ModelClassViewHotelSearchManager { ieViewNews = rl };
                try
                {
                    if (name != "" || rl.Count <= 1)
                    {
                        var p = (from q in db.hotels where q.deleted == 0 && q.minprice > 0 && q.id != iddiff && (q.dis.Contains(near) || q.provin.Contains(near)) select q).OrderByDescending(o => o.rate).ThenBy(o => o.minprice).Take(10);
                        var rs2 = p.ToList();
                        string gallery = "";
                        if (rs2.Count < 5)
                        {
                            p = (from q in db.hotels where q.deleted == 0 && q.minprice > 0 && q.id != iddiff select q).OrderByDescending(o => o.totalviews).Take(10);
                            rs2 = p.ToList();
                            gallery = "";
                            for (int i = 0; i < rs2.Count; i++)
                            {
                                gallery += "<div id=itemhotelsearch class=itemhotelsearch><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><div style=\"float:left;display:block;position:relative;width:25%;\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"95%\" height=\"75%\" alt=\"" + rs2[i].name + "\"></div><div style=\"float:left;display:block;position:relative;width:55%;\"><span><span style=\"color:#002060;font-weight:bold;\">" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span><br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: <span class=pricedetail>" + Config.formatNumber((int)rs2[i].minprice) + "</span></span></div></a></div>";
                            }
                            ViewBag.gallery = gallery;
                            ViewBag.gallerytitle = "Khách sạn tương tự";
                        }
                        else
                        {
                            for (int i = 0; i < rs2.Count; i++)
                            {
                                //gallery += "<div id=itemhotelsearch style=\"width:50%;float:left;display:block;position:relative;\"><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"35%\" height=\"45%\" alt=\"" + rs2[i].name + "\"><span>" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "<br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: " + Config.formatNumber((int)rs2[i].minprice) + "</span></a></div>";
                                gallery += "<div id=itemhotelsearch class=itemhotelsearch><a href=\"/hotel/" + Config.unicodeToNoMark(rs2[i].name.Trim()) + "-" + ViewBag.fdate + "-" + ViewBag.tdate + "-" + rs2[i].id + "-0\" target=\"_blank\"><div style=\"float:left;display:block;position:relative;width:25%;\"><img src=\"" + Config.domain + "/" + rs2[i].image + "\" width=\"95%\" height=\"75%\" alt=\"" + rs2[i].name + "\"></div><div style=\"float:left;display:block;position:relative;width:55%;\"><span><span style=\"color:#002060;font-weight:bold;\">" + rs2[i].name.Trim() + "-" + rs2[i].provin.Trim() + "</span><br>" + Config.displayHotelRate((int)rs2[i].rate) + "<br>Giá từ: <span class=pricedetail>" + Config.formatNumber((int)rs2[i].minprice) + "</span></span></div></a></div>";
                            }
                            ViewBag.gallery = gallery;
                            ViewBag.gallerytitle = "Khách sạn cùng khu vực";
                        }
                    }

                }
                catch (Exception ex)
                {

                }

                return View(viewModel);
            }
            catch (Exception ex) {
                return View();
            }
        }
        public ActionResult Promotion(int? fromdate,int? todate,int? page)
        {
           try
           {
                if (page == null) page = 1;
                ViewBag.page = page;
                DateTime d1 = DateTime.Now.AddDays(1);
                DateTime d2 = DateTime.Now.AddDays(2);
                if (fromdate == null)
                {
                    ViewBag.fdate = Config.convertToDateTimeId(d1.ToString());
                }
                if (todate == null)
                {
                    ViewBag.tdate = Config.convertToDateTimeId(d2.ToString());
                }
          
                string query = "";
                query = "select top 1000 id,name,provin,image,address,rate,minprice,promotiondetail,invisibleprice from ";
                query += "(select id,name,provin,image,address,rate,minprice,invisibleprice from hotel where deleted=0 and haspromotion=1) as A inner join ";
                query += "(select idhotel,details as promotiondetail from hotel_promotion where ((fdate<=" + ViewBag.fdate + " and tdate>=" + ViewBag.fdate + ") or (fdate<=" + ViewBag.tdate + " and tdate>=" + ViewBag.tdate + "))) as B on A.id=B.idhotel ";
                query += " order by rate desc,minprice";

            
                var rs = db.Database.SqlQuery<viewHotelSearchManager>(query).Distinct();
                var rl = rs.ToList();
                //ViewBag.Title = keyword;
                var viewModel = new ModelClassViewHotelSearchManager { ieViewNews = rl };
                return View(viewModel);
           }
           catch (Exception ex)
           {
               return View();
           }
        }
        [HttpPost]
        public string getHotelPriceByEmail(int idhotel,string email,string phone) {
            try
            {
                string month = Config.getMonthFromDateId(Config.datetimeid());
                int fromdate=Config.datetimeid();
                int todate=Config.datetimeidaddday(1);
                var rs = (from q in db.hotel_room_price where q.idhotel == idhotel && q.deleted == 0 && q.month.Contains("," + month + ",") select q).OrderBy(o => o.price).ToList();
                string content = "";
                string hotelname=rs[0].hotelname;
                content += "<tr style=\"border-bottom:solid 1px #70C053;\"><th style=\"border-right:1px solid #70C053;\">Phòng</th><th style=\"border-right:1px solid #70C053;\">Giá</th></tr>";
                for (int i = 0; i < rs.Count; i++) {
                    content += "<tr style=\"border-bottom:solid 1px #70C053;\"><td style=\"border-right:1px solid #70C053;\">" + rs[i].roomname + "</td><td>" + Config.formatNumber(rs[i].price) + " VNĐ</td></tr>";
                }
                content += "<tr style=\"border-bottom:solid 1px #70C053;\"><td colspan=2 align=\"center\" style=\"border-right:1px solid #70C053;\"><a href=\"" + Config.domain + "/hotel/" + Config.unicodeToNoMark(rs[0].hotelname) + "-" + fromdate + "-" + todate + "-" + rs[0].idhotel + "-0\" style=\"background-color: #A22EA0;border-color:#A22EA0;text-shadow: 0 -1px 0 #1f659a;color:#fff;display: inline-block;text-align:center;padding: 6px 12px;font-size: 14px;border-radius: 4px;text-decoration: none;\">Đặt Phòng</a></td></tr>";
                try
                {
                    //customer_hotel_price htp = new customer_hotel_price();
                    //htp.email = email;
                    //htp.phone = phone;
                    //htp.requirement = "Lấy báo giá khách sạn " + hotelname + " bằng email.";
                    //htp.datetime = DateTime.Now;
                    //db.customer_hotel_price.Add(htp);
                    customer_list_email clm = new customer_list_email();
                    clm.cemail = email;
                    clm.date = DateTime.Now;
                    db.customer_list_email.Add(clm);
                    db.SaveChanges();
                }
                catch (Exception ctp) { 
                }
                string pathTemplate = HttpContext.Server.MapPath("/Scripts/tempEmailHotelPrice.txt");
                StreamReader SR = new StreamReader(pathTemplate);
                string body;
                body = SR.ReadToEnd();
                SR.Close();
                body = body.Replace("@domain", Config.domain);
                body = body.Replace("@ViewBag.hotelname", hotelname);
                body = body.Replace("@ViewBag.content", content);
                string toemail = email;
                rs = null;
                return Config.mail(Config.emailcompany, toemail, Config.subjectEmailHotelPrice, Config.passemailcompany, body);
            }
            catch (Exception ex) {
                return "-1";
            }
        }
    }
}
