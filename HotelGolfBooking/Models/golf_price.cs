//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelGolfBooking.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class golf_price
    {
        public int id { get; set; }
        public int idgolf { get; set; }
        public string golfname { get; set; }
        public Nullable<int> price { get; set; }
        public Nullable<int> priceweekend { get; set; }
        public Nullable<int> pricebuggy { get; set; }
        public string month { get; set; }
        public Nullable<int> deleted { get; set; }
    }
}
