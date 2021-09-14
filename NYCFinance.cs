using HtmlAgilityPack;
using HtmlParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebScraperLibrary;
using NYCFinance.Model;
using NYCFinance.SupportClasses;

namespace NYCFinance
{
    class NYCFinance : MainScrapper, IDisposable
    {
        private readonly HelperClass _helperClass;

        #region Constructor
        public NYCFinance()
        {
            _helperClass = new HelperClass();
        }
        #endregion

        #region FETCH_PROPTERTY_DETAILS
        /// <summary>
        /// To fetch proptery details as per only street.
        /// </summary>
        /// <param name="street"></param>
        public void NYCFinanceForFetchPropertyDetails(string street)
        {
            Console.WriteLine("Welcome NYC");
            Console.WriteLine("==============================START PROCESS==================================");
            userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36";
            simpleContentType = "application/x-www-form-urlencoded";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            var html = "";
            List<PropertyDetails> propertyDetails = null;
            Random random = new Random();
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                html = DoSimpleGetRequest("https://a836-pts-access.nyc.gov/care/forms/htmlframe.aspx?mode=content/home.htm");
                html = DoSimpleGetRequest("https://a836-pts-access.nyc.gov/care/search/commonsearch.aspx?mode=address");
                Thread.Sleep(random.Next(1500, 4000));

                Parser p;
                HtmlNode form;
                HtmlForm form1;

                if (lastResult.Contains("id=\"btAgree\""))
                {

                    // This is  first page (AGREE PAGE)
                    p = new Parser(LastResult);
                    form = p.GetElement("form", Attributes.Xpath, "//form[@method='post']");
                    var formUrl = form.Attributes.FirstOrDefault(x => x.Name.Equals("action"))?.Value ?? "Disclaimer.aspx?FromUrl=..%2fsearch%2fcommonsearch.aspx%3fmode%3daddress";
                    formUrl = formUrl.Replace("./Disclaimer", "/Disclaimer");
                    formUrl = $"https://a836-pts-access.nyc.gov/care/Search{formUrl}";
                    form1 = Parser.ParseForm(form);
                    contentValues = form1.Fields;
                    contentValues["btAgree"] = "";
                    PostForm(formUrl);
                }

                Thread.Sleep(random.Next(2000, 5000));
                p = new Parser(LastResult);
                form = p.GetElement("form", Attributes.Xpath, "//form[@method='post']");
                form1 = Parser.ParseForm(form);
                contentValues = form1.Fields;

                int page_num = 1;
                string hdAction = "Search";

                SEARCH_RESULTS:
                contentValues["ScriptManager1_TSM"] = ";;AjaxControlToolkit, Version=4.1.50731.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e:en-US:f8fb2a65-e23a-483b-b20e-6db6ef539a22:ea597d4b:b25378d2;Telerik.Web.UI, Version=2020.2.512.45, Culture=neutral, PublicKeyToken=121fae78165ba3d4:en-US:88f9a2dc-9cbf-434f-a243-cf2dd9f642dc:16e4e7cd:f7645509:24ee1bba:c128760b:19620875:874f8ea2:f46195d3:b2e06756:92fe8ea0:fa31b949:4877f69a:33715776:490a9d4e";
                contentValues["PageNum"] = page_num.ToString();
                contentValues["SortBy"] = "PARID";
                contentValues["SortDir"] = "asc";
                contentValues["PageSize"] = "15";
                contentValues["hdAction"] = hdAction;
                contentValues["hdIndex"] = page_num.ToString();
                contentValues["sIndex"] = "-1";
                contentValues["hdListType"] = "PA";
                contentValues["hdJur"] = "";
                contentValues["hdSelectAllChecked"] = false;
                contentValues["inpUnit"] = "";
                contentValues["inpNumber"] = "";
                //contentValues["inpStreet"] = "iowa";
                contentValues["inpStreet"] = street;
                contentValues["inpSuffix2"] = "";
                contentValues["selSortBy"] = "PARID";
                contentValues["selSortDir"] = "asc";
                contentValues["selPageSize"] = "15";
                contentValues["searchOptions$hdBeta"] = "";
                contentValues["btSearch"] = "";
                contentValues["RadWindow_NavigateUrl_ClientState"] = "";
                contentValues["mod"] = "ADDRESS";
                contentValues["mask"] = "";
                contentValues["param1"] = "";
                contentValues["searchimmediate"] = "";

                Thread.Sleep(random.Next(1000, 4000));
                PostForm("https://a836-pts-access.nyc.gov/care/search/CommonSearch.aspx?mode=ADDRESS");
                html = LastResult;

                if (html.Contains("id=\"searchResults\""))
                {
                    if (propertyDetails == null || propertyDetails?.Count == 0)
                        propertyDetails = _helperClass.ParsePropertyDetails(html);
                    else
                    {
                        var items = _helperClass.ParsePropertyDetails(html);
                        if (items?.Count > 0)
                        {
                            foreach (var item in items)
                                propertyDetails.Add(item);
                        }
                    }

                    if (html.Contains("Next >>"))
                    {
                        Thread.Sleep(random.Next(4000, 8000));
                        ++page_num;
                        hdAction = "NewPage";
                        goto SEARCH_RESULTS;
                    }
                    else if (propertyDetails?.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(propertyDetails);
                        Console.WriteLine(json);
                    }
                }
                else if (html.Contains(SupportClass.ResultNotFoundMessage))
                {
                    Console.WriteLine(SupportClass.ResultNotFoundMessage);
                }
                else
                {
                    Console.WriteLine("Something went wrong !");
                }
            }
            catch (Exception e)
            {
                // TODO - Write LOG here...
                Console.WriteLine("Something went wrong !");
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine("==============================END PROCESS==================================");
        }
        #endregion

