using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class CompanyService : BaseService
    {
        public static Guid CreateCompany(Guid userId)
        {
            using (var db = new JobEntities())
            {
                Company company = db.Companies.SingleOrDefault(x => x.UserId == userId);
                if (company == null)
                {
                    company = new Company();
                    company.Id = Guid.NewGuid();
                    company.UserId = userId;
                    db.Companies.Add(company);
                    db.SaveChanges();
                }
                return company.Id;
            }
        }

        public static Company GetCompany(Guid companyId)
        {
            using (var db = new JobEntities())
            {
                var company = db.Companies.Find(companyId);
                return company;
            }
        }

        public static Company GetCompanyByUserId(Guid userId)
        {
            using (var db = new JobEntities())
            {
                var company = db.Companies.FirstOrDefault(x => x.UserId == userId);
                return company;
            }
        }

        public static List<Company> GetAllCompany()
        {
            using (var db = new JobEntities())
            {
                var company = db.Companies.ToList();
                return company;
            }
        }
    }
}