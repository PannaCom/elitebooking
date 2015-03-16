using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HotelGolfBooking
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "detail",
                "detail/{url}-{id}",
                new { controller = "News", action = "Details", url = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "newslist",
                "news/{keyword}-{page}",
                new { controller = "News", action = "List", keyword = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
                "catnews",
                "category/{keyword}-{catname}-{catid}-{page}",
                new { controller = "News", action = "Cat", keyword = UrlParameter.Optional, catname = UrlParameter.Optional, catid = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                "search hotel",
                "{fromdate}-{todate}-{name}-{rate}-{dis}-page{page}",
                new { controller = "Search", action = "SearchHotel", fromdate = UrlParameter.Optional, todate = UrlParameter.Optional, name = UrlParameter.Optional, rate = UrlParameter.Optional, dis = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
                "good price",
                "GoodPrice/{fromdate}-{todate}-{name}-{rate}-{provin}-page{page}",
                new { controller = "Search", action = "GoodPrice", fromdate = UrlParameter.Optional, todate = UrlParameter.Optional, name = UrlParameter.Optional, rate = UrlParameter.Optional, provin = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
               "hotel booking",
               "hotel/{name}-{fromdate}-{todate}-{idhotel}-{idroom}",
               new { controller = "HotelBooking", action = "Booking", name = UrlParameter.Optional, fromdate = UrlParameter.Optional, todate = UrlParameter.Optional, idhotel = UrlParameter.Optional, idroom = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}