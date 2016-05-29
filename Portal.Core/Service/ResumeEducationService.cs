using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class ResumeEducationService : BaseService
    {
        public static ResumeEducation GetResumeEducationById(int Id)
        {
            using (var db = new JobEntities())
            {
                var education = db.ResumeEducations.Find(Id);
                return education;
            }
        }
    }
}