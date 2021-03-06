//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal.Core.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public System.Guid Id { get; set; }
        public int Ranking { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> ImageCover { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Address { get; set; }
        public string AddressForMap { get; set; }
        public Nullable<System.Guid> City { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public Nullable<int> CountView { get; set; }
        public Nullable<System.Guid> TradeId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual Image Image { get; set; }
    }
}
