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
    
    public partial class Banner
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> ImageId { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> Status { get; set; }
    
        public virtual Image Image { get; set; }
    }
}
