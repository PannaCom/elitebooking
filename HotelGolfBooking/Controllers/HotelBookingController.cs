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
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
namespace HotelGolfBooking.Controllers
{
    public class HotelBookingController : Controller
    {
        private hotelbookingEntities db = new hotelbookingEntities();

        //
        // GET: /HotelBooking/

        public ActionResult Index(string name,string provin,string statusadmin,int? fromdate,int? todate,int? page)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            if (name == null) name = "";
            if (provin == null) provin = "";
            if (statusadmin == null) statusadmin = "";
            if (fromdate == null) fromdate = -1;           
            if (todate == null) todate = 99999999;           
            ViewBag.name = name;
            ViewBag.provin = provin;
            if (fromdate!=-1) 
                ViewBag.fromdate = Config.convertFromDateIdToDateString2(fromdate.ToString());
            else ViewBag.fromdate="";
            if (todate != 99999999) 
                ViewBag.todate = Config.convertFromDateIdToDateString2(todate.ToString()); 
            else ViewBag.todate="";
            ViewBag.statusadmin = statusadmin;
            var p = (from q in db.hotel_booking where q.cname.Contains(name) && q.cname!=null && q.provin.Contains(provin) && q.statusadmin.Contains(statusadmin) && q.fromdate>=fromdate && q.fromdate<=todate select q).OrderByDescending(o => o.id).Take(10000);
            int pageSize = Config.PageSize;
            int pageNumber = (page ?? 1);
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /HotelBooking/Details/5

        public ActionResult Details(int id = 0)
        {
            hotel_booking hotel_booking = db.hotel_booking.Find(id);
            if (hotel_booking == null)
            {
                return HttpNotFound();
            }
            return View(hotel_booking);
        }
        public ActionResult Confirm(int idhotel,string code) {
            ViewBag.code = code;
            ViewBag.idhotel = idhotel;
            hotel ht = db.hotels.Find(idhotel);
            ViewBag.rule = ht.ruleroom+"<br>"+ht.ruleextra;
            ViewBag.hotelname = ht.name;
            
            //var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
            //var rs = p.ToList()[0];
            //ViewBag.hotelname = rs.hotelname;
            //ViewBag.fdate = rs.fromdate;
            //ViewBag.tdate = rs.todate;
            //ViewBag.numofroom = rs.numofroom;
            //ViewBag.roomname = rs.roomname;
            return View();
        }
        [HttpPost]
        public string addNewInvoice(int idbooking,int totalitem)
        {
            int sumtotalprice = 0;
            try
            {
                db.Database.ExecuteSqlCommand("delete from hotel_booking_invoice where idbooking="+idbooking);
                for (int i = 1; i <= totalitem; i++)
                {
                    if (Request.Form["price" + i] != null)
                    {
                        int fromdate = int.Parse(Request.Form["fromdate" + i].ToString());
                        int todate = int.Parse(Request.Form["todate" + i].ToString());
                        int totalday = int.Parse(Request.Form["totalday" + i].ToString());
                        int numofroom = int.Parse(Request.Form["numofroom" + i].ToString());
                        int price = int.Parse(Request.Form["price" + i].ToString());
                        int total = int.Parse(Request.Form["total" + i].ToString());
                        int totalextrabed = int.Parse(Request.Form["totalextrabed" + i].ToString());
                        int totalextraother = int.Parse(Request.Form["totalextraother" + i].ToString());
                        int totalprice = int.Parse(Request.Form["totalprice" + i].ToString());
                        sumtotalprice += totalprice;
                        string query = "insert into hotel_booking_invoice(idbooking,fromdate,todate,price,numofroom,totalday,total,totalextrabed,totalextraother,totalprice) values(" + idbooking + ",N'" + fromdate + "',N'" + todate + "'," + price + "," + numofroom + ","+totalday+","+total+","+totalextrabed+","+totalextraother+","+totalprice+")";
                        db.Database.ExecuteSqlCommand(query);
                    }
                }
                if (sumtotalprice != 0) {
                    db.Database.ExecuteSqlCommand("update hotel_booking set totalprice="+sumtotalprice+" where id="+idbooking);
                }

            }
            catch (Exception ex) {
                return "0";
            }
            return "1";
        }
        public string deleteInvoice(int idbooking) {
            try
            {
                db.Database.ExecuteSqlCommand("delete from hotel_booking_invoice where idbooking=" + idbooking);
                db.Database.ExecuteSqlCommand("update hotel_booking set totalprice=0 where id=" + idbooking);

            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public ActionResult ConfirmEmail() {
            return View();
        }
        //
        // GET: /HotelBooking/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            return View();
        }
        public string test() {
            return "1";
        }
        [HttpPost]
        public string addNewChoupon(hotel_booking hb, int idhotel, string hotelname,string provin, int idroom, string roomname, int numofroom, int numofguest, int? fromdate, int? todate, string cname, string cemail, string cphone, string caddress, string cnote, string paymenttype)//hotel_booking hb, 
        {
            string idsession = Guid.NewGuid().ToString();
            try
            {
                hb.idsession = idsession;
                hb.idhotel = idhotel;
                hb.hotelname = hotelname;
                hb.provin = provin;
                hb.idroom = idroom;
                hb.roomname = roomname;
                hb.numofroom = numofroom;
                hb.numofguest = numofguest;
                if (fromdate != null && todate != null)
                {
                    hb.fromdate = fromdate;
                    hb.todate = todate;
                }

                hb.cname = cname;
                hb.cemail = cemail;
                hb.cphone = cphone;
                hb.caddress = caddress;
                hb.paymenttype = paymenttype;
                hb.cnote = cnote;

                hb.datebook = DateTime.Now;
                hb.status = Config.choupon_status_step1 + paymenttype;
                hb.typebook = Config.typebook2;
                hb.statusadmin = Config.admin_status_step1;
                hb.extrabedfee = 0;
                hb.extraotherfee = 0;
                db.hotel_booking.Add(hb);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return "0";
            }
            return idsession;
        }
        public string addNew(hotel_booking hb,int idhotel, string hotelname,string provin, int idroom, string roomname, int numofroom,int numofguest, int fromdate, int todate)//hotel_booking hb, 
        {
            string idsession = Guid.NewGuid().ToString();
            try
            {
                hb.idsession = idsession;
                hb.idhotel = idhotel;
                hb.hotelname = hotelname;
                hb.provin = provin;
                hb.idroom = idroom;
                hb.roomname = roomname;
                hb.numofroom = numofroom;
                hb.numofguest = numofguest;
                hb.fromdate = fromdate;
                hb.todate = todate;
                hb.datebook = DateTime.Now;
                hb.status = Config.booking_status_step1;
                hb.typebook = Config.typebook1;
                hb.statusadmin = Config.admin_status_step1;
                hb.extrabedfee = 0;
                hb.extraotherfee = 0;
                db.hotel_booking.Add(hb);
                db.SaveChanges();
            }catch(Exception ex){
                return "0";
            }
            return idsession;
        }
        public string getTotalPrice(int idbooking,int idhotel,int idroom,int numofroom,int fromdate,int todate,int typebook) {
            int totalprice = 0;
            int discount = 0;
            var price1 = 0;
            int price2 = 0;
            int totaldays = 0;
            int tes1 = 0;
            int test2 = 0;
            try
            {
                //Neu da co roi thi tra ve           
                try
                {
                    price1 = (int)db.hotel_booking.Where(o => o.id == idbooking).Max(o => o.totalprice);
                }
                catch (Exception ex) {
                    price1 = 0;
                }
                //Neu da co tu tinh bang tay thi tra ve           
                if (price1 != 0) return price1.ToString();
               
                //Giá của ngày bắt đầu đến
                price1 = UtilDb.getTotalPrice(idhotel, idroom, fromdate, typebook);
                //Giá của ngày kết thúc
                price2 = UtilDb.getTotalPrice(idhotel, idroom, todate, typebook);
                int extraprice = UtilDb.TotalExtraFeeBasic(idbooking);
                //Neu la choupon thi tinh luon
                if (typebook != 0)
                {
                    totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                    totalprice = totaldays * numofroom * (price1 + extraprice);
                    return totalprice.ToString();
                }
                //Truong hop 1: Thoi gian dat nam ngoai hai khoang khuyen mai va khong bao gom khoang khuyen mai
                discount = 0;
                try
                {
                    tes1 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Min(o => o.discount);
                    
                }
                catch (Exception ex) {
                    tes1 = -1;
                    try
                    {
                        test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Min(o => o.discount);
                    }
                    catch (Exception ex2) {
                        test2 = 0;
                        try
                        {
                            test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Min(o => o.discount);
                        }
                        catch (Exception ex3) {
                            test2 = -1;
                        }
                    }
                }
                if (tes1==-1 && test2==-1)
                {
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());
                        totalprice = totaldays1 * numofroom * price1 + totaldays2 * numofroom * price2 + (totaldays1 + totaldays2) * numofroom * extraprice;
                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1 + extraprice);
                    }
                    return totalprice.ToString();
                }
                //Trường hợp khác: Thời gian nằm xen kẽ với khoảng khuyến mại
                ////Ví dụ đặt phòng từ 25/08 đến 5/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex) {
                    discount = 0;
                }

