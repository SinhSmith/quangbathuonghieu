using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Core.Service
{
    public class BaseService
    {
        public enum CategoryType
        {
            Country = 1,
            City = 2,
            EducationLevel = 3,
            JobLevel = 4,
            JobCategory = 5,
            District = 6,
            Language = 7,
            NumberOfEmployees = 8
        }
    }
}