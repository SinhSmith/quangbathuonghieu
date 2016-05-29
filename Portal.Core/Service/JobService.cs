using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class JobService : BaseService
    {
        public static Job GetJobByJobId(int id, params string[] includes)
        {
            using (var db = new JobEntities())
            {
                var query = db.Set<Job>().AsQueryable();
                foreach (var include in includes)
                    query = query.Include(include);

                return query.FirstOrDefault(x => x.Id == id);
            }
        }

        public static List<Job> GetAllJobs()
        {
            using (var db = new JobEntities())
            {
                var jobs = db.Jobs.ToList();
                return jobs;
            }
        }

        public static List<Job> GetJobsByCompanyId(Guid companyId)
        {
            using (var db = new JobEntities())
            {
                var jobs = db.Jobs.Where(x => x.CompanyId == companyId).ToList();
                return jobs;
            }
        }
    }
}