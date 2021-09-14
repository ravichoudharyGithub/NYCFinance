using HtmlAgilityPack;
using NYCFinance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraperLibrary;

namespace NYCFinance.SupportClasses
{
    public class HelperClass : MainScrapper
    {


        /// <summary>
        /// To parse property details
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public List<PropertyDetails> ParsePropertyDetails(string html)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var listOfResultClass = new List<PropertyDetails>();

                var searchResults = htmlDoc.DocumentNode.SelectNodes("//table[@id='searchResults']");
                if (searchResults != null && searchResults.Count == 1)
                {
                    var items = searchResults.FirstOrDefault()?.ChildNodes?.Where(x => x.ChildNodes?.Count == 3)?.ToList() ?? null;
                    if (items?.Count > 1)
                    {
                        foreach (var tr in items.Skip(1))
                        {
                            if (tr.Name.Equals("tr") && tr.Attributes.Contains("class") && tr.Attributes["class"].Value.Equals("SearchResults"))
                            {
                                var nodes = tr.ChildNodes.ToList();
                                listOfResultClass.Add(new PropertyDetails
                                {
                                    BBL = nodes[0].InnerText.Trim(),
                                    Owner = nodes[1].InnerText.Trim(),
                                    Address = nodes[2].InnerText.Trim()
                                });
                            }
                        }

                        return listOfResultClass;
                    }
                    else
                    {
                        Console.WriteLine("No matching result found.");
                    }
                }
                else
                {
                    Console.WriteLine("Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                //TODO - Write log here
            }

            return null;
        }

        /// <summary>
        /// To parse property information
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public PropertyInfo ParsePropertyInfo(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var searchResults = htmlDoc.DocumentNode.SelectNodes("//table[@id='Property Owner(s)']");
            if (searchResults != null && searchResults.Count == 1)
            {
                var propInfo = new PropertyInfo
                {
                    PropertyOwner = searchResults.FirstOrDefault().InnerText.Trim()
                };

                // PropertyData
                var propertyData = htmlDoc.DocumentNode.SelectNodes("//table[@id='Property Data']");
                if (propertyData != null)
                {
                    var rows = propertyData.FirstOrDefault()?.ChildNodes?.ToList() ?? null;
                    if (rows?.Count > 0)
                    {
                        foreach (var tr in rows)
                        {
                            var text = tr.InnerText.Replace("&nbsp;", " ");

                            if (string.IsNullOrEmpty(text))
                                continue;

                            if (text.Contains("Tax Year"))
                                propInfo.TaxYear = text.Replace("Tax Year", "").Trim();
                            else if (text.Contains("Lot Grouping"))
                                propInfo.LotGrouping = text.Replace("Lot Grouping", "").Trim();
                            else if (text.Contains("Property Address"))
                                propInfo.PropertyAddress = text.Replace("Property Address", "").Trim();
                            else if (text.Contains("Tax Class"))
                                propInfo.TaxClass = text.Replace("Tax Class", "").Trim();
                            else if (text.Contains("Building Class"))
                                propInfo.BuildingClass = text.Replace("Building Class", "").Trim();
                            else if (text.Contains("Condo Development"))
                                propInfo.CondoDevelopment = text.Replace("Condo Development", "").Trim();
                            else if (text.Contains("Condo Suffix"))
                                propInfo.CondoSuffix = text.Replace("Condo Suffix", "").Trim();
                        }
                    }
                }

                return propInfo;
            }

            return null;
        }

        /// <summary>
        /// To Parse Notices of Property Value.
        /// </summary>
        /// <param name="html"></param>
        public List<NoticesOfPropertyValue> ParsePropertyNotices(string html)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var sideMenu = htmlDoc.DocumentNode.SelectNodes("//li//a//span[text()='Notices of Property Value']");
                SIDE_MENU:
                if (sideMenu != null && sideMenu.Count == 1 && sideMenu.FirstOrDefault().ParentNode != null)
                {
                    var propDetailURL = sideMenu.FirstOrDefault().ParentNode.Attributes.FirstOrDefault(x => x.Name.Equals("href"))?.Value ?? null;
                    if (string.IsNullOrEmpty(propDetailURL))
                    {
                        sideMenu = null;
                        goto SIDE_MENU;
                    }
                    propDetailURL = propDetailURL.Replace("../", "");
                    propDetailURL = $"https://a836-pts-access.nyc.gov/care/{propDetailURL}";

                    var htmlPropNotices = DoSimpleGetRequest(propDetailURL);

                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlPropNotices);


