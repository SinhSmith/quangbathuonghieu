using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class ResumeService : BaseService
    {
        public static Resume GetResumeByUserId(Guid userId, params string[] includes)
        {
            using (var db = new JobEntities())
            {
                var query = db.Set<Resume>().AsQueryable();
                foreach (var include in includes)
                    query = query.Include(include);

                return query.FirstOrDefault(x => x.UserId == userId);
            }
        }

        public static bool CreateResume(Guid userId, string Email)
        {
            using (var db = new JobEntities())
            {
                Resume resume = db.Resumes.SingleOrDefault(x => x.UserId == userId);
                if (resume == null)
                {
                    resume = new Resume();
                    resume.UserId = userId;
                    resume.Email = Email;
                    resume.DateOfBirth = new DateTime(1970, 01, 01);
                    resume.Gender = true;
                    resume.MaritalStatus = false;
                    resume.Nationality = false;
                    resume.Country = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    resume.YearOfExperience = 0;
                    resume.HighestEducation = Guid.Parse("00000000-0000-0000-0000-000000000303");
                    resume.ExpectedSalary = 0;
                    db.Resumes.Add(resume);
                    db.SaveChanges();                    
                }
                return true;
            }
        }
    }
}