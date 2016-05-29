using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.CMS.Models
{
    public class SubmitApplicationViewModel
    {
        public int Id { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<bool> Gender { get; set; }
        public Nullable<bool> MaritalStatus { get; set; }
        public Nullable<bool> Nationality { get; set; }
        public string Address { get; set; }
        public Nullable<System.Guid> Country { get; set; }
        public Nullable<System.Guid> City { get; set; }
        public Nullable<System.Guid> District { get; set; }
        public string Phone { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public bool Applied { get; set; }  
    }

   
}