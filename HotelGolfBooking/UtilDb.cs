using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelGolfBooking.Models;

namespace HotelGolfBooking
{
    
    public class UtilDb
    {
        private static hotelbookingEntities db = new hotelbookingEntities();
        public static int getTotalPrice(int idhotel,int idroom,int fromdate,int? typebook){
            int price=0;
            if (typebook == 0)
            {
                string month = Config.getMonthFromDateId(fromdate);
                try
                {
                    price = (int)db.hotel_room_price.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.idroom == idroom).Where(o => o.month.Contains("," + month + ",")).Min(o => o.price);
                    //var p = (from q in db.hotel_room_price where q.deleted == 0 && q.idhotel == idhotel && q.idroom == idroom && q.month.Contains("," + month + ",") orderby q.price select q.price).Take(1);
                    //price = (int)p.ToList()[0];
                    //p = null;
                }
                catch (Exception ex) {
                    
                    return 0;
                }
            }
            else
            {
                try
                {
                    price = (int)db.hotel_choupon.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.idroom == idroom).Min(o => o.chouponprice);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return price;
        }
        public static int getGolfTotalPrice(int idgolf, int dateplay, int? typebook)
        {
            int price = 0;
            if (typebook == 0)
            {
                string month = Config.getMonthFromDateId(dateplay);
                try
                {
                    if (!Config.isWeekendDate(dateplay))
                    {
                        price = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains("," + month + ",")).Min(o => o.price);
                    }
                    else {
                        price = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains("," + month + ",")).Min(o => o.priceweekend);
                    }
                    //var p = (from q in db.hotel_room_price where q.deleted == 0 && q.idhotel == idhotel && q.idroom == idroom && q.month.Contains("," + month + ",") orderby q.price select q.price).Take(1);
                    //price = (int)p.ToList()[0];
                    //p = null;
                }
                catch (Exception ex)
                {

                    return 0;
                }
            }
            else
            {
                try
                {
                    price = (int)db.golf_choupon.Where(o => o.deleted==0).Where(o => o.idgolf == idgolf).Min(o => o.chouponprice);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return price;
        }
        public static int getPriceBuggies(int idgolf,int dateplay)
        {
           
            int price = 0;
            try
            {
                string month = Config.getMonthFromDateId(dateplay);
                
                price = (int)db.golf_price.Where(o => o.deleted == 0).Where(o => o.idgolf == idgolf).Where(o => o.month.Contains("," + month + ",")).Min(o => o.pricebuggy);
               
                //var p = (from q in db.hotel_room_price where q.deleted == 0 && q.idhotel == idhotel && q.idroom == idroom && q.month.Contains("," + month + ",") orderby q.price select q.price).Take(1);
                //price = (int)p.ToList()[0];
                //p = null;
                return price;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }
        public static string getInvoiceDetail(int idbooking) {
            var p = (from q in db.hotel_booking_invoice where q.idbooking == idbooking select q).OrderBy(o => o.fromdate).ThenBy(o => o.todate).ToList();
            string content = "";
            content+="<div id=\"listInvoice\" name=\"listInvoice\" style=\"z-index:1001;position:relative;width:100%;height:auto;display:block;border:1px solid;\">";
            content += "			   <table style=\"width: 100%;border: 1px solid #ebebeb;border-radius: 5px 5px 5px 5px;\" id=\"tblListInvoice\" class=\"tblListInvoice\">";
			content+="				   <tr><th width=\"73\">Từ ngày</th><th width=\"73\">Đến ngày</th><th width=\"73\">Giá</th><th width=\"73\">Số phòng</th><th width=\"73\">Số ngày</th><th width=\"73\">Thành tiền</th><th width=\"73\">Giường phụ</th><th width=\"73\">Phí khác</th><th width=\"73\">Tổng tiền</th></tr>";
			for(int i=0;i<p.Count;i++){
                content+="<tr>";
			    content+="<td width=\"73\">"+Config.convertFromDateIdToDateString(p[i].fromdate)+"</td>";
                content+="<td width=\"73\">"+Config.convertFromDateIdToDateString(p[i].todate)+"</td>";
                content+="<td width=\"73\">"+Config.formatNumber((int)p[i].price)+"</td>";
			    content+="<td width=\"73\" align=center>"+p[i].numofroom+"</td>";
                content+= "<td width=\"73\" align=center>" + p[i].totalday + "</td>";					  
			    content+="<td width=\"73\">"+Config.formatNumber((int)p[i].total)+"</td>";	
			    content+="<td width=\"73\">"+Config.formatNumber((int)p[i].totalextrabed)+"</td>";	
	            content+="<td width=\"73\">"+Config.formatNumber((int)p[i].totalextraother)+"</td>";
		        content+="<td width=\"73\">"+Config.formatNumber((int)p[i].totalprice)+"</td>";	
                 content+="</tr>";
            }
            content += "</table>";
            content += "</div>";
            return content;
        }
        public static string getInvoiceDetailGolf(int idbooking)
        {
            var p = (from q in db.golf_booking_invoice where q.idbooking == idbooking select q).OrderBy(o => o.price).ToList();
            string content = "";
            content += "<div id=\"listInvoice\" name=\"listInvoice\" style=\"z-index:1001;position:relative;width:100%;height:auto;display:block;border:1px solid;\">";
            content += "			   <table style=\"width: 100%;border: 1px solid #ebebeb;border-radius: 5px 5px 5px 5px;\" id=\"tblListInvoice\" class=\"tblListInvoice\">";
            content += "				   <tr style=\"border-bottom:solid 1px #808080;\"><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Giá</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Giá xe Buggy</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Phụ thu hố</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Phụ thu xe buggies</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Số khách</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Số xe buggies</th><th width=\"73\" style=\"border-right:1px solid #00ff90;border-bottom:solid 1px #808080;\">Tổng tiền</th><th width=\"30%\" style=\"border-bottom:solid 1px #808080;\">Ghi chú</th></tr>";//<th width=\"73\">Ngày chơi</th>
            string noteextrafee = "";
            for (int i = 0; i < p.Count; i++)
            {
                content += "<tr style=\"border-bottom:solid 1px #808080;\">";
                //content += "<td width=\"73\">" + Config.convertFromDateIdToDateString(p[i].playdate) + "</td>";
                content += "<td width=\"73\" style=\"border-right:1px solid #00ff90;\">" + Config.formatNumber((int)p[i].price) + "</td>";
                content += "<td width=\"73\" style=\"border-right:1px solid #00ff90;\">" + Config.formatNumber((int)p[i].pricebuggies) + "</td>";
                content += "<td width=\"73\" style=\"border-right:1px solid #00ff90;\">" + Config.formatNumber((int)p[i].priceextralhole) + "</td>";
                content += "<td width=\"73\" style=\"border-right:1px solid #00ff90;\">" + Config.formatNumber((int)p[i].priceextralbuggies) + "</td>";
                content += "<td width=\"73\" align=center style=\"border-right:1px solid #00ff90;\">" + p[i].numofguest + "</td>";
                content += "<td width=\"73\" align=center style=\"border-right:1px solid #00ff90;\">" + p[i].numofbuggies + "</td>";
                content += "<td width=\"73\" style=\"border-right:1px solid #00ff90;\">" + Config.formatNumber((int)p[i].totalprice) + "</td>";
                if (p[i].noteextrafee != null)
                {
                    content += "<td width=\"73\">" + p[i].noteextrafee + "</td>";
                }
                else {
                    content += "<td width=\"73\"></td>";
                }
                content += "</tr>";
            }
            content += "</table>";
            content += "</div>";
            return content;
        }
        public static int TotalExtraFeeBasic(int idbooking)
        {
            try
            {
                var p = (from q in db.hotel_booking where q.id == idbooking orderby q.id select q).Take(1);
                var rs = p.ToList();
                byte extrabedfee = (byte)db.hotel_booking.Where(o => o.id == idbooking).Min(o => o.extrabedfee);
                byte extraotherfee = (byte)db.hotel_booking.Where(o => o.id == idbooking).Min(o => o.extraotherfee);
                int idhotel = (int)rs[0].idhotel;
                int idroom = (int)rs[0].idroom;
                int extrafee = 0;
                int tempfee = 0;
                if (extrabedfee == 1)
                {
                    tempfee = (int)db.hotel_room.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.id == idroom).Min(o => o.extrabedfee);
                }
                extrafee += tempfee;
                tempfee = 0;
                if (extraotherfee == 1)
                {
                    tempfee = (int)db.hotel_room.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.id == idroom).Min(o => o.extraotherfee);
                }
                extrafee += tempfee;
                p = null;
                rs = null;
                return extrafee;
            }
            catch (Exception ex) {
                return 0;
            }
        }
        public static int TotalExtraBedFeeBasic(int idbooking)
        {
            try
            {
                var p = (from q in db.hotel_booking where q.id == idbooking orderby q.id select q).Take(1);
                var rs = p.ToList();
                byte extrabedfee = (byte)db.hotel_booking.Where(o => o.id == idbooking).Min(o => o.extrabedfee);                
                int idhotel = (int)rs[0].idhotel;
                int idroom = (int)rs[0].idroom;
                int extrafee = 0;
                int tempfee = 0;
                if (extrabedfee == 1)
                {
                    tempfee = (int)db.hotel_room.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.id == idroom).Min(o => o.extrabedfee);
                }
                extrafee += tempfee;               
                p = null;
                rs = null;
                return extrafee;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static int TotalExtraOtherFeeBasic(int idbooking)
        {
            try
            {
                var p = (from q in db.hotel_booking where q.id == idbooking orderby q.id select q).Take(1);
                var rs = p.ToList();
             
                byte extraotherfee = (byte)db.hotel_booking.Where(o => o.id == idbooking).Min(o => o.extraotherfee);
                int idhotel = (int)rs[0].idhotel;
                int idroom = (int)rs[0].idroom;
                int extrafee = 0;
                int tempfee = 0;              
                if (extraotherfee == 1)
                {
                    tempfee = (int)db.hotel_room.Where(o => o.deleted == 0).Where(o => o.idhotel == idhotel).Where(o => o.id == idroom).Min(o => o.extraotherfee);
                }
                extrafee += tempfee;
                p = null;
                rs = null;
                return extrafee;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}