        /// <summary>
        /// To Fetch Property information.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="street"></param>
        public void NYCFinanceForFetchPropertyInfo(string number, string street)
        {
            Console.WriteLine("Welcome NYC");
            Console.WriteLine("==============================START PROCESS==================================");
            userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36";
            simpleContentType = "application/x-www-form-urlencoded";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            var html = "";
            Random random = new Random();
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                html = DoSimpleGetRequest("https://a836-pts-access.nyc.gov/care/forms/htmlframe.aspx?mode=content/home.htm");
                html = DoSimpleGetRequest("https://a836-pts-access.nyc.gov/care/search/commonsearch.aspx?mode=address");
                Thread.Sleep(random.Next(1500, 4000));

                Parser p;
                HtmlNode form;
                HtmlForm form1;

                if (lastResult.Contains("id=\"btAgree\""))
                {
                    // This is  first page (AGREE PAGE)
                    p = new Parser(LastResult);
                    form = p.GetElement("form", Attributes.Xpath, "//form[@method='post']");
                    var formUrl = form.Attributes.FirstOrDefault(x => x.Name.Equals("action"))?.Value ?? "Disclaimer.aspx?FromUrl=..%2fsearch%2fcommonsearch.aspx%3fmode%3daddress";
                    formUrl = formUrl.Replace("./Disclaimer", "/Disclaimer");
                    formUrl = $"https://a836-pts-access.nyc.gov/care/Search{formUrl}";
                    form1 = Parser.ParseForm(form);
                    contentValues = form1.Fields;
                    contentValues["btAgree"] = "";
                    PostForm(formUrl);
                }

                Thread.Sleep(random.Next(2000, 5000));
                p = new Parser(LastResult);
                form = p.GetElement("form", Attributes.Xpath, "//form[@method='post']");
                form1 = Parser.ParseForm(form);
                contentValues = form1.Fields;

                int page_num = 1;
                string hdAction = "Search";

                contentValues["ScriptManager1_TSM"] = ";;AjaxControlToolkit, Version=4.1.50731.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e:en-US:f8fb2a65-e23a-483b-b20e-6db6ef539a22:ea597d4b:b25378d2;Telerik.Web.UI, Version=2020.2.512.45, Culture=neutral, PublicKeyToken=121fae78165ba3d4:en-US:88f9a2dc-9cbf-434f-a243-cf2dd9f642dc:16e4e7cd:f7645509:24ee1bba:c128760b:19620875:874f8ea2:f46195d3:b2e06756:92fe8ea0:fa31b949:4877f69a:33715776:490a9d4e";
                contentValues["PageNum"] = page_num.ToString();
                contentValues["SortBy"] = "PARID";
                contentValues["SortDir"] = "asc";
                contentValues["PageSize"] = "15";
                contentValues["hdAction"] = hdAction;
                contentValues["hdIndex"] = page_num.ToString();
                contentValues["sIndex"] = "-1";
                contentValues["hdListType"] = "PA";
                contentValues["hdJur"] = "";
                contentValues["hdSelectAllChecked"] = false;
                contentValues["inpUnit"] = "";
                //contentValues["inpNumber"] = "86-72";
                //contentValues["inpStreet"] = "sancho Street";
                contentValues["inpNumber"] = number;
                contentValues["inpStreet"] = street;
                contentValues["inpSuffix2"] = "";
                contentValues["selSortBy"] = "PARID";
                contentValues["selSortDir"] = "asc";
                contentValues["selPageSize"] = "15";
                contentValues["searchOptions$hdBeta"] = "";
                contentValues["btSearch"] = "";
                contentValues["RadWindow_NavigateUrl_ClientState"] = "";
                contentValues["mod"] = "ADDRESS";
                contentValues["mask"] = "";
                contentValues["param1"] = "";
                contentValues["searchimmediate"] = "";

                Thread.Sleep(random.Next(1000, 4000));
                PostForm("https://a836-pts-access.nyc.gov/care/search/CommonSearch.aspx?mode=ADDRESS");
                html = LastResult;

                if (html.Contains("Property Owner(s)"))
                {
                    var propertyInfo = _helperClass.ParsePropertyInfo(html);

                    var propetyNotices = _helperClass.ParsePropertyNotices(html);

                    _helperClass.ParsePrintSummary(html);
                }
                else if (html.Contains(SupportClass.ResultNotFoundMessage))
                {
                    Console.WriteLine(SupportClass.ResultNotFoundMessage);
                }
                else
                {
                    Console.WriteLine("Something went wrong !");
                }
            }
            catch (Exception e)
            {
                // TODO - Write LOG here....
                Console.WriteLine("Something went wrong !");
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine("==============================END PROCESS==================================");
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
