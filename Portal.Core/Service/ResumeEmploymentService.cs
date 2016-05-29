using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class ResumeEmploymentService : BaseService
    {
        public static ResumeEmployment GetResumeEmploymentById(int Id)
        {
            using (var db = new JobEntities())
            {
                var employment = db.ResumeEmployments.Find(Id);
                return employment;
            }
        }
    }
}