                    var searchResults = htmlDoc.DocumentNode.SelectNodes("//table[@id='Notices of Property Value']");
                    if (searchResults != null && searchResults.Count == 1)
                    {
                        if (searchResults.FirstOrDefault().ChildNodes?.Count == 0)
                        {
                            sideMenu = null;
                            goto SIDE_MENU;
                        }

                        var listOfNoticesPropVal = new List<NoticesOfPropertyValue>();
                        foreach (var tr in searchResults.FirstOrDefault().ChildNodes)
                        {
                            var text = tr.InnerText.Replace("&nbsp;", "").Trim();
                            if (string.IsNullOrEmpty(text) || tr.ChildNodes.Count < 3)
                                continue;

                            var objPropValue = new NoticesOfPropertyValue
                            {
                                Term = tr.ChildNodes[0].InnerText.Replace("&nbsp;", " ").Trim(),
                                Date = tr.ChildNodes[1].InnerText.Replace("&nbsp;", " ").Trim(),
                                Other = tr.ChildNodes[2].InnerText.Replace("&nbsp;", " ").Replace("&nbsp", "").Trim()
                            };

                            listOfNoticesPropVal.Add(objPropValue);
                        }

                        return listOfNoticesPropVal;
                    }
                }
                else
                {
                    Console.WriteLine(SupportClass.ResultNotFoundMessage);
                }
            }
            catch (Exception e)
            {
                // TODO - Write LOG here...
            }

            return null;
        }

        /// <summary>
        /// To Parse Print Summary.
        /// </summary>
        /// <param name="html"></param>
        public void ParsePrintSummary(string html)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                //function PrintDatalet(val)
                //{
                //    var doc = document.frmMain;
                //    var url = '../Datalets/PrintDatalet.aspx?pin=' + doc.hdPin.value +
                //          '&gsp=' + doc.hdMode.value + '&taxyear=' + doc.hdTaxYear.value +
                //          '&jur=' + doc.hdJur.value + '&ownseq=' + doc.hdOwnSeq.value +
                //          '&card=' + doc.hdCard.value + '&roll=' + roll +
                //          '&State=' + doc.hdMask.value + '&item=' + doc.hdItem.value +
                //          '&items=' + doc.hdItems.value + '&all=' + val + "&ranks=" + ranks;
                //    window.open(url, 'Print', "location=no, scrollbars=yes, menubar=yes, resizable=yes, width=900");
                //}

                var hdPinNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdPin']")?.FirstOrDefault() ?? null;
                var hdPin = SupportClass.GetHtmlNodeAttributeValue(hdPinNode);

                var hdModeNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdMode']")?.FirstOrDefault() ?? null;
                var hdMode = SupportClass.GetHtmlNodeAttributeValue(hdModeNode);

                var hdTaxYearNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdTaxYear']")?.FirstOrDefault() ?? null;
                var hdTaxYear = SupportClass.GetHtmlNodeAttributeValue(hdTaxYearNode);

                var hdJurNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdJur']")?.FirstOrDefault() ?? null;
                var hdJur = SupportClass.GetHtmlNodeAttributeValue(hdJurNode);

                var hdOwnSeqNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdOwnSeq']")?.FirstOrDefault() ?? null;
                var hdOwnSeq = SupportClass.GetHtmlNodeAttributeValue(hdOwnSeqNode);

                var hdCardNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdCard']")?.FirstOrDefault() ?? null;
                var hdCard = SupportClass.GetHtmlNodeAttributeValue(hdCardNode);

                var hdMaskNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdMask']")?.FirstOrDefault() ?? null;
                var hdMask = SupportClass.GetHtmlNodeAttributeValue(hdMaskNode);

                var hdItemNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdItem']")?.FirstOrDefault() ?? null;
                var hdItem = SupportClass.GetHtmlNodeAttributeValue(hdItemNode);

                var hdItemsNode = htmlDoc.DocumentNode.SelectNodes("//input[@id='hdItems']")?.FirstOrDefault() ?? null;
                var hdItems = SupportClass.GetHtmlNodeAttributeValue(hdItemsNode);

                var roll = "RP_NY";
                var all = "all";
                var ranks = "Datalet";

                var url = "../Datalets/PrintDatalet.aspx?pin=" + hdPin +
                      "&gsp=" + hdMode + "&taxyear=" + hdTaxYear +
                      "&jur=" + hdJur + "&ownseq=" + hdOwnSeq +
                      "&card=" + hdCard + "&roll=" + roll +
                      "&State=" + hdMask + "&item=" + hdItem +
                      "&items=" + hdItems + "&all=" + all + "&ranks=" + ranks;

                url = url.Replace("../Datalets", "/Datalets");
                url = $"https://a836-pts-access.nyc.gov/care{url}";

                //var htmlprint = DoSimpleGetRequest("https://a836-pts-access.nyc.gov/care/Datalets/PrintDatalet.aspx?pin=4105170018&gsp=NOPV&taxyear=2022&jur=65&ownseq=0&card=1&roll=RP_NY&State=1&item=1&items=-1&all=all&ranks=Datalet");
                var summaryHtml = DoSimpleGetRequest(url);

                htmlDoc.LoadHtml(summaryHtml);
                var propertyProfile = ParsePropertyProfile(htmlDoc);
                var accountHistorySummary = ParseAccountHistorySummary(htmlDoc);
                var accountHistoryDetails = ParseAccountHistoryDetails(htmlDoc);
                var taxBills = ParsePropertyTaxBills(htmlDoc);
                var finalAssessment = ParseFinalAssessment(htmlDoc);
                var landInformation = ParseLandInformation(htmlDoc);
                var assessmentInformation = ParseAssessmentInformation(htmlDoc);
                var taxable_BillableAssessed = ParseTaxable_BillableAssessedValue(htmlDoc);
                var exemptionInformation = ParseExemptionInformation(htmlDoc);
                var marketValues = ParseMarketValueHistory(htmlDoc);
            }
            catch (Exception ex)
            {

            }
        }

        public PropertyProfile ParsePropertyProfile(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {
                var tableProfile = htmlDoc.DocumentNode.SelectNodes("//table[@id='Profile']")?.FirstOrDefault() ?? null;
                if (tableProfile?.ChildNodes?.Count > 0)
                {
                    var profile = new PropertyProfile();

                    foreach (var tr in tableProfile.ChildNodes)
                    {
                        if (string.IsNullOrEmpty(tr.InnerText.Trim()) || tr.ChildNodes?.Count < 2)
                            continue;

                        var text = tr.InnerText.Trim();
                        var value = tr.ChildNodes[1].InnerText.Replace("&nbsp;", "").Trim();
                        if (string.IsNullOrEmpty(value))
                            continue;

                        if (text.Contains("Building Class"))
                            profile.BuildingClass = value;
                        else if (text.Contains("Tax Class"))
                            profile.TaxClass = value;
                        else if (text.Contains("Unused SCRIE Credit"))
                            profile.UnusedSCRIECredit = value;
                        else if (text.Contains("Unused DRIE Credit"))
                            profile.UnusedDRIECredit = value;
                        else if (text.Contains("Refund Available"))
                            profile.RefundAvailable = value;
                        else if (text.Contains("Overpayment amount"))
                            profile.Overpaymentamount = value;
                    }

                    return profile;
                }
            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public List<AccountHistorySummary> ParseAccountHistorySummary(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {
                var accountSummaryTable = htmlDoc.DocumentNode.SelectNodes("//table[@id='Account History Summary']")?.FirstOrDefault() ?? null;
                if (accountSummaryTable?.ChildNodes?.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public List<AccountHistoryDetails> ParseAccountHistoryDetails(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public List<PropertyTaxBills> ParsePropertyTaxBills(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public Exemptions ParseExemptions(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public FinalAssessment ParseFinalAssessment(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public LandInformation ParseLandInformation(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public List<AssessmentInformation> ParseAssessmentInformation(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public Taxable_BillableAssessedValue ParseTaxable_BillableAssessedValue(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public ExemptionInformation ParseExemptionInformation(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }

        public List<MarketValueHistory> ParseMarketValueHistory(HtmlDocument htmlDoc)
        {
            if (htmlDoc == null)
                return null;

            try
            {

            }
            catch (Exception ex)
            {
                // TODO - Write LOG here....
            }

            return null;
        }
    }
}
