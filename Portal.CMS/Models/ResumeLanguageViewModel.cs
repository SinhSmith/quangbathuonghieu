using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.CMS.Models
{
    public class ResumeLanguageViewModel
    {
        public System.Guid Id { get; set; }
        public Nullable<int> ResumeId { get; set; }
        public Nullable<System.Guid> LanguageId { get; set; }
        public Nullable<int> LanguageProficiency { get; set; }
    }
}