using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelGolfBooking.Views
{

        public class feedback { 
            public int id {get;set;}
            public string cname{get;set;}
            public string caddress{get;set;}
            public string fullcontent{get;set;}
            public string hotelname{get;set;}
            public string hotelimage{get;set;}
            public int? idhotel{get;set;}
            public string golfname{get;set;}
            public string golfimage{get;set;}
            public int? idgolf { get; set; }
        }
        public class viewHotelSearchManager
        {
            public int id { get; set; }
            public string name { get; set; }
            public string provin { get; set; }
            public string image { get; set; }
            public string address { get; set; }
            public int rate { get; set; }
            public int minprice { get; set; }
            public byte invisibleprice { get; set; }   
        }
        public class ModelClassViewHotelSearchManager
        {
            public IEnumerable<viewHotelSearchManager> ieViewNews { get; set; }
        }
   
}