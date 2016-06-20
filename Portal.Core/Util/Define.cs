using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Core.Util
{
    public class Define
    {
        public static string RootUrl = "http://uytinviet.com.vn/";
        public static string DefaultImageUrl = "/Images/no-logo.png";
        public enum UserRole
        {
            NotSet = 0,
            Candidates = 1,
            Employer = 2,
            Administrator = 3

        }

        public enum Status
        {
            Delete = 0,
            Active = 1,
            Pending = 2
        }
    }
}