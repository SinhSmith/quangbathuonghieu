using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class CategoryService : BaseService
    {
        public static readonly List<object> Status = new List<object>() { new { Id = 0, Name = "Ngừng hoạt động" }, new { Id = 1, Name = "Hoạt động" } };
        public static Category GetCategoryById(Guid Id)
        {
            using (var db = new PortalEntities())
            {
                var category = db.Categories.Find(Id);
                return category;
            }
        }

        public static List<Category> GetCities()
        {
            using (var db = new PortalEntities())
            {
                List<Category> categories = db.Categories.Where(x => x.Type == (int)CategoryType.City && x.Status == (int)Util.Define.Status.Active).OrderBy(x => x.SortOrder).ToList();
                return categories;
            }
        }

        public static List<Category> GetDistrict(Guid cityId)
        {
            using (var db = new PortalEntities())
            {
                List<Category> categories = db.Categories.Where(x => x.Type == (int)CategoryType.District && x.ParentId == cityId && x.Status == (int)Util.Define.Status.Active).OrderBy(x => x.SortOrder).ToList();
                return categories;
            }
        }
    }
}