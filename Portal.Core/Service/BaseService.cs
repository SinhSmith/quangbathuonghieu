using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Portal.Core.Service
{
    public class BaseService
    {
        public enum CategoryType
        {
            [Description("Tỉnh thành")]
            City = 1,
            [Description("Ngành nghề")]
            Trades = 2,
            [Description("Quận huyện")]
            District = 3
        }
    }

    /// <summary>
    /// Class EnumHelper.
    /// </summary>
    public static class EnumHelper
    {
        public static string GetDescriptionFromEnum(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
}