using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Portal.Core.Util
{
    public class Common
    {
        public static string RemoveUnicode(string inputText)
        {
            inputText = Cut(inputText);
            string stFormD = inputText.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string str = "";
            for (int i = 0; i <= stFormD.Length - 1; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc == UnicodeCategory.NonSpacingMark == false)
                {
                    if (stFormD[i] == 'đ')
                        str = "d";
                    else if (stFormD[i] == 'Đ')
                        str = "D";
                    else
                        str = stFormD[i].ToString();
                    sb.Append(str);
                }
            }
            return sb.ToString().Replace(" ", "-");
        }

        public static string Cut(string title)
        {
            title = title.Trim();
            title = title.Replace(" ", "-");
            title = title.Replace("(", "-");
            title = title.Replace(")", "-");
            title = title.Replace("[", "-");
            title = title.Replace("]", "-");
            title = title.Replace("#", "-");
            title = title.Replace("\"", "-");
            title = title.Replace("\'", "-");
            title = title.Replace("<", "-");
            title = title.Replace(">", "-");
            title = title.Replace("*", "-");
            title = title.Replace("%", "-");
            title = title.Replace("&", "-");
            title = title.Replace(":", "-");
            title = title.Replace("\\", "-");
            title = title.Replace("?", "-");
            title = title.Replace("/", "-");
            title = title.Replace("_", "-");
            title = title.Replace(",", "-");
            title = title.Replace(".", "-");
            title = title.ToLower();

            title = title.Replace("------", "-");
            title = title.Replace("-----", "-");
            title = title.Replace("----", "-");
            title = title.Replace("---", "-");
            title = title.Replace("--", "-");
            if (title.Length > 0 && title[0] == '-')
                title = title.Substring(1);
            return title;
        }

        public static string StripHTML(string HTMLText)
        {
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            HTMLText = reg.Replace(HTMLText, "");
            return Regex.Replace(HTMLText, @"\t|\n|\r", "");
        }

        public static string DescriptionTrip(string HTMLText)
        {
            HTMLText = HTMLText.Length > 500 ? HTMLText.Substring(0, 500) : HTMLText;
            HTMLText = HTMLText.Substring(0, HTMLText.LastIndexOf(" ") - 1);
            HTMLText = HTMLText.Replace("•", "-");
            return HTMLText;
        }
    }
}