                if (discount != 0)
                {
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        //Nằm ở hai khoảng giá khác nhau, có 4 giai đoạn
                        string sDate = todate.ToString();
                        string t1date = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), t1date);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(t1date, tdate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        int totalday4 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + totalday4 * numofroom * price2 + (totalday1 + totalday2 + totalday3 + totalday4) * numofroom * extraprice;
                    }
                    else {
                        //Nằm ở hai khoảng giá = nhau, có 3 giai đoạn
                    
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2+  (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    return totalprice.ToString();
                }
                //Truong hop khac: Thoi gian nam xen ke voi khoang khuyen mai
                //totalprice=(price1*songaytindongia1+price2*songaytinhdongia2-giakhuyenmai1*songaytinhkhuyenmai1thucsu1-giakhuyenmai2*songaytinhkhuyenmai2)*sophong+extraprice*sophong*tongsongay;
                //Nếu ngày đến nằm trong khoảng khuyến mại, còn ngày đi nằm ngoài khoảng khuyến mại    
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex) {
                    discount = 0;
                }
                
                if (discount != 0)
                {
                    //Neu ngay den va ngay di nam o 2 giai doan khac nhau
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                      
                        string sDate = todate.ToString();
                        int t1date = int.Parse(sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01");//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        if (t1date>tdate){
                            int temp=t1date;
                            t1date=tdate;
                            tdate=temp;
                        }
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), t1date.ToString());//Khoảng giá 1, price1; co khuyen mai
                        int totalday2 = Config.getDateDiff(t1date.ToString(), tdate.ToString());//Khoảng giá 2, price2 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1pro + totalday2 * numofroom * price2pro + totalday3 * numofroom * price2+(totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    else
                    {
                        //Lấy ra ngày cuối cùng của khuyến mại
                        //Có 2 trường hợp, trường hợp 1, todate nằm ở tháng khác với giá khác, ngày kết thúc nằm ngoài khoảng khuyến mại có vài ngày
                        //Ví dụ đặt phòng từ 01/9 đến 4/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                        //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), tdate.ToString());
                        totalprice += (int)(totalday1 * (price1 - price1 * discount / 100) * numofroom);
                        int totalday2 = Config.getDateDiff(tdate.ToString(), todate.ToString());
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice += (int)(totalday2 * price2 * numofroom);
                        totalprice += (int)(totaldays * extraprice * numofroom);                        
                    }
                    return totalprice.ToString();
                }
                //Truong hop 2, lich dat phong nam tron ven ben trong khoang khuyen mai
                //Ví dụ đặt phòng từ 30/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate >= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                { 
                    //Do nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());                        
                        totalprice = totaldays1 * numofroom * price1pro + totaldays2 * numofroom * price2pro + (totaldays1 + totaldays2) * numofroom * extraprice;
                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1pro + extraprice);
                    }
                    return totalprice.ToString();
                }
                //Truong hop 2, ngay di nam xen ke vao khoang khuyen mai
                //Ví dụ đặt phòng từ 27/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                {
                    //Do chỉ có ngày đi nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 không khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    //Có 3 khoảng giá
                    int price1pro = price1 - price1 * discount / 100; ;
                    int price2pro = price2 - price2 * discount / 100;//giá khuyến mại của tháng mới
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(tdate, todate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    else
                    {
                        
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), todate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + (totalday1 + totalday2) * numofroom * extraprice;
                    }
                    return totalprice.ToString();
                }

            }
            catch (Exception ex) {
                return "0";
            }
            return totalprice.ToString();
        }
        public class invoiceitem
        {   
            public int fromdate { get; set; }
            public int todate { get; set; }
            public int totalday { get; set; }
            public int price { get; set; }
            public int numofroom { get; set; }
            public int total { get; set; }
            public int totalextrabed { get; set; }
            public int totalextraother { get; set; }
            public int totalprice { get; set; }
           
        }
        public string viewInvoice(int idbooking,int idhotel,int idroom,int numofroom,int fromdate,int todate,int typebook){
            invoiceitem[] items=new invoiceitem[5];
            items[0] = new invoiceitem();
            items[1] = new invoiceitem();
            items[2] = new invoiceitem();
            items[3] = new invoiceitem();
            items[0].numofroom = numofroom;
            items[1].numofroom = numofroom;
            items[2].numofroom = numofroom;
            items[3].numofroom = numofroom;
            int totalprice = 0;
            int discount = 0;
            var price1 = 0;
            int price2 = 0;
            int totaldays = 0;
            int tes1 = 0;
            int test2 = 0;
            try
            {
                //Neu da co roi thi tra ve           
                try
                {
                    price1 = (int)db.hotel_booking_invoice.Where(o => o.idbooking == idbooking).Max(o => o.totalprice);
                }
                catch (Exception ex)
                {
                    price1 = 0;
                }
                //Neu da co tu tinh bang tay thi tra ve           
                if (price1 != 0) {
                    var p = (from q in db.hotel_booking_invoice where q.idbooking == idbooking select q).OrderBy(o=>o.fromdate).ThenBy(o=>o.todate);
                    return JsonConvert.SerializeObject(p.ToList());
                }

                //Giá của ngày bắt đầu đến
                price1 = UtilDb.getTotalPrice(idhotel, idroom, fromdate, typebook);
                //Giá của ngày kết thúc
                price2 = UtilDb.getTotalPrice(idhotel, idroom, todate, typebook);
                int extraprice = UtilDb.TotalExtraFeeBasic(idbooking);
                int extrapricebed = UtilDb.TotalExtraBedFeeBasic(idbooking);
                int extrapriceother = UtilDb.TotalExtraOtherFeeBasic(idbooking);
                //Neu la choupon thi tinh luon
                if (typebook != 0)
                {
                    totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                    totalprice = totaldays * numofroom * (price1 + extraprice);
                    items[0].fromdate = fromdate;
                    items[0].todate = todate;
                    items[0].totalday = totaldays;
                    items[0].price = price1;
                    items[0].total = totaldays * numofroom * price1;
                    items[0].totalextrabed = totaldays * numofroom * extrapricebed;
                    items[0].totalextraother = totaldays * numofroom * extrapriceother;
                    items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                    return JsonConvert.SerializeObject(items.ToList());
                }
                //Truong hop 1: Thoi gian dat nam ngoai hai khoang khuyen mai
                discount = 0;
                try
                {
                    tes1 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Min(o => o.discount);

                }
                catch (Exception ex)
                {
                    tes1 = -1;
                    try
                    {
                        test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Min(o => o.discount);
                    }
                    catch (Exception ex2)
                    {
                        test2 = 0;
                        try
                        {
                            test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Min(o => o.discount);
                        }
                        catch (Exception ex3)
                        {
                            test2 = -1;
                        }
                    }
                }
                if (tes1 == -1 && test2 == -1)
                {
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());
                        totalprice = totaldays1 * numofroom * price1 + totaldays2 * numofroom * price2 + (totaldays1 + totaldays2) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = int.Parse(tdate);
                        items[0].price = price1;
                        items[0].totalday = totaldays1;
                        items[0].total = totaldays1 * numofroom * price1;
                        items[0].totalextrabed = totaldays1 * numofroom * extrapricebed;
                        items[0].totalextraother = totaldays1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;

                        items[1].fromdate = int.Parse(tdate);
                        items[1].todate = todate;
                        items[1].price = price2;
                        items[1].totalday = totaldays2;
                        items[1].total = totaldays2 * numofroom * price2;
                        items[1].totalextrabed = totaldays2 * numofroom * extrapricebed;
                        items[1].totalextraother = totaldays2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());

                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1 + extraprice);
                        items[0].fromdate = fromdate;
                        items[0].todate = todate;
                        items[0].price = price1;
                        items[0].totalday = totaldays;
                        items[0].total = totaldays * numofroom * price1;
                        items[0].totalextrabed = totaldays * numofroom * extrapricebed;
                        items[0].totalextraother = totaldays * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());
                    }
                    //return totalprice.ToString();
                }
                //Trường hợp khác: Thời gian nằm xen kẽ với khoảng khuyến mại
                ////Ví dụ đặt phòng từ 25/08 đến 5/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }

                if (discount != 0)
                {
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        //Nằm ở hai khoảng giá khác nhau, có 4 giai đoạn
                        string sDate = todate.ToString();
                        string t1date = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), t1date);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(t1date, tdate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        int totalday4 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + totalday4 * numofroom * price2 + (totalday1 + totalday2 + totalday3 + totalday4) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = fdate;
                        items[0].price = price1;
                        items[0].totalday = totalday1;
                        items[0].total = totalday1 * numofroom * price1;
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                        items[1].fromdate = fdate;
                        items[1].todate = int.Parse(t1date);
                        items[1].price = price1pro;
                        items[1].totalday = totalday2;
                        items[1].total = totalday2 * numofroom * price1pro;
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        items[2].fromdate = int.Parse(t1date);
                        items[2].todate = tdate;
                        items[2].price = price2pro;
                        items[2].totalday = totalday3;
                        items[2].total = totalday3 * numofroom * price2pro;
                        items[2].totalextrabed = totalday3 * numofroom * extrapricebed;
                        items[2].totalextraother = totalday3 * numofroom * extrapriceother;
                        items[2].totalprice = items[2].total + items[2].totalextrabed + items[2].totalextraother;
                        items[3].fromdate = tdate;
                        items[3].todate = todate;
                        items[3].price = price2;
                        items[3].totalday = totalday4;
                        items[3].total = totalday4 * numofroom * price2;
                        items[3].totalextrabed = totalday4 * numofroom * extrapricebed;
                        items[3].totalextraother = totalday4 * numofroom * extrapriceother;
                        items[3].totalprice = items[3].total + items[3].totalextrabed + items[3].totalextraother;
                    }
                    else
                    {
                        //Nằm ở hai khoảng giá = nhau, có 3 giai đoạn
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2 + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = fdate;
                        items[0].price = price1;
                        items[0].totalday = totalday1;
                        items[0].total = totalday1 * numofroom * price1;
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                        items[1].fromdate = fdate;
                        items[1].todate = tdate;
                        items[1].price = price1pro;
                        items[1].totalday = totalday2;
                        items[1].total = totalday2 * numofroom * price1pro;
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        items[2].fromdate = tdate;
                        items[2].todate = todate;
                        items[2].price = price2;
                        items[2].totalday = totalday3;
                        items[2].total = totalday3 * numofroom * price2;
                        items[2].totalextrabed = totalday3 * numofroom * extrapricebed;
                        items[2].totalextraother = totalday3 * numofroom * extrapriceother;
                        items[2].totalprice = items[2].total + items[2].totalextrabed + items[2].totalextraother;
                    }
                    return JsonConvert.SerializeObject(items.ToList());
                }
                //Truong hop khac: Thoi gian nam xen ke voi khoang khuyen mai
                //totalprice=(price1*songaytindongia1+price2*songaytinhdongia2-giakhuyenmai1*songaytinhkhuyenmai1thucsu1-giakhuyenmai2*songaytinhkhuyenmai2)*sophong+extraprice*sophong*tongsongay;
                //Nếu ngày đến nằm trong khoảng khuyến mại, còn ngày đi nằm ngoài khoảng khuyến mại    
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }

                if (discount != 0)
                {
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {

                        string sDate = todate.ToString();
                        int t1date = int.Parse(sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01");//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        if (t1date > tdate)
                        {
                            int temp = t1date;
                            t1date = tdate;
                            tdate = temp;
                        }
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), t1date.ToString());//Khoảng giá 1, price1; co khuyen mai
                        int totalday2 = Config.getDateDiff(t1date.ToString(), tdate.ToString());//Khoảng giá 2, price2 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1pro + totalday2 * numofroom * price2pro + totalday3 * numofroom * price2 + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = t1date;
                        items[0].price = price1pro;
                        items[0].totalday = totalday1;
                        items[0].total = totalday1 * numofroom * price1pro;
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                        items[1].fromdate = t1date;
                        items[1].todate = tdate;
                        items[1].price = price2pro;
                        items[1].totalday = totalday2;
                        items[1].total = totalday2 * numofroom * price2pro;
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        items[2].fromdate = tdate;
                        items[2].todate = todate;
                        items[2].price = price2;
                        items[2].totalday = totalday3;
                        items[2].total = totalday3 * numofroom * price2;
                        items[2].totalextrabed = totalday3 * numofroom * extrapricebed;
                        items[2].totalextraother = totalday3 * numofroom * extrapriceother;
                        items[2].totalprice = items[2].total + items[2].totalextrabed + items[2].totalextraother;
                        
                        return JsonConvert.SerializeObject(items.ToList());
                    }
                    else
                    {
                        //Lấy ra ngày cuối cùng của khuyến mại
                        //Có 2 trường hợp, trường hợp 1, todate nằm ở tháng khác với giá khác, ngày kết thúc nằm ngoài khoảng khuyến mại có vài ngày
                        //Ví dụ đặt phòng từ 01/9 đến 4/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                        //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), tdate.ToString());
                        totalprice += (int)(totalday1 * (price1 - price1 * discount / 100) * numofroom);
                        int totalday2 = Config.getDateDiff(tdate.ToString(), todate.ToString());
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice += (int)(totalday2 * price2 * numofroom);
                        totalprice += (int)(totaldays * extraprice * numofroom);

                        items[0].fromdate = fromdate;
                        items[0].todate = tdate;
                        items[0].price = price1 - price1 * discount / 100;
                        items[0].totalday = totalday1;
                        items[0].total = (int)(totalday1 * (price1 - price1 * discount / 100) * numofroom);
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;

                        items[1].fromdate = tdate;
                        items[1].todate = todate;
                        items[1].price = price2;
                        items[1].totalday = totalday2;
                        items[1].total = (int)(totalday2 * price2 * numofroom);
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());
                    }

                }
                //Truong hop 2, lich dat phong nam tron ven ben trong khoang khuyen mai
                //Ví dụ đặt phòng từ 30/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate >= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                {
                    //Do nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());
                        totalprice = totaldays1 * numofroom * price1pro + totaldays2 * numofroom * price2pro + (totaldays1 + totaldays2) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = int.Parse(tdate);
                        items[0].price = price1pro;
                        items[0].totalday = totaldays1;
                        items[0].total = totaldays1 * numofroom * price1pro;
                        items[0].totalextrabed = totaldays1 * numofroom * extrapricebed;
                        items[0].totalextraother = totaldays1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;

                        items[1].fromdate = int.Parse(tdate);
                        items[1].todate = todate;
                        items[1].price = price2pro;
                        items[1].totalday = totaldays2;
                        items[1].total = totaldays2 * numofroom * price2pro;
                        items[1].totalextrabed = totaldays2 * numofroom * extrapricebed;
                        items[1].totalextraother = totaldays2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());
                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1pro + extraprice);
                        items[0].fromdate = fromdate;
                        items[0].todate = todate;
                        items[0].price = price1pro;
                        items[0].totalday = totaldays;
                        items[0].total = totaldays * numofroom * price1pro;
                        items[0].totalextrabed = totaldays * numofroom * extrapricebed;
                        items[0].totalextraother = totaldays * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());
                    }
                    
                }
                //Truong hop 2, ngay di nam xen ke vao khoang khuyen mai
                //Ví dụ đặt phòng từ 27/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                {
                    //Do chỉ có ngày đi nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 không khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    //Có 3 khoảng giá
                    int price1pro = price1 - price1 * discount / 100; ;
                    int price2pro = price2 - price2 * discount / 100;//giá khuyến mại của tháng mới
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(tdate, todate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = fdate;
                        items[0].price = price1;
                        items[0].totalday = totalday1;
                        items[0].total = totalday1 * numofroom * price1;
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;

                        items[1].fromdate = fdate;
                        items[1].todate = int.Parse(tdate);
                        items[1].price = price1pro;
                        items[1].totalday = totalday2;
                        items[1].total = totalday2 * numofroom * price1pro;
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;

                        items[2].fromdate = int.Parse(tdate);
                        items[2].todate = todate;
                        items[2].price = price2pro;
                        items[2].totalday = totalday3;
                        items[2].total = totalday3 * numofroom * price2pro;
                        items[2].totalextrabed = totalday3 * numofroom * extrapricebed;
                        items[2].totalextraother = totalday3 * numofroom * extrapriceother;
                        items[2].totalprice = items[2].total + items[2].totalextrabed + items[2].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());
                    }
                    else
                    {

                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), todate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + (totalday1 + totalday2) * numofroom * extraprice;
                        items[0].fromdate = fromdate;
                        items[0].todate = fdate;
                        items[0].price = price1;
                        items[0].totalday = totalday1;
                        items[0].total = totalday1 * numofroom * price1;
                        items[0].totalextrabed = totalday1 * numofroom * extrapricebed;
                        items[0].totalextraother = totalday1 * numofroom * extrapriceother;
                        items[0].totalprice = items[0].total + items[0].totalextrabed + items[0].totalextraother;

                        items[1].fromdate = fdate;
                        items[1].todate = todate;
                        items[1].price = price1pro;
                        items[1].totalday = totalday2;
                        items[1].total = totalday2 * numofroom * price1pro;
                        items[1].totalextrabed = totalday2 * numofroom * extrapricebed;
                        items[1].totalextraother = totalday2 * numofroom * extrapriceother;
                        items[1].totalprice = items[1].total + items[1].totalextrabed + items[1].totalextraother;
                        return JsonConvert.SerializeObject(items.ToList());

                    }
                    
                }

            }
            catch (Exception ex)
            {
                return "";
            }
            return JsonConvert.SerializeObject(items.ToList());
        }
        public int TotalPrice(int idbooking,int idhotel, int idroom, int numofroom, int fromdate, int todate, int typebook)
        {
            int totalprice = 0;
            int discount = 0;
            var price1 = 0;
            int price2 = 0;
            int totaldays = 0;
            int tes1 = 0;
            int test2 = 0;
            try
            {
                //Neu da co roi thi tra ve           
                try
                {
                    price1 = (int)db.hotel_booking.Where(o => o.id == idbooking).Max(o => o.totalprice);
                }
                catch (Exception ex)
                {
                    price1 = 0;
                }
                //Neu da co tu tinh bang tay thi tra ve           
                if (price1 != 0) return price1;

                //Giá của ngày bắt đầu đến
                price1 = UtilDb.getTotalPrice(idhotel, idroom, fromdate, typebook);
                //Giá của ngày kết thúc
                price2 = UtilDb.getTotalPrice(idhotel, idroom, todate, typebook);
                int extraprice = UtilDb.TotalExtraFeeBasic(idbooking);
                //Neu la choupon thi tinh luon
                if (typebook != 0)
                {
                    totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                    totalprice = totaldays * numofroom * (price1 + extraprice);
                    return totalprice;
                }
                //Truong hop 1: Thoi gian dat nam ngoai hai khoang khuyen mai
                discount = 0;
                try
                {
                    tes1 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Min(o => o.discount);

                }
                catch (Exception ex)
                {
                    tes1 = -1;
                    try
                    {
                        test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Min(o => o.discount);
                    }
                    catch (Exception ex2)
                    {
                        test2 = 0;
                        try
                        {
                            test2 = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Min(o => o.discount);
                        }
                        catch (Exception ex3)
                        {
                            test2 = -1;
                        }
                    }
                }
                if (tes1 == -1 && test2 == -1)
                {
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());
                        totalprice = totaldays1 * numofroom * price1 + totaldays2 * numofroom * price2 + (totaldays1 + totaldays2) * numofroom * extraprice;
                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1 + extraprice);
                    }
                    return totalprice;
                }
                //Trường hợp khác: Thời gian nằm xen kẽ với khoảng khuyến mại
                ////Ví dụ đặt phòng từ 25/08 đến 5/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate >= fromdate).Where(o => o.fdate <= todate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }

                if (discount != 0)
                {
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        //Nằm ở hai khoảng giá khác nhau, có 4 giai đoạn
                        string sDate = todate.ToString();
                        string t1date = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), t1date);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(t1date, tdate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        int totalday4 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + totalday4 * numofroom * price2 + (totalday1 + totalday2 + totalday3 + totalday4) * numofroom * extraprice;
                    }
                    else
                    {
                        //Nằm ở hai khoảng giá = nhau, có 3 giai đoạn

                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.fdate >= fromdate).Where(o => o.tdate <= todate).Where(o => o.tdate >= fromdate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2 + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    return totalprice;
                }
                //Truong hop khac: Thoi gian nam xen ke voi khoang khuyen mai
                //totalprice=(price1*songaytindongia1+price2*songaytinhdongia2-giakhuyenmai1*songaytinhkhuyenmai1thucsu1-giakhuyenmai2*songaytinhkhuyenmai2)*sophong+extraprice*sophong*tongsongay;
                //Nếu ngày đến nằm trong khoảng khuyến mại, còn ngày đi nằm ngoài khoảng khuyến mại    
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }

                if (discount != 0)
                {
                    //Neu ngay den va ngay di nam o 2 giai doan khac nhau
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {

                        string sDate = todate.ToString();
                        int t1date = int.Parse(sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01");//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.fdate);
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        if (t1date > tdate)
                        {
                            int temp = t1date;
                            t1date = tdate;
                            tdate = temp;
                        }
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), t1date.ToString());//Khoảng giá 1, price1; co khuyen mai
                        int totalday2 = Config.getDateDiff(t1date.ToString(), tdate.ToString());//Khoảng giá 2, price2 có khuyến mại                        
                        int totalday3 = Config.getDateDiff(tdate.ToString(), todate.ToString());//Khoảng giá 3, price2 ko khuyến mại
                        totalprice = totalday1 * numofroom * price1pro + totalday2 * numofroom * price2pro + totalday3 * numofroom * price2 + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    else
                    {
                        //Lấy ra ngày cuối cùng của khuyến mại
                        //Có 2 trường hợp, trường hợp 1, todate nằm ở tháng khác với giá khác, ngày kết thúc nằm ngoài khoảng khuyến mại có vài ngày
                        //Ví dụ đặt phòng từ 01/9 đến 4/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, giá tháng 9 là 9470000
                        //Tìm ra ngày giai đoạn 1 chưa khuyến mại
                        int tdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate <= todate).Max(o => o.tdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), tdate.ToString());
                        totalprice += (int)(totalday1 * (price1 - price1 * discount / 100) * numofroom);
                        int totalday2 = Config.getDateDiff(tdate.ToString(), todate.ToString());
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice += (int)(totalday2 * price2 * numofroom);
                        totalprice += (int)(totaldays * extraprice * numofroom);
                    }
                    return totalprice;
                }
                //Truong hop 2, lich dat phong nam tron ven ben trong khoang khuyen mai
                //Ví dụ đặt phòng từ 30/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Where(o => o.tdate >= todate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                {
                    //Do nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    int price1pro = price1 - price1 * discount / 100;
                    int price2pro = price2 - price2 * discount / 100;
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng
                        int totaldays1 = Config.getDateDiff(fromdate.ToString(), tdate);
                        int totaldays2 = Config.getDateDiff(tdate, todate.ToString());
                        totalprice = totaldays1 * numofroom * price1pro + totaldays2 * numofroom * price2pro + (totaldays1 + totaldays2) * numofroom * extraprice;
                    }
                    else
                    {
                        totaldays = Config.getDateDiff(fromdate.ToString(), todate.ToString());
                        totalprice = totaldays * numofroom * (price1pro + extraprice);
                    }
                    return totalprice;
                }
                //Truong hop 2, ngay di nam xen ke vao khoang khuyen mai
                //Ví dụ đặt phòng từ 27/8 đến mùng 2/9, khuyến mại từ 29/8 đến 3/9, giá tháng 8 là 9370000, 9 là 9470000
                try
                {
                    discount = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.discount);
                }
                catch (Exception ex)
                {
                    discount = 0;
                }
                if (discount != 0)
                {
                    //Do chỉ có ngày đi nằm trọn vẹn trong khuyến mại nên có 2 khoảng giá, giá price1 không khuyến mại và giá price2 khuyến mại
                    //Nếu ngày đến và ngày đi nằm ở hai tháng có hai mức giá khác nhau
                    //Có 3 khoảng giá
                    int price1pro = price1 - price1 * discount / 100; ;
                    int price2pro = price2 - price2 * discount / 100;//giá khuyến mại của tháng mới
                    if (price1 != price2)
                    {
                        string sDate = todate.ToString();
                        string tdate = sDate.Substring(0, 4) + sDate.Substring(4, 2) + "01";//Ngày đầu của tháng đảm bảo >=tdate
                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), tdate);//Khoảng giá 2, price1 có khuyến mại
                        int totalday3 = Config.getDateDiff(tdate, todate.ToString());//Khoảng giá 3, price2 có khuyến mại
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + totalday3 * numofroom * price2pro + (totalday1 + totalday2 + totalday3) * numofroom * extraprice;
                    }
                    else
                    {

                        int fdate = (int)db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= todate).Where(o => o.tdate >= todate).Where(o => o.fdate >= fromdate).Max(o => o.fdate);
                        int totalday1 = Config.getDateDiff(fromdate.ToString(), fdate.ToString());//Khoảng giá 1, price1; không khuyến mại
                        int totalday2 = Config.getDateDiff(fdate.ToString(), todate.ToString());//Khoảng giá 2, price1 có khuyến mại                        
                        totalprice = totalday1 * numofroom * price1 + totalday2 * numofroom * price1pro + (totalday1 + totalday2) * numofroom * extraprice;
                    }
                    return totalprice;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }
            return totalprice;
        }
        
        public ActionResult Print(int idhotel, string code) {
            ViewBag.code = code;
            ViewBag.idhotel = idhotel;
            //Thong tin khach san
            var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);            
            var rs = p.ToList()[0];
            var rule = db.hotel_rule.First(o => o.rulehotel != null);
            ViewBag.datebook = rs.datebook;
            ViewBag.hotelname = rs.hotelname;
            ViewBag.cname = rs.cname;
            ViewBag.cemail = rs.cemail;
            ViewBag.cphone = rs.cphone;
            ViewBag.caddress = rs.caddress;
            ViewBag.cnote = rs.cnote;
            if (rs.fromdate != null || rs.todate != null)
            {
                ViewBag.fdate = rs.fromdate;
                ViewBag.tdate = rs.todate;
            }
            else {
                ViewBag.fdate = "";
                ViewBag.tdate = "";
            }
            ViewBag.numofroom = rs.numofroom;
            ViewBag.numofguest = rs.numofguest;
            ViewBag.roomname = rs.roomname;
            var p2 = (from q2 in db.hotels where q2.id==idhotel select q2).Take(1);
            var rs2 = p2.ToList()[0];
            ViewBag.address = rs2.address;
            ViewBag.phone = rs2.phone;
            ViewBag.hcheckin = rs2.hcheckin;
            ViewBag.hcheckout = rs2.hcheckout;           
            ViewBag.ruleextra = rule.rulehotel;//rs2.ruleextra;
            ViewBag.ruleroom = "";// rs2.ruleroom;
            //Tim khuyen mai va gia ca
            var price = 0;
            int idroom = (int)rs.idroom;
            price = TotalPrice(rs.id,idhotel,idroom,(int)rs.numofroom,(int)rs.fromdate,(int)rs.todate,(int)rs.typebook);
            //ViewBag.extrafee = UtilDb.TotalExtraFeeBasic(rs.id)+"/ngày";
            var pi = (from qi in db.hotel_room where qi.deleted == 0 && qi.id == idroom select qi.breakfast).Take(1);
            string include = pi.ToList()[0];
            ViewBag.include = include;
            //var discount = db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= rs.fromdate).Where(o => o.tdate >= rs.fromdate).Max(o => o.discount);
            //if (discount == null) discount = 0;
            //ViewBag.discount = discount;            
            ViewBag.totalprice = Config.formatNumber(price);
            ViewBag.invoice = UtilDb.getInvoiceDetail(rs.id);
            p = null;
            rs = null;
            p2 = null;
            rs2 = null;
            pi = null;
            return View();
        }
        public string PaymentProccess(int idhotel, string code, int type, long amount,string info)
        {
            info = "InvoiceNo" + info;
            Payment payment = new Payment();
            Hashtable hash = new Hashtable();
            long samount = 0;
            if (type == 1) {
                payment.SecureSecret = "198BE3F2E8C75A53F38C1C4A5B6DBA27";
                payment.VirtualPaymentClientUrl = "https://paymentcert.smartlink.com.vn:8181/vpcpay.do";
                hash.Add("vpc_AccessCode", "ECAFAB");
                hash.Add("vpc_Merchant", "SMLTEST");
                hash.Add("vpc_Currency", "VND");
                hash.Add("vpc_Version", "1.1");
                samount=amount*100;
                hash.Add("vpc_Amount", samount.ToString());
            }else{
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
            hash.Add("vpc_ReturnURL", Config.domain + "/HotelBooking/ConfirmPayment?idhotel=" + idhotel + "&code=" + code);
            hash.Add("vpc_BackURL", Config.domain + "/HotelBooking/Payment?idhotel=" + idhotel + "&code=" + code);
            hash.Add("vpc_TicketNo", idhotel.ToString());//Request.ServerVariables["REMOTE_ADDR"]
            return payment.getRedirectUrl(hash);
            //return "";
        }
        public ActionResult ConfirmPayment(int idhotel, string code) {
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
                            var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
                            var rs = p.ToList()[0];
                            ViewBag.hotelname = rs.hotelname;
                            ViewBag.cname = rs.cname;
                            ViewBag.cemail = rs.cemail;
                            ViewBag.cphone = rs.cphone;
                            ViewBag.caddress = rs.caddress;
                            ViewBag.info = "Thanh toán hóa đơn đặt phòng khách sạn " + ViewBag.hotelname + " thành công. Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!";
                            info = info + "<br>Khách sạn: " + ViewBag.hotelname;
                            info = info + "<br>Từ ngày " + Config.convertFromDateIdToDateString(rs.fromdate.ToString()) + ", đến ngày " + Config.convertFromDateIdToDateString(rs.todate.ToString());
                            info = info + "<br>Họ tên: " + ViewBag.cname;
                            info = info + "<br>Email: " + ViewBag.cemail;
                            info = info + "<br>Phone: " + ViewBag.cphone;
                            info = info + "<br>Địa chỉ: " + ViewBag.caddress;
                            info = info + "<br>Số tiền: " + Config.formatNumber(price);
                            hotel_payment_info hpi = new hotel_payment_info();
                            hpi.amount = price;
                            hpi.info = info;
                            hpi.transactionNo = transactionNo;
                            hpi.datetime = DateTime.Now;
                            db.hotel_payment_info.Add(hpi);
                            db.SaveChanges();
                            string update = updatePayment(code, idhotel, Config.payment_method2);
                            Config.mail(Config.emailcompany, ViewBag.cemail, "Chuyển khoản đặt phòng khách sạn thành công", Config.passemailcompany, "Bạn đã chuyển khoản thanh toán hóa đơn thành công với thông tin sau:<br> " + info + "<br>Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!");
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
            }catch(Exception ex){
                ViewBag.info = "Thanh toán chưa thành công. Kiểm tra lại đường truyền hoặc thông tin thẻ!";
            }
                //string status = "";
                //int type = 0;
                //if (Request.QueryString["vpc_ResponseCode"] != null)
                //{
                //    status = Request.QueryString["vpc_ResponseCode"].ToString();//Noi dia
                //    type = 1;
                //}
                //else
                //{
                //    status = Request.QueryString["vpc_TxnResponseCode"].ToString();//Quoc te
                //    type = 2;
                //}
                //string vpc_AccessCode = "72AD46B6";
                //string Secure_Secret = "198BE3F2E8C75A53F38C1C4A5B6DBA27";
                //if (type == 2) Secure_Secret = "B575ED17E000D6E2BD8634FD0E6B042D";
                //string vpc_Amount = Request.QueryString["vpc_Amount"].ToString();
                //string vpc_Locale = Request.QueryString["vpc_Locale"].ToString();
                //string vpc_Merchant = Request.QueryString["vpc_Merchant"].ToString();
                //string vpc_OrderInfo = Request.QueryString["vpc_OrderInfo"].ToString();
                //string vpc_ReturnURL = Config.domain + "/HotelBooking/ConfirmPayment?idhotel=" + idhotel + "&code=" + code;
                //string vpc_TicketNo = idhotel.ToString();
                //string vpc_TransactionNo = Request.QueryString["vpc_TransactionNo"].ToString();
                //string vpc_Version = Request.QueryString["vpc_Version"].ToString();
                //string vpc_SecureHash = Request.QueryString["vpc_SecureHash"].ToString();
                //string md5_input = Secure_Secret + vpc_AccessCode + vpc_Amount + vpc_Locale + vpc_Merchant +
                //        vpc_OrderInfo + vpc_ReturnURL + vpc_TicketNo + vpc_TransactionNo + vpc_Version;
              
                //MD5 md5Hash = MD5.Create();
                //string finalSecureHash=Config.GetMd5Hash(md5Hash, md5_input);

            //    if (status.Equals("0") && finalSecureHash.ToLower().Equals(vpc_SecureHash.ToLower()))
            //    {
            //        string amount = Request.QueryString["vpc_Amount"].ToString();
            //        int price=(int)long.Parse(amount);
            //        if (type == 1) price = price / 100;
            //        string transactionNo = Request.QueryString["vpc_TransactionNo"].ToString();
            //        string info = "<B>"+Request.QueryString["vpc_OrderInfo"].ToString()+"</B>";
            //        var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
            //        var rs = p.ToList()[0];
            //        ViewBag.hotelname = rs.hotelname;
            //        ViewBag.cname = rs.cname;
            //        ViewBag.cemail = rs.cemail;
            //        ViewBag.cphone = rs.cphone;
            //        ViewBag.caddress = rs.caddress;
            //        ViewBag.info = "Thanh toán hóa đơn đặt phòng khách sạn " + ViewBag.hotelname + " thành công. Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!";
            //        info = info + "<br>Khách sạn: " + ViewBag.hotelname;
            //        info = info + "<br>Từ ngày " + Config.convertFromDateIdToDateString(rs.fromdate.ToString()) + ", đến ngày " + Config.convertFromDateIdToDateString(rs.todate.ToString());
            //        info = info + "<br>Họ tên: " + ViewBag.cname;
            //        info = info + "<br>Email: " + ViewBag.cemail;
            //        info = info + "<br>Phone: " + ViewBag.cphone;
            //        info = info + "<br>Địa chỉ: " + ViewBag.caddress;
            //        info = info + "<br>Số tiền: " + Config.formatNumber(price);
            //        hotel_payment_info hpi = new hotel_payment_info();
            //        hpi.amount = price;
            //        hpi.info = info;
            //        hpi.transactionNo = transactionNo;
            //        hpi.datetime = DateTime.Now;
            //        db.hotel_payment_info.Add(hpi);
            //        db.SaveChanges();                   
            //        string update = updatePayment(code, idhotel, Config.payment_method2);
            //        Config.mail(Config.emailcompany, ViewBag.cemail, "Chuyển khoản đặt phòng khách sạn thành công", Config.passemailcompany, "Bạn đã chuyển khoản thanh toán hóa đơn thành công với thông tin sau:<br> " + info + "<br>Xin trân trọng cảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!");
                   
            //    }
            //    else {
            //        ViewBag.info = "Thanh toán chưa thành công!";
            //    }
               
            //}catch(Exception ex){
            //    ViewBag.info = "Thanh toán chưa thành công!";
            //}
            return View();
        }
        public ActionResult Payment(int idhotel, string code)
        {
            ViewBag.code = code;
            ViewBag.idhotel = idhotel;
            var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            ViewBag.hotelname = rs.hotelname;
            ViewBag.cname = rs.cname;
            ViewBag.cemail = rs.cemail;
            ViewBag.cphone = rs.cphone;
            ViewBag.caddress = rs.caddress;
            ViewBag.cnote = rs.cnote;
            ViewBag.fdate = rs.fromdate;
            ViewBag.tdate = rs.todate;
            ViewBag.numofroom = rs.numofroom;
            ViewBag.roomname = rs.roomname;
            ViewBag.typebook = rs.typebook;
            ViewBag.info = rs.id;// "DatPhongKhachSan" + Config.unicodeToNoMarkCat(rs.hotelname) + "idbooking" + rs.id;
            //Tim khuyen mai va gia ca
            var price = 0;
            int idroom = (int)rs.idroom;
            price = TotalPrice(rs.id, idhotel, idroom, (int)rs.numofroom, (int)rs.fromdate, (int)rs.todate, (int)rs.typebook);
            //ViewBag.extrafee = UtilDb.TotalExtraFeeBasic(rs.id) + "/ngày";
            //var discount = db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= rs.fromdate).Where(o => o.tdate >= rs.fromdate).Max(o => o.discount);
            //if (discount == null) discount = 0;
            //ViewBag.discount = discount;
            int totaldays = Config.getDateDiff(rs.fromdate.ToString(), rs.todate.ToString());
            ViewBag.amount = price;
            ViewBag.totalprice = Config.formatNumber(price);
            return View();
        }
        public string updateExtraFeeStatus(int id, int? extrabedfee, int? extraotherfee)
        {
            try
            {
                string query = "update hotel_booking set extrabedfee=" + extrabedfee + ",extraotherfee=" + extraotherfee + " where id=" + id;
                db.Database.ExecuteSqlCommand(query);
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public string updateAdminStatus(int id, string statusadmin,int typebook,int idhotel,int idroom)
        {
            try
            {
                string query = "update hotel_booking set statusadmin=N'" + statusadmin + "' where id=" + id;
                db.Database.ExecuteSqlCommand(query);
                if (typebook == Config.typebook2) {
                    query = "update hotel_choupon set total=total-1 where idhotel=" + idhotel + " and idroom=" + idroom + " and total>0";
                    db.Database.ExecuteSqlCommand(query);
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public string updatePayment(string code, int idhotel,string paytype)//hotel_booking hb, 
        {
            try
            {
                string query = "update hotel_booking set status=N'" + Config.booking_status_step3 + paytype + "',paymenttype=N'" + paytype + "',statusadmin=N'"+Config.admin_status_step2+"' where idhotel=" + idhotel + " and idsession=N'" + code + "'";
                db.Database.ExecuteSqlCommand(query);
                //var p = (from q in db.hotel_booking where q.idsession.Contains(code) && q.idhotel==idhotel select q).Take(1);
                //var rs = p.ToList();
                try
                {
                    int typebook = (int)db.hotel_booking.Where(o => o.idsession.Contains(code)).Where(o => o.idhotel == idhotel).Min(o => o.typebook);
                    if (typebook == Config.typebook2)
                    {
                        int idroom = (int)db.hotel_booking.Where(o => o.idsession.Contains(code)).Where(o => o.idhotel == idhotel).Min(o => o.idroom);
                        query = "update hotel_choupon set total=total-1 where idhotel=" + idhotel + " and idroom=" + idroom + " and total>0";
                        db.Database.ExecuteSqlCommand(query);
                    }
                }
                catch (Exception ex1)
                { }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public string emailConfirmBooking(int idhotel,string code)
        {
            
            var p2 = (from q2 in db.hotels where q2.id == idhotel select q2).Take(1);
            var rs2 = p2.ToList()[0];
            var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            var rule = db.hotel_rule.First(o=>o.rulehotel!=null);
            
            //Tim khuyen mai va gia ca
            var price = 0;
            int idroom = (int)rs.idroom;
            //price = UtilDb.getTotalPrice(idhotel, idroom,(int)rs.fromdate, rs.typebook);
            price = TotalPrice(rs.id, idhotel, idroom, (int)rs.numofroom, (int)rs.fromdate, (int)rs.todate, (int)rs.typebook);
            //int extrafee = UtilDb.TotalExtraFeeBasic(rs.id);
            var pi = (from qi in db.hotel_room where qi.deleted == 0 && qi.id == idroom select qi.breakfast).Take(1);
            string include = pi.ToList()[0];

            //var discount = db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= rs.fromdate).Where(o => o.tdate >= rs.fromdate).Max(o => o.discount);
            //if (discount == null) discount = 0;
            string invoice = UtilDb.getInvoiceDetail(rs.id);
            int totaldays = 0;
            if (rs.todate != null && rs.fromdate != null) totaldays = Config.getDateDiff(rs.fromdate.ToString(), rs.todate.ToString()); 
            int totalprice = price;
            string body = "";
            string pathTemplate = HttpContext.Server.MapPath("/Scripts/tempEmailConfirm.txt");
            StreamReader SR = new StreamReader(pathTemplate);
            body = SR.ReadToEnd();
            SR.Close();
            body = body.Replace("@domain", Config.domain);
            body = body.Replace("@ViewBag.code",code);
            body = body.Replace("@ViewBag.datebook", rs.datebook.ToString());
            body = body.Replace("@ViewBag.cname", rs.cname);
            body = body.Replace("@ViewBag.hotelname", rs.hotelname);
            body = body.Replace("@ViewBag.address", rs2.address);
            body = body.Replace("@ViewBag.phone", rs2.phone);
            body = body.Replace("@ViewBag.fdate", Config.convertFromDateIdToDateString(rs.fromdate.ToString()));
            body = body.Replace("@ViewBag.tdate", Config.convertFromDateIdToDateString(rs.todate.ToString()));
            body = body.Replace("@ViewBag.hcheckin", rs2.hcheckin);
            body = body.Replace("@ViewBag.hcheckout", rs2.hcheckout);
            body = body.Replace("@totalnight", totaldays.ToString());
            body = body.Replace("@ViewBag.numofroom", rs.numofroom.ToString());
            body = body.Replace("@ViewBag.roomname", rs.roomname);
            body = body.Replace("@ViewBag.include", include);
            //body = body.Replace("@ViewBag.extrafee", extrafee.ToString()+"/ngày");
            body = body.Replace("@ViewBag.numofguest", rs.numofguest.ToString());
            body = body.Replace("@ViewBag.totalprice", Config.formatNumber(totalprice));           
            body = body.Replace("@ViewBag.invoice", invoice);
            body = body.Replace("@ViewBag.ruleroom", rule.rulehotel);
            body = body.Replace("@ViewBag.ruleextra", "");
            //body += "Quý khách vừa đặt " + rs.numofroom + " phòng " + rs.roomname + " tại khách sạn " + rs.hotelname + ", từ ngày " + Config.convertFromDateIdToDateString(rs.fromdate.ToString()) + " đến ngày " + Config.convertFromDateIdToDateString(rs.todate.ToString()) + ", Với tổng số tiền là:" + Config.formatNumber(totalprice) + " VND. Chúng tôi vừa đặt phòng thành công cho quý vị";
            //body += "\r\nTừ " + Config.domain;
            string toemail = rs.cemail;
            p = null;
            rs = null;
            p2 = null;
            rs2 = null;
            pi = null;
            return Config.mail(Config.emailcompany, toemail, Config.subjectEmailBooking, Config.passemailcompany, body);
           
        }
        public string emailConfirmPayment(int idhotel, string code)
        {
            var p2 = (from q2 in db.hotels where q2.id == idhotel select q2).Take(1);
            var rs2 = p2.ToList()[0];
            var p = (from q in db.hotel_booking where q.idhotel == idhotel && q.idsession.Contains(code) select q).Take(1);
            var rs = p.ToList()[0];
            var rule = db.hotel_rule.First(o => o.rulehotel != null);
            //Tim khuyen mai va gia ca
            var price = 0;
            int idroom = (int)rs.idroom;
            price = TotalPrice(rs.id, idhotel, idroom, (int)rs.numofroom, (int)rs.fromdate, (int)rs.todate, (int)rs.typebook);
            //int extrafee = UtilDb.TotalExtraFeeBasic(rs.id);
            
            var pi = (from qi in db.hotel_room where qi.deleted == 0 && qi.id == idroom select qi.breakfast).Take(1);
            string include = pi.ToList()[0];

            //var discount = db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= rs.fromdate).Where(o => o.tdate >= rs.fromdate).Max(o => o.discount);
            //if (discount == null) discount = 0;
            string invoice = UtilDb.getInvoiceDetail(rs.id);
            int totaldays = 0;
            if (rs.todate != null && rs.fromdate != null) totaldays = Config.getDateDiff(rs.fromdate.ToString(), rs.todate.ToString()); ;
            int totalprice = price;
            string body = "";
            string pathTemplate = HttpContext.Server.MapPath("/Scripts/tempEmailConfirmPayment.txt");
            StreamReader SR = new StreamReader(pathTemplate);
            body = SR.ReadToEnd();
            SR.Close();
            body = body.Replace("@paymentlink", Config.domain+"/HotelBooking/Payment?idhotel="+idhotel+"&code="+code);
            body = body.Replace("@domain", Config.domain);
            body = body.Replace("@ViewBag.code", code);            
            body = body.Replace("@ViewBag.datebook", rs.datebook.ToString());
            body = body.Replace("@ViewBag.cname", rs.cname);
            body = body.Replace("@ViewBag.hotelname", rs.hotelname);
            body = body.Replace("@ViewBag.address", rs2.address);
            body = body.Replace("@ViewBag.phone", rs2.phone);
            body = body.Replace("@ViewBag.fdate", Config.convertFromDateIdToDateString(rs.fromdate.ToString()));
            body = body.Replace("@ViewBag.tdate", Config.convertFromDateIdToDateString(rs.todate.ToString()));
            body = body.Replace("@ViewBag.hcheckin", rs2.hcheckin);
            body = body.Replace("@ViewBag.hcheckout", rs2.hcheckout);
            body = body.Replace("@totalnight", totaldays.ToString());
            body = body.Replace("@ViewBag.numofroom", rs.numofroom.ToString());
            body = body.Replace("@ViewBag.roomname", rs.roomname);
            body = body.Replace("@ViewBag.include", include);
            //body = body.Replace("@ViewBag.extrafee", extrafee.ToString()+"/ngày");
            body = body.Replace("@ViewBag.numofguest", rs.numofguest.ToString());
            body = body.Replace("@ViewBag.totalprice", Config.formatNumber(totalprice));           
            body = body.Replace("@ViewBag.invoice", invoice);
            body = body.Replace("@ViewBag.ruleroom", rule.rulehotel);
            body = body.Replace("@ViewBag.ruleextra", "");
            string toemail = rs.cemail;
            p = null;
            rs = null;
            p2 = null;
            rs2 = null;
            pi = null;
            return Config.mail(Config.emailcompany, toemail, Config.subjectEmailPayment, Config.passemailcompany, body);
           
        }
        public string updateBooking(string cname,string cemail,string cphone,string caddress,string cnote,string paymenttype,string code,int idhotel)//hotel_booking hb, 
        {
            try
            {
                string query = "update hotel_booking set cname=N'" + cname + "', cemail=N'" + cemail + "', cphone=N'" + cphone + "',caddress=N'" + caddress + "',cnote=N'" + cnote + "',status=N'" + Config.booking_status_step2 + "',paymenttype=N'" + paymenttype + "' where idhotel=" + idhotel + " and idsession=N'" + code + "'";
                db.Database.ExecuteSqlCommand(query);
                
            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public string updateBookingAdmin(string cname, string cemail, string cphone, string caddress, string cnote, string paymenttype,int? fromdate,int? todate,string code, int idhotel)//hotel_booking hb, 
        {
            try
            {
                string query = "update hotel_booking set cname=N'" + cname + "', cemail=N'" + cemail + "', cphone=N'" + cphone + "',caddress=N'" + caddress + "',cnote=N'" + cnote + "',status=N'" + Config.booking_status_step2 + "',paymenttype=N'" + paymenttype + "',fromdate="+fromdate+",todate="+todate+" where idhotel=" + idhotel + " and idsession=N'" + code + "'";
                db.Database.ExecuteSqlCommand(query);

            }
            catch (Exception ex)
            {
                return "0";
            }
            return "1";
        }
        public ActionResult Booking(string name,int fromdate,int todate,int idhotel,int idroom) {
            if (fromdate < Config.datetimeid()) {
                fromdate = Config.datetimeidaddday(1);
            }
            if (todate < Config.datetimeid())
            {
                todate = Config.datetimeidaddday(2);
            }
            hotel hotel = db.hotels.Find(idhotel);
            ViewBag.fdate = fromdate;
            ViewBag.tdate = todate;
            ViewBag.idhotel = idhotel;
            ViewBag.idroom = idroom;
            ViewBag.title = hotel.name;
            ViewBag.provin = hotel.provin;
            ViewBag.des = Config.smoothDes(hotel.des);
            ViewBag.image = Config.domain+hotel.image;
            ViewBag.invisibleprice = hotel.invisibleprice;
            ViewBag.rule = hotel.ruleroom + "<br>" + hotel.ruleextra;
            ViewBag.url = Config.domain+"/hotel/"+name + "-" + fromdate + "-" + todate + "-" + idhotel+"-"+idroom;
            var p = (from q in db.hotel_image where q.idhotel == idhotel select q).ToList();
            var content = "";
            for (int i = 0; i < p.Count; i++) {
                content += "<div class=\"item\"><a href=\"" + p[i].name + "\" data-rel=\"prettyPhoto[gallery1]\"><img src=\"" + Config.domain+"/"+p[i].name + "\" alt=\"" + p[i].caption + "\" class=\"img-responsive\" width=750 height=481 style=\"width=750px;height:481px;\"></a> </div>";
            }
            int? discount = db.hotel_promotion.Where(o => o.idhotel == idhotel).Where(o => o.fdate <= fromdate).Where(o => o.tdate >= fromdate).Max(o=>o.discount);
            if (discount != null && discount > 0)
            {
                ViewBag.discount = (int)discount;
            }
            else {
                ViewBag.discount = 0;
            }
            ViewBag.ImageList = content;
            if (Request.Browser.IsMobileDevice)
            {
                ViewBag.ismobile = 1;
            }
            else
            {
                ViewBag.ismobile = 0;
            }
            return View(hotel);
        }
        //
        // POST: /HotelBooking/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotel_booking hotel_booking)
        {
            if (ModelState.IsValid)
            {
                db.hotel_booking.Add(hotel_booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotel_booking);
        }

        //
        // GET: /HotelBooking/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_booking hotel_booking = db.hotel_booking.Find(id);
            if (hotel_booking == null)
            {
                return HttpNotFound();
            }
            return View(hotel_booking);
        }

        //
        // POST: /HotelBooking/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotel_booking hotel_booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel_booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotel_booking);
        }

        //
        // GET: /HotelBooking/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Admin");
            hotel_booking hotel_booking = db.hotel_booking.Find(id);
            if (hotel_booking == null)
            {
                return HttpNotFound();
            }
            return View(hotel_booking);
        }

        //
        // POST: /HotelBooking/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotel_booking hotel_booking = db.hotel_booking.Find(id);
            db.hotel_booking.Remove(hotel_booking);
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