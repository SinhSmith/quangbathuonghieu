using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;

namespace Portal.Core.Service
{
    public class TrackingService : BaseService
    {
        public static int GetTotalVisitors()
        {
            using (var db = new PortalEntities())
            {
                var tracking = db.Trackings.FirstOrDefault(x => x.Url == Portal.Core.Util.Define.RootUrl);
                return tracking.TotalVisitors.Value;
            }
        }

        public static bool UpdateTotalVisitors()
        {
            using (var db = new PortalEntities())
            {
                var tracking = db.Trackings.FirstOrDefault(x => x.Url == Portal.Core.Util.Define.RootUrl);
                tracking.TotalVisitors++;
                db.SaveChanges();
                
                return true;
            }
        }
    }
}