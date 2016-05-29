using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Core.Database;
using Portal.Core.Util;

namespace Portal.Core.Service
{
    public class ProfileService
    {
        public static void UpdateProfile(Profile _profile)
        {
            using (var db = new JobEntities())
            {
                Profile profile = db.Profiles.SingleOrDefault(x => x.UserId == _profile.UserId);
                if (profile == null)
                {
                    profile = new Profile();
                    profile.UserId = _profile.UserId;
                    profile.Role = _profile.Role;
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                }
                else
                {
                    profile.Role = _profile.Role;
                    db.SaveChanges();
                }               
                
            }
        }


        public static Profile GetProfile(string _userId)
        {
            using (var db = new JobEntities())
            {
                if (string.IsNullOrEmpty(_userId))
                    return null;
                else
                {
                    Guid userId = Guid.Parse(_userId);
                    Profile profile = db.Profiles.SingleOrDefault(x => x.UserId == userId);
                    if (profile == null)
                    {
                        db.Profiles.Add(new Profile() { UserId = userId, Role = (int)Define.UserRole.NotSet });
                        db.SaveChanges();
                    }
                    return profile;
                }
            }
        }
    }
}