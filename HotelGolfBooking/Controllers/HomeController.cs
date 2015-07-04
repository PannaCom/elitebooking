using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using HotelGolfBooking.Views;
namespace HotelGolfBooking.Controllers
{
    public class HomeController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            DateTime d1 = DateTime.Now.AddDays(1);
            DateTime d2 = DateTime.Now.AddDays(2);
            ViewBag.fromdate = Config.convertToDateTimeId(d1.ToString());
            ViewBag.todate = Config.convertToDateTimeId(d2.ToString());
            if (!Request.Browser.IsMobileDevice)
            {
                try
                {
                    var hp = (from sl in db.slides where sl.deleted == 0 select sl).OrderBy(o => o.type).ThenByDescending(o => o.id).Take(20);
                    var rshp = hp.ToList();
                    //ViewBag.image1 = Config.domain + "/" + rshp[0].image;
                    //ViewBag.caption1 = rshp[0].caption;
                    //ViewBag.slogan1 = rshp[0].slogan;
                    //ViewBag.imagelink1 = Config.domain + "/" + rshp[0].imagelink;
                    //ViewBag.link1 = rshp[0].link;
                    //ViewBag.linktext1 = rshp[0].linktext;
                    //ViewBag.image2 = Config.domain + "/" + rshp[1].image;
                    //ViewBag.caption2 = rshp[1].caption;
                    //ViewBag.slogan2 = rshp[1].slogan;
                    //ViewBag.imagelink2 = Config.domain + "/" + rshp[1].imagelink;
                    //ViewBag.link2 = rshp[1].link;
                    //ViewBag.linktext2 = rshp[1].linktext;
                    string slide = "";
                    for (int h = 0; h < rshp.Count; h++)
                    {
                        slide += "<li data-transition=\"fade\" data-slotamount=\"7\" data-masterspeed=\"1500\">";
                        slide += "<img src=\"" + Config.domain + "/" + rshp[h].image + "\" style=\"opacity:0;\" alt=\"slidebg1\"  data-bgfit=\"cover\" data-bgposition=\"left bottom\" data-bgrepeat=\"no-repeat\">";
                        slide += " <div class=\"textlink\"><span style=\"text-shadow: 0 0 5px #0026ff, 0 0 7px #0026ff;margin: 10 10 10 10;color:#fff;display: block;font-size:18px;line-height: 1;-webkit-margin-before: 0.67em;-webkit-margin-after: 0.67em;-webkit-margin-start: 0px;-webkit-margin-end: 0px;font-weight: bold;\">" + rshp[h].caption + "</span><b><a href=\"" + rshp[h].link + "\" style=\"color:#fff;font-weight:bold;font-size:12px;\">" + rshp[h].linktext + "</a></b></div>";
                        slide += " <div class=\"caption sft revolution-starhotel smalltext\" data-x=\"35\" data-y=\"330\" data-speed=\"800\" data-start=\"1700\" data-easing=\"easeOutBack\">";
                        slide += " <span style=\"text-shadow: 0 0 25px #000, 0 0 27px #000;margin: 10px 0;color:#fff;display: block;font-size:28px;line-height: 1;-webkit-margin-before: 0.67em;-webkit-margin-after: 0.67em;-webkit-margin-start: 0px;-webkit-margin-end: 0px;font-weight: bold;\">" + rshp[h].slogan + "</span><br>";
                        slide += " </div></li>";
                    }
                    ViewBag.slide = slide;
                    ViewBag.ismobile = 0;
                }
                catch (Exception ex)
                {
                    ViewBag.ismobile = 1;
                }
            }
            else { 
                ViewBag.slide = "";
                ViewBag.ismobile = 1;
            }
            //khach san quan tam
            try
            {
                var p=(from q in db.hotels where q.deleted==0 select q).OrderByDescending(o=>o.totalviews).Take(10);
                var prs=p.ToList();
                string goodhotel="";
                for(int j=0;j<prs.Count;j++){
                    ///hotel/" + Config.unicodeToNoMark(prs[j].name) + "-" + ViewBag.fromdate + "-" + ViewBag.todate + "-" + prs[j].id + "
                    goodhotel += "<div class=\"item\"><a href=\"" + Config.domain + "/" + prs[j].image + "\" data-rel=\"prettyPhoto[gallery1]\"><img src=\"" + Config.domain + "/" + prs[j].image + "\" width=\"600\" height=\"400\" alt=\"" + prs[j].name + "-" + prs[j].dis + "-" + prs[j].provin + "\"><i class=\"fa fa-search\"></i><span>" + prs[j].name.Trim() + "-"+prs[j].provin.Trim() + "</span></a></div>";
                }
                ViewBag.goodhotel = goodhotel;
            }
            catch (Exception ex2) { 
            }
            //phan hoi cua khach hang
            try
            {
                string query="select top 6 id,cname,caddress,fullcontent,hotelname,hotelimage,idhotel,golfname,golfimage,idgolf from ";
                    query+="(select id,cname,caddress,fullcontent,hotelname,golfname from customer_feedback where status=1) as A ";
	                query+="left join (select id as idhotel,image as hotelimage,name from hotel where deleted=0) as B on A.hotelname=B.name ";
	                query+="left join (select id as idgolf,image as golfimage,name  from golf where deleted=0) as C on A.golfname=C.name ";
                    query += " order by id desc";
                    var rs = db.Database.SqlQuery<feedback>(query).ToList();
                string contentFeedback = "<div class=\"item\">"; ;
                for (int i = 0; i < rs.Count; i++)
                {
                    var cname = rs[i].cname;
                    var caddress = rs[i].caddress;
                    var fullcontent = rs[i].fullcontent;
                    var idhotel = rs[i].idhotel;
                    var idgolf = rs[i].idgolf;
                    var hotelimage = rs[i].hotelimage;
                    var hotelname = rs[i].hotelname;
                    var golfimage = rs[i].golfimage;
                    var golfname = rs[i].golfname;
                    var image = hotelimage != null ? hotelimage : golfimage;
                    var link = "";
                    var name = "";
                    var title = "";
                    if (hotelimage != null)
                    {
                        title = hotelname;
                        name = Config.unicodeToNoMark(hotelname.Trim());
                        link = "/hotel/" + name + "-" + ViewBag.fromdate + "-" + ViewBag.todate + "-" + idhotel+"-0";
                    }
                    else
                    {
                        title = golfname;
                        link = "/GolfBooking/Contact?idgolf=" + idgolf + "&checkin=" + ViewBag.fromdate + "&typebook="+Config.typebook1;
                    }
                    contentFeedback += "<div class=\"row\">";
                    contentFeedback += " <div class=\"col-lg-3 col-md-4 col-sm-2 col-xs-12\"><a href=\"" + link + "\"><img src=\"" + Config.domain + "/" + image + "\" width=102 height=102 alt=\"" + title + "\" class=\"img-circle\"/></a></div>";
                    contentFeedback += " <div class=\"col-lg-9 col-md-8 col-sm-10 col-xs-12\">";
                    contentFeedback += " <div class=\"text-balloon\">" + fullcontent + "<span>" + cname + ", " + caddress + "</span></div>";
                    contentFeedback += "</div>";
                    contentFeedback += "</div>";
                    if (i >= 1 && (i+1) % 2 == 0)
                    {
                        contentFeedback += "</div><div class=\"item\">";
                    }
                }
                contentFeedback += "</div>";
                ViewBag.contentFeedback = contentFeedback;
            }
            catch (Exception ex) { 

            }
            return View();
        }
        public ActionResult Admin()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
