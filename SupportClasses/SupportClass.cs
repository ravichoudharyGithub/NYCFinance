using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCFinance.SupportClasses
{
    public static class SupportClass
    {
        public const string ResultNotFoundMessage = "Your search did not find any records.";

        public static string GetHtmlNodeAttributeValue(HtmlNode htmlNode)
        {
            if (htmlNode == null)
                return null;

            return htmlNode.Attributes.FirstOrDefault(x => x.Name.Equals("value"))?.Value ?? null;
        }
    }
}
