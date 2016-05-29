using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.CMS.Models
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public Nullable<System.Guid> CompanyByAdministrator { get; set; }
        public Nullable<System.Guid> CompanyId { get; set; }
        public string CompanyString { get; set; }
        public string JobTitle { get; set; }
        public Nullable<System.Guid> JobLevel { get; set; }
        public string JobLevelString { get; set; }
        public Nullable<int> SalaryRangeFrom { get; set; }
        public Nullable<int> SalaryRangeTo { get; set; }
        [AllowHtml]
        public string JobDescription { get; set; }
        [AllowHtml]
        public string JobRequirement { get; set; }
        [AllowHtml]
        public string JobSkill { get; set; }
        [AllowHtml]
        public string JobBenefit { get; set; }
        public string JobCategorieString { get; set; }
        public string JobWorkPlaceString { get; set; }
        public string JobTag1 { get; set; }
        public string JobTag2 { get; set; }
        public string JobTag3 { get; set; }
        public string ContactPerson { get; set; }
        public string EmailForApplications { get; set; }
        public string PreferredLanguageForApplications { get; set; }
        public Nullable<System.DateTime> TimeCreate { get; set; }
        public Nullable<System.DateTime> TimeExpired { get; set; }
        public Nullable<int> Status { get; set; }
    }
}