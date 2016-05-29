using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class JobApplyService : BaseService
    {
        public static bool Insert(JobApply _jobApply)
        {
            using (var db = new JobEntities())
            {
                JobApply jobApply = db.JobApplies.SingleOrDefault(x => x.JobId == _jobApply.JobId && x.UserId == _jobApply.UserId);
                if (jobApply == null)
                {
                    db.JobApplies.Add(_jobApply);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
        }

        public static JobApply Get(int jobId, Guid userId)
        {
            using (var db = new JobEntities())
            {
                return db.JobApplies.SingleOrDefault(x => x.JobId == jobId && x.UserId == userId);
            }
        }  
    }
}