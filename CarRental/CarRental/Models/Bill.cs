//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarRental.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bill
    {
        public int id { get; set; }
        public string vuid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string paymentMethod { get; set; }
        public Nullable<int> totalAmount { get; set; }
        public string productDetails { get; set; }
        public string createdBy { get; set; }
    }
}
