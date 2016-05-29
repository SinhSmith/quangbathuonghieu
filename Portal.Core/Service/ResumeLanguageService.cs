using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class ResumeLanguageService : BaseService
    {
        public static List<ResumeLanguage> GetResumeLanguagesByResumeId(int resumeId)
        {
            using (var db = new JobEntities())
            {
                var resume = db.ResumeLanguages.Where(x => x.ResumeId == resumeId).OrderBy(x => x.LanguageId).ToList();
                return resume;
            }
        }

        public static void DeleteResumeLanguagesByResumeId(int resumeId)
        {
            using (var db = new JobEntities())
            {
                db.ResumeLanguages.RemoveRange(db.ResumeLanguages.Where(x => x.ResumeId == resumeId));
                db.SaveChanges();
            }
        }

        public static void DeleteResumeLanguageById(int Id)
        {
            using (var db = new JobEntities())
            {
                db.ResumeLanguages.Remove(db.ResumeLanguages.FirstOrDefault(x => x.Id == Id));
                db.SaveChanges();
            }
        }
    }
}