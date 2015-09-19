using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelGolfBooking.Models;
using PagedList;
using System.IO;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
namespace HotelGolfBooking.Controllers
{
    public class GolfBookingController : Controller
    {
        //
        // GET: /GolfBooking/
        private hotelbookingEntities db = new hotelbookingEntities();
        public ActionResult Index(string name, string provin, string statusadmin, int? fromdate, int? todate, int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (name == null) name = "";
            if (provin == null) provin = "";
            if (statusadmin == null) statusadmin = "";
            if (fromdate == null) fromdate = -1;
            if (todate == null) todate = 99999999;
            ViewBag.name = name;
            ViewBag.provin = provin;
            if (fromdate != -1)
                ViewBag.fromdate = Config.convertFromDateIdToDateString2(fromdate.ToString());
            else ViewBag.fromdate = "";
            if (todate != 99999999)
                ViewBag.todate = Config.convertFromDateIdToDateString2(todate.ToString());
            else ViewBag.todate = "";
            ViewBag.statusadmin = statusadmin;
            var p = (from q in db.golf_booking where q.cname.Contains(name) && q.cname != null && q.provin.Contains(provin) && q.statusadmin.Contains(statusadmin) && q.dateplay >= fromdate && q.dateplay <= todate select q).OrderByDescending(o => o.id).Take(100);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public string getGolfTotalPrice(int idbooking, int idgolf, int dateplay, int typebook)
        {
            //Tim khuyen mai va gia ca
            var price = 0;
            try
            {
                price = (int)db.golf_booking.Where(o => o.id == idbooking).Min(o => o.totalprice);
            }
            catch (Exception) {
                price=0;
            }
            if (price != 0) return price.ToString();
            price = UtilDb.getGolfTotalPrice(idgolf, dateplay, typebook);
            //var discount = db.golf_promotion.Where(o => o.idgolf == idgolf).Where(o => o.fdate <= dateplay).Where(o => o.tdate >= dateplay).Max(o => o.discount);
            //if (discount == null || typebook != 0) discount = 0;            
            int totalprice = price;// (int)((price - (price * discount) / 100));
            return totalprice.ToString();
        }
        public int TotalPrice(int idbooking, int idgolf, int dateplay, int typebook)
        {
            //Tim khuyen mai va gia ca
            var price = 0;
            try
            {
                price = (int)db.golf_booking.Where(o => o.id == idbooking).Min(o => o.totalprice);
            }
            catch (Exception)
            {
                price = 0;
            }
            if (price != 0) return price;
            price = UtilDb.getGolfTotalPrice(idgolf, dateplay, typebook);
            //var discount = db.golf_promotion.Where(o => o.idgolf == idgolf).Where(o => o.fdate <= dateplay).Where(o => o.tdate >= dateplay).Max(o => o.discount);
            //if (discount == null || typebook != 0) discount = 0;
            int totalprice = price;// (int)((price - (price * discount) / 100));
            return totalprice;
        }
        public ActionResult Contact(int idgolf,int? checkin,int typebook) {
            golf golf = db.golves.Find(idgolf);
            ViewBag.idgolf = golf.id;
            ViewBag.golfname = golf.name;
            ViewBag.provin = golf.provin;
            ViewBag.des = golf.des;
            ViewBag.typebook = typebook;
            ViewBag.note = golf.note;
            if (checkin != null)
            {
                ViewBag.checkin = checkin;
            }
            else {
                ViewBag.checkin = Config.datetimeid();
            }
            var p = (from q in db.golves where q.deleted == 0 select q).OrderByDescending(o => o.id).Take(10);
            var rs=p.ToList();
            string gallery = "";
            for (int i = 0; i < rs.Count; i++) {
                gallery += "<div class=\"item\"><a href=\"" + Config.smoothLink(rs[i].website) + "\" target=\"_blank\"><img src=\"" + Config.domain + "/" + rs[i].image + "\" width=\"800\" height=\"504\" alt=\"" + rs[i].name + "\"><i class=\"fa fa-search\"></i><span>" + rs[i].name + "</span></a></div>";
            }
            ViewBag.gallery = gallery;
            if (Request.Browser.IsMobileDevice)
            {
                ViewBag.ismobile = 1;
            }
            else {
                ViewBag.ismobile = 0;
            }
            return View();
        }
        public string updatetotalprice(int id, int totalprice) {
            string query = "update golf_booking set totalprice=" + totalprice + " where id=" + id;
            db.Database.ExecuteSqlCommand(query);
            return "1";
        }
        public string updateAdminStatus(int id, string statusadmin)
        {
            try
            {
                string query = "update golf_booking set statusadmin=N'" + statusadmin + "' where id=" + id;
                db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public ActionResult Payment(int idgolf, string code)
        {
            ViewBag.code = code;
            ViewBag.idgolf = idgolf;
            //Thong tin san golf
            var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            ViewBag.datebook = rs.datebook;
            ViewBag.golfname = rs.golfname;
            ViewBag.cname = rs.cname;
            ViewBag.cemail = rs.cemail;
            ViewBag.cphone = rs.cphone;
            ViewBag.caddress = rs.caddress;
            ViewBag.cnote = rs.note;
            ViewBag.checkin = rs.checkin;
            ViewBag.teetime = rs.teetime;
            ViewBag.dateplay = Config.convertFromDateIdToDateString(rs.dateplay.ToString());
            ViewBag.totalprice = TotalPrice(rs.id, rs.idgolf,(int)rs.dateplay,(int)rs.typebook);
            ViewBag.info = rs.id;
            ViewBag.amount = ViewBag.totalprice;
            if (rs.buggies!=null && (int)rs.buggies == 1)
            {
                ViewBag.buggies = "có";
            }
            else
            {
                ViewBag.buggies = "không";
            }
            ViewBag.holes = rs.holes;
            ViewBag.checkin = rs.checkin;
            //Thong tin san golf
            var p2 = (from q2 in db.golves where q2.id == idgolf && q2.deleted == 0 select q2).Take(1);
            var rs2 = p2.ToList()[0];
            ViewBag.address = rs2.address;
            ViewBag.phone = rs2.contact;
            //Thong tin choupon
            var p3 = (from q3 in db.golf_choupon where q3.idgolf == idgolf && q3.deleted == 0 select q3).OrderByDescending(o => o.id).Take(1);
            if (p3 != null && p3.ToList().Count > 0)
            {
                var rs3 = p3.ToList()[0];
                ViewBag.extra = rs3.facility1 + "," + rs3.facility2 + "," + rs3.facility3 + "," + rs3.facility4 + "," + rs3.facility5 + "," + rs3.facility6;
            }
            return View();
        }
        public string PaymentProccess(int idgolf, string code, int type, long amount, string info)
        {
            info = "InvoiceNo" + info;
            Payment payment = new Payment();
            Hashtable hash = new Hashtable();
            long samount = 0;
            if (type == 1)
            {
                payment.SecureSecret = "198BE3F2E8C75A53F38C1C4A5B6DBA27";
                payment.VirtualPaymentClientUrl = "https://paymentcert.smartlink.com.vn:8181/vpcpay.do";
                hash.Add("vpc_AccessCode", "ECAFAB");
                hash.Add("vpc_Merchant", "SMLTEST");
                hash.Add("vpc_Currency", "VND");
                hash.Add("vpc_Version", "1.1");
                samount = amount * 100;
                hash.Add("vpc_Amount", samount.ToString());
            }
            else
            {
                payment.SecureSecret = "B575ED17E000D6E2BD8634FD0E6B042D";
                payment.VirtualPaymentClientUrl = "https://migs.mastercard.com.au/vpcpay";
                hash.Add("vpc_AccessCode", "72AD46B6");
                hash.Add("vpc_Merchant", "test03051980");
                hash.Add("vpc_Currency", "VND");
                hash.Add("vpc_Version", "1");
                //amount = amount / 21000;//Phải có bảng rate nữa
                samount = amount;
                hash.Add("vpc_Amount", samount.ToString());
            }

            hash.Add("vpc_Command", "pay");
            hash.Add("vpc_MerchTxnRef", "Test1234/1");
            hash.Add("vpc_OrderInfo", info);
            hash.Add("vpc_Locale", "VN");
            hash.Add("vpc_ReturnURL", Config.domain + "/GolfBooking/ConfirmPayment?idgolf=" + idgolf + "&code=" + code);
            hash.Add("vpc_BackURL", Config.domain + "/GolfBooking/Payment?idgolf=" + idgolf + "&code=" + code);
            hash.Add("vpc_TicketNo",idgolf.ToString());//Request.ServerVariables["REMOTE_ADDR"]
            return payment.getRedirectUrl(hash);
            //return "";
        }
        public ActionResult Print(int idgolf, string code)
        {
            var rule = db.golf_rule.First(o => o.rulegolf != null);
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            ViewBag.code = code;
            ViewBag.idgolf = idgolf;
            //Thong tin san golf
            var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];

            ViewBag.datebook = rs.datebook;
            ViewBag.golfname = rs.golfname;
            ViewBag.cname = rs.cname;
            ViewBag.cemail = rs.cemail;
            ViewBag.cphone = rs.cphone;
            ViewBag.caddress = rs.caddress;
            ViewBag.cnote = rs.note;
            ViewBag.checkin = rs.checkin;
            ViewBag.teetime = rs.teetime;
            ViewBag.invoice = UtilDb.getInvoiceDetailGolf(rs.id);
            if (rs.dateplay != null)
            {
                ViewBag.dateplay = rs.dateplay;
            }
            else {
                ViewBag.dateplay = "";
            }
            ViewBag.totalprice = TotalPrice(rs.id, rs.idgolf, (int)rs.dateplay, (int)rs.typebook);
            if (rs.buggies!=null && (int)rs.buggies == 1)
            {
                ViewBag.buggies = "có";
            }else{
                ViewBag.buggies = "không";
            }
            ViewBag.holes = rs.holes;
            ViewBag.checkin = rs.checkin;
            //Thong tin san golf
            var p2 = (from q2 in db.golves where q2.id == idgolf && q2.deleted==0 select q2).Take(1);
            var rs2 = p2.ToList()[0];
            ViewBag.address = rs2.address;
            ViewBag.phone = rs2.contact;
            //Thong tin choupon
            var p3 = (from q3 in db.golf_choupon where q3.idgolf == idgolf && q3.deleted == 0 select q3).OrderByDescending(o=>o.id).Take(1);
            if (p3 != null && p3.ToList().Count > 0)
            {
                var rs3 = p3.ToList()[0];
                ViewBag.extra = rs3.facility1 + "," + rs3.facility2 + "," + rs3.facility3 + "," + rs3.facility4 + "," + rs3.facility5 + "," + rs3.facility6;
            }
            ViewBag.extra += "<br>"+rule.rulegolf;
            return View();
        }
        public string emailConfirmBooking(int idgolf, string code)
        {
            var rule = db.golf_rule.First(o => o.rulegolf!=null);
            //Thong tin san golf
            var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            string invoice = UtilDb.getInvoiceDetailGolf(rs.id);
            string buggies = "có";
            int totalprice = TotalPrice(rs.id, rs.idgolf, (int)rs.dateplay, (int)rs.typebook);
            if (rs.buggies != null && (int)rs.buggies == 1)
            {
                buggies = "có";
            }
            else
            {
                buggies = "không";
            }           
            //Thong tin san golf
            var p2 = (from q2 in db.golves where q2.id == idgolf && q2.deleted == 0 select q2).Take(1);
            var rs2 = p2.ToList()[0];
            string extra="";
            //Thong tin choupon
            var p3 = (from q3 in db.golf_choupon where q3.idgolf == idgolf && q3.deleted == 0 select q3).OrderByDescending(o => o.id).Take(1);
            if (p3 != null && p3.ToList().Count > 0)
            {
                var rs3 = p3.ToList()[0];
                extra = rs3.facility1 + "," + rs3.facility2 + "," + rs3.facility3 + "," + rs3.facility4 + "," + rs3.facility5 + "," + rs3.facility6;
            }
            extra += "<br>"+rule.rulegolf;
            string dateplay = "";
            if (rs.dateplay != null)
            {
                dateplay = rs.dateplay.ToString();
            }
            else
            {
                dateplay = "";
            }
            string body = "";
            string pathTemplate = HttpContext.Server.MapPath("/Scripts/tempEmailConfirmGolf.txt");
            StreamReader SR = new StreamReader(pathTemplate);
            body = SR.ReadToEnd();
            SR.Close();
            body = body.Replace("@domain", Config.domain);
            body = body.Replace("@ViewBag.code", code);
            body = body.Replace("@ViewBag.datebook", rs.datebook.ToString());
            body = body.Replace("@ViewBag.cname", rs.cname);
            body = body.Replace("@ViewBag.golfname", rs.golfname);
            body = body.Replace("@ViewBag.address", rs2.address);
            body = body.Replace("@ViewBag.phone", rs2.contact);
            body = body.Replace("@ViewBag.dateplay", Config.convertFromDateIdToDateString(dateplay));
            body = body.Replace("@ViewBag.checkin", rs.checkin.ToString());
            body = body.Replace("@ViewBag.teetime", rs.teetime.ToString());            
            body = body.Replace("@ViewBag.holes", rs.holes.ToString());
            body = body.Replace("@ViewBag.buggies", buggies);
            body = body.Replace("@ViewBag.invoice", invoice);
            body = body.Replace("@ViewBag.totalprice", Config.formatNumber((int)totalprice));
            body = body.Replace("@ViewBag.extra", extra);
            
            //body += "Quý khách vừa đặt " + rs.numofroom + " phòng " + rs.roomname + " tại khách sạn " + rs.hotelname + ", từ ngày " + Config.convertFromDateIdToDateString(rs.fromdate.ToString()) + " đến ngày " + Config.convertFromDateIdToDateString(rs.todate.ToString()) + ", Với tổng số tiền là:" + Config.formatNumber(totalprice) + " VND. Chúng tôi vừa đặt phòng thành công cho quý vị";
            //body += "\r\nTừ " + Config.domain;
            Config.mail(Config.emailcompany, rs.cemail, Config.subjectEmailBookingGolf, Config.passemailcompany, body);
            return "0";
        }
        public string emailConfirmBookingPayment(int idgolf, string code)
        {
            var rule = db.golf_rule.First(o => o.rulegolf != null);
            //Thong tin san golf
            var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            string invoice = UtilDb.getInvoiceDetailGolf(rs.id);
            string buggies = "có";
            int totalprice = TotalPrice(rs.id, rs.idgolf, (int)rs.dateplay, (int)rs.typebook);
            if (rs.buggies!=null && (int)rs.buggies == 1)
            {
                buggies = "có";
            }
            else
            {
                buggies = "không";
            }
            //Thong tin san golf
            var p2 = (from q2 in db.golves where q2.id == idgolf && q2.deleted == 0 select q2).Take(1);
            var rs2 = p2.ToList()[0];
            string extra = "";
            //Thong tin choupon
            var p3 = (from q3 in db.golf_choupon where q3.idgolf == idgolf && q3.deleted == 0 select q3).OrderByDescending(o => o.id).Take(1);
            if (p3 != null && p3.ToList().Count > 0)
            {
                var rs3 = p3.ToList()[0];
                extra = rs3.facility1 + "," + rs3.facility2 + "," + rs3.facility3 + "," + rs3.facility4 + "," + rs3.facility5 + "," + rs3.facility6;
            }
            extra += "<br>" + rule.rulegolf;
            string body = "";
            string pathTemplate = HttpContext.Server.MapPath("/Scripts/tempEmailConfirmGolfPayment.txt");
            StreamReader SR = new StreamReader(pathTemplate);
            body = SR.ReadToEnd();
            SR.Close();
            body = body.Replace("@paymentlink", Config.domain + "/GolfBooking/Payment?idgolf=" + idgolf + "&code=" + code);
            body = body.Replace("@domain", Config.domain);
            body = body.Replace("@ViewBag.code", code);
            body = body.Replace("@ViewBag.datebook", Config.formatDateVn((DateTime)rs.datebook));
            body = body.Replace("@ViewBag.cname", rs.cname);
            body = body.Replace("@ViewBag.golfname", rs.golfname);
            body = body.Replace("@ViewBag.address", rs2.address);
            body = body.Replace("@ViewBag.phone", rs2.contact);
            body = body.Replace("@ViewBag.dateplay", Config.convertFromDateIdToDateString(rs.dateplay.ToString()));
            body = body.Replace("@ViewBag.checkin", rs.checkin.ToString());
            body = body.Replace("@ViewBag.teetime", rs.teetime.ToString());           
            body = body.Replace("@ViewBag.holes", rs.holes.ToString());
            body = body.Replace("@ViewBag.buggies", buggies);
            body = body.Replace("@ViewBag.invoice", invoice);
            body = body.Replace("@ViewBag.totalprice", Config.formatNumber((int)totalprice));
            body = body.Replace("@ViewBag.extra", extra);

            //body += "Quý khách vừa đặt " + rs.numofroom + " phòng " + rs.roomname + " tại khách sạn " + rs.hotelname + ", từ ngày " + Config.convertFromDateIdToDateString(rs.fromdate.ToString()) + " đến ngày " + Config.convertFromDateIdToDateString(rs.todate.ToString()) + ", Với tổng số tiền là:" + Config.formatNumber(totalprice) + " VND. Chúng tôi vừa đặt phòng thành công cho quý vị";
            //body += "\r\nTừ " + Config.domain;
            Config.mail(Config.emailcompany, rs.cemail, Config.subjectEmailBookingGolf, Config.passemailcompany, body);
            return "0";
        }
        public class invoiceitem
        {
            public int playdate { get; set; }
            public int price { get; set; }
            public int pricebuggies { get; set; }
            public int priceextralhole { get; set; }
            public int priceextralbuggies { get; set; }
            public int numofguest { get; set; }
            public int numofbuggies { get; set; }
            public int totalprice { get; set; }
            public string noteextrafee { get; set; }

        }
        public string addNewInvoice(int idbooking, int totalitem,int playdate)
        {
            int sumtotalprice = 0;
            try
            {
                db.Database.ExecuteSqlCommand("delete from golf_booking_invoice where idbooking=" + idbooking);
                for (int i = 1; i <= totalitem; i++)
                {
                    if (Request.Form["price" + i] != null)
                    {
                        int price = int.Parse(Request.Form["price" + i].ToString());
                        int pricebuggies = int.Parse(Request.Form["pricebuggies" + i].ToString());
                        int priceextralhole = int.Parse(Request.Form["priceextralhole" + i].ToString());
                        int priceextralbuggies = int.Parse(Request.Form["priceextralbuggies" + i].ToString());
                        int numofguest = int.Parse(Request.Form["numofguest" + i].ToString());
                        int numofbuggies = int.Parse(Request.Form["numofbuggies" + i].ToString());
                        int totalprice = int.Parse(Request.Form["totalprice" + i].ToString());
                        string noteextrafee = Request.Form["noteextrafee" + i].ToString();
                        sumtotalprice += totalprice;
                        //string query = "insert into golf_booking_invoice(idbooking,playdate,price,pricebuggies,numofroom,totalday,total,totalextrabed,totalextraother,totalprice) values(" + idbooking + ",N'" + fromdate + "',N'" + todate + "'," + price + "," + numofroom + "," + totalday + "," + total + "," + totalextrabed + "," + totalextraother + "," + totalprice + ")";
                        //db.Database.ExecuteSqlCommand(query);
                        golf_booking_invoice gbi = new golf_booking_invoice();
                        gbi.idbooking = idbooking;
                        gbi.playdate = playdate.ToString();
                        gbi.price = price;
                        gbi.pricebuggies = pricebuggies;
                        gbi.priceextralbuggies = priceextralbuggies;
                        gbi.priceextralhole = priceextralhole;
                        gbi.numofguest = numofguest;
                        gbi.numofbuggies = numofbuggies;
                        gbi.totalprice = totalprice;
                        gbi.noteextrafee = noteextrafee;
                        db.golf_booking_invoice.Add(gbi);
                        db.SaveChanges();
                    }
                }
                if (sumtotalprice != 0)
                {
                    db.Database.ExecuteSqlCommand("update golf_booking set totalprice=" + sumtotalprice + " where id=" + idbooking);
                }

            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public string viewInvoice(int idbooking,int idgolf,int numofbuggies,int numofguest,int dateplay,int typebook) {
            //Neu da co roi thi tra ve     
            int price1 = 0;
            try
            {
                price1 = (int)db.golf_booking_invoice.Where(o => o.idbooking == idbooking).Max(o => o.totalprice);
            }
            catch (Exception ex)
            {
                price1 = 0;
            }
            //Neu da co tu tinh bang tay thi tra ve           
            if (price1 != 0)
            {
                var p = (from q in db.golf_booking_invoice where q.idbooking == idbooking select q).OrderBy(o => o.price).ThenBy(o => o.totalprice);
                return JsonConvert.SerializeObject(p.ToList());
            }

            int price = UtilDb.getGolfTotalPrice(idgolf, dateplay, typebook);
            int pricebuggies = UtilDb.getPriceBuggies(idgolf, dateplay);
            invoiceitem[] items = new invoiceitem[5];
            items[0] = new invoiceitem();
            items[0].playdate = dateplay;
            items[0].price = price;
            items[0].pricebuggies = pricebuggies;
            items[0].priceextralhole = 0;
            items[0].priceextralbuggies = numofbuggies * pricebuggies;
            items[0].numofguest = numofguest;
            items[0].numofbuggies = numofbuggies;
            items[0].totalprice = numofguest * price + numofbuggies * pricebuggies;
            items[0].noteextrafee = "";
            return JsonConvert.SerializeObject(items.ToList());
        }
        public string deleteInvoice(int idbooking)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from golf_booking_invoice where idbooking=" + idbooking);
                db.Database.ExecuteSqlCommand("update golf_booking set totalprice=0 where id=" + idbooking);

            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        //public ActionResult addNew(golf_booking gb,) { 

        //}
        //
        // GET: /GolfBooking/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GolfBooking/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult ConfirmEmail() {
            return View();
        }
        public ActionResult ConfirmPayment(int idgolf, string code)
        {
            try
            {
                String responseCode = "";// Request.QueryString["vpc_ResponseCode"];              
                int type = 0;
                if (Request.QueryString["vpc_ResponseCode"] != null)
                {
                    responseCode = Request.QueryString["vpc_ResponseCode"].ToString();//Noi dia
                    type = 1;
                }
                else
                {
                    responseCode = Request.QueryString["vpc_TxnResponseCode"].ToString();//Quoc te
                    type = 2;
                }
                if (responseCode != null && responseCode.Length > 0)
                {
                    Hashtable hash = new Hashtable();
                    foreach (String key in Request.QueryString.AllKeys)
                    {
                        if (key.StartsWith("vpc_"))
                        {
                            hash.Add(key, Request.QueryString[key]);
                        }
                    }

                    Payment payment = new Payment();
                    payment.SecureSecret = "198BE3F2E8C75A53F38C1C4A5B6DBA27";
                    payment.checkSum(hash);
                    if (payment.isEmptysecureSecret())
                    {
                        ViewBag.info = "Thông tin bảo mật trả về không đúng";
                    }
                    else
                    {
                        if (payment.isValidsecureHash())
                        {
                            string amount = Request.QueryString["vpc_Amount"].ToString();
                            int price = (int)long.Parse(amount);
                            if (type == 1) price = price / 100;
                            string transactionNo = Request.QueryString["vpc_TransactionNo"].ToString();
                            string info = "<B>" + Request.QueryString["vpc_OrderInfo"].ToString() + "</B>";
                            var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
                            var rs = p.ToList()[0];
                            ViewBag.golfname = rs.golfname;
                            ViewBag.cname = rs.cname;
                            ViewBag.cemail = rs.cemail;
                            ViewBag.cphone = rs.cphone;
                            ViewBag.caddress = rs.caddress;
                            ViewBag.info = "Thanh toán hóa đơn đặt sân golf " + ViewBag.golfname + " thành công. Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!";
                            info = info + "<br>Sân golf: " + ViewBag.golfname;
                            info = info + "<br>Chơi ngày " + Config.convertFromDateIdToDateString(rs.dateplay.ToString());
                            info = info + "<br>Họ tên: " + ViewBag.cname;
                            info = info + "<br>Email: " + ViewBag.cemail;
                            info = info + "<br>Phone: " + ViewBag.cphone;
                            info = info + "<br>Địa chỉ: " + ViewBag.caddress;
                            info = info + "<br>Số tiền: " + Config.formatNumber(price);
                            golf_payment_info hpi = new golf_payment_info();
                            hpi.amount = price;
                            hpi.info = info;
                            hpi.transactionNo = transactionNo;
                            hpi.datetime = DateTime.Now;
                            db.golf_payment_info.Add(hpi);
                            db.SaveChanges();
                            string update = updateAdminStatus(rs.id, Config.admin_status_step2);
                            Config.mail(Config.emailcompany, ViewBag.cemail, "Chuyển khoản đặt sân Golf thành công", Config.passemailcompany, "Bạn đã chuyển khoản thanh toán hóa đơn thành công với thông tin sau:<br> " + info + "<br>Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!");
                        }
                        else
                        {
                            ViewBag.info = "Thanh toán chưa thành công. Kiểm tra lại đường truyền hoặc thông tin thẻ";
                        }
                    }
                }
                else
                {
                    ViewBag.info = "Thanh toán chưa thành công. Kiểm tra lại đường truyền hoặc thông tin thẻ";
                }
            }
            catch (Exception ex)
            {
                ViewBag.info = "Thanh toán chưa thành công. Kiểm tra lại đường truyền hoặc thông tin thẻ!";
            }

            //try
            //{
            //    string status = "";
            //    int type = 0;
            //    if (Request.QueryString["vpc_ResponseCode"] != null)
            //    {
            //        status = Request.QueryString["vpc_ResponseCode"].ToString();//Noi dia
            //        type = 1;
            //    }
            //    else
            //    {
            //        status = Request.QueryString["vpc_TxnResponseCode"].ToString();//Quoc te
            //        type = 2;
            //    }

            //    if (status.Equals("0"))
            //    {
            //        string amount = Request.QueryString["vpc_Amount"].ToString();
            //        int price = (int)long.Parse(amount);
            //        if (type == 1) price = price / 100;
            //        string transactionNo = Request.QueryString["vpc_TransactionNo"].ToString();
            //        string info = "<B>" + Request.QueryString["vpc_OrderInfo"].ToString() + "</B>";
            //        var p = (from q in db.golf_booking where q.idgolf == idgolf && q.idsession.Contains(code) select q).Take(1);
            //        var rs = p.ToList()[0];
            //        ViewBag.golfname = rs.golfname;
            //        ViewBag.cname = rs.cname;
            //        ViewBag.cemail = rs.cemail;
            //        ViewBag.cphone = rs.cphone;
            //        ViewBag.caddress = rs.caddress;
            //        ViewBag.info = "Thanh toán hóa đơn đặt sân golf " + ViewBag.golfname + " thành công. Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!";
            //        info = info + "<br>Sân golf: " + ViewBag.golfname;
            //        info = info + "<br>Chơi ngày " + Config.convertFromDateIdToDateString(rs.dateplay.ToString());
            //        info = info + "<br>Họ tên: " + ViewBag.cname;
            //        info = info + "<br>Email: " + ViewBag.cemail;
            //        info = info + "<br>Phone: " + ViewBag.cphone;
            //        info = info + "<br>Địa chỉ: " + ViewBag.caddress;
            //        info = info + "<br>Số tiền: " + Config.formatNumber(price);
            //        golf_payment_info hpi = new golf_payment_info();
            //        hpi.amount = price;
            //        hpi.info = info;
            //        hpi.transactionNo = transactionNo;
            //        hpi.datetime = DateTime.Now;
            //        db.golf_payment_info.Add(hpi);
            //        db.SaveChanges();
            //        string update = updateAdminStatus(rs.id, Config.admin_status_step2);
            //        Config.mail(Config.emailcompany, ViewBag.cemail, "Chuyển khoản đặt sân Golf thành công", Config.passemailcompany, "Bạn đã chuyển khoản thanh toán hóa đơn thành công với thông tin sau:<br> " + info + "<br>Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!");

            //    }
            //    else
            //    {
            //        ViewBag.info = "Thanh toán chưa thành công!";
            //    }

            //}
            //catch (Exception ex)
            //{
            //    ViewBag.info = "Thanh toán chưa thành công!";
            //}
            return View();
        }
        public string test() {
            return "1";
        }
        [HttpPost]
        public string addNew(int idgolf, string golfname, string provin, string cname, string cemail, string cphone, string caddress, int holes, int buggies, string checkin, string teetime, int dateplay, string note, string paymenttype, int numofguest, int numofbuggies, int typebook)
        {
            golf_booking golf_booking = new golf_booking();
            string idsession = Guid.NewGuid().ToString();
            golf_booking.idsession = idsession;
            golf_booking.idgolf = idgolf;
            golf_booking.golfname = golfname;
            golf_booking.provin = provin;
            golf_booking.cname = cname;
            golf_booking.cemail = cemail;
            golf_booking.cphone = cphone;
            golf_booking.caddress = caddress;
            golf_booking.holes = holes;
            golf_booking.buggies = buggies;
            golf_booking.checkin=checkin;
            golf_booking.teetime = teetime;
            golf_booking.dateplay = dateplay;
            golf_booking.note = note;
            golf_booking.paymenttype = paymenttype;
            golf_booking.status = Config.booking_status_step3 + paymenttype;
            golf_booking.statusadmin = Config.admin_status_step1;
            golf_booking.countmail = 0;
            golf_booking.datebook = DateTime.Now;
            golf_booking.typebook = typebook;// Config.typebook1;
            golf_booking.numofbuggies = numofbuggies;
            golf_booking.numofguest = numofguest;
            db.golf_booking.Add(golf_booking);
            db.SaveChanges();
            return "1";
        }
        [HttpPost]
        public string addNewChoupon(int idgolf, string golfname,string provin, string cname, string cemail, string cphone, string caddress, string checkin, string teetime, int? dateplay, string note, string paymenttype,int totalprice)
        {
            golf_booking golf_booking = new golf_booking();
            string idsession = Guid.NewGuid().ToString();
            golf_booking.idsession = idsession;
            golf_booking.idgolf = idgolf;
            golf_booking.golfname = golfname;
            golf_booking.provin = provin;
            golf_booking.cname = cname;
            golf_booking.cemail = cemail;
            golf_booking.cphone = cphone;
            golf_booking.caddress = caddress;
            golf_booking.checkin = checkin;
            golf_booking.teetime = teetime;
            golf_booking.dateplay = dateplay;
            golf_booking.note = note;
            golf_booking.paymenttype = paymenttype;
            golf_booking.status = Config.booking_status_step3 + paymenttype;
            golf_booking.statusadmin = Config.admin_status_step1;
            golf_booking.countmail = 0;
            golf_booking.datebook = DateTime.Now;
            golf_booking.typebook = Config.typebook2;
            //golf_booking.totalprice = totalprice;
            db.golf_booking.Add(golf_booking);
            db.SaveChanges();
            return "1";
        }
        public string update(int id, string cname, string cemail, string cphone, string caddress, int holes, int buggies, string checkin, string teetime, int dateplay, string note, string paymenttype, int numofbuggies, int numofguest)
        {
            string query = "update golf_booking set ";
            query+="cname=N'"+cname+"',";
            query+="cemail=N'" + cemail + "',";
            query += "cphone=N'" + cphone + "',";
            query += "caddress=N'" + caddress + "',";          
            query += "holes=" + holes + ",";
            query += "buggies=" + buggies + ",";
            query += "numofbuggies=" + numofbuggies + ",";
            query += "numofguest=" + numofguest + ",";
            query += "checkin=N'" + checkin + "',";
            query += "teetime=N'" + teetime + "',";            
            query += "dateplay=" + dateplay + ",";
            query += "note=N'" + note + "',";
            query += "paymenttype=N'" + paymenttype + "',";
            query += "status=N'" + Config.booking_status_step3 + paymenttype + "', ";
            query += "statusadmin=N'" + Config.admin_status_step1+ "' ";
            query += " where id="+id;
            db.Database.ExecuteSqlCommand(query);
            return "1";
        }
        //
        // POST: /GolfBooking/Create

        [HttpPost]
        public ActionResult Create(golf_booking golf_booking)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    //golf_booking.status = 0;
                    //golf_booking.statusadmin = Config.booking_status_step2;
                    //golf_booking.countmail = 0;
                    //golf_booking.datebook = DateTime.Now;
                    db.golf_booking.Add(golf_booking);
                    db.SaveChanges();
                    return RedirectToAction("ConfirmEmail");
                }
            }
            catch
            {
                return View();
            }
            return View();
        }

        //
        // GET: /GolfBooking/Edit/5

        public ActionResult Edit(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            golf_booking gb = db.golf_booking.Find(id);
            if (gb == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            ViewBag.golfname = gb.golfname;
            return View(gb);
          
        }

        //
        // POST: /GolfBooking/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GolfBooking/Delete/5

        public ActionResult Delete(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }

        //
        // POST: /GolfBooking/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
