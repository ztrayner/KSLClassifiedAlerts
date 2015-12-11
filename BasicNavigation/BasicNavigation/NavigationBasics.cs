using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KSLClassifiedAlerts.Context.Models;
using KSLClassifiedAlerts.Context.DAL;
using System.Net;
using System.Net.Mail;

namespace BasicNavigation
{
    class NavigationBasics
    {
        public KSLClassifiedAlertsContext db = new KSLClassifiedAlertsContext();
        public List<string> userIDs = new List<string>();
        public List<Search> searchIDs = new List<Search>();
        public string SearchURL { get; set; }
        public string pageToSearch { get; set; }
        public int pageCount { get; set; }
        public int searchCounter { get; set; }
        public HtmlWeb htmlWebContext = new HtmlWeb();
        public List<Classified> allListings = new List<Classified>();

        public string sendEmail(string toAddress, string subject, string body)
        {

            string result = "Message sent Successfully..!!";
            var fromAddress = "kslclassifiedsscraper@gmail.com";
            const string fromPassword = "foxconsulting";

            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, fromPassword),
                    Timeout = 30000
                };

                MailMessage message = new MailMessage(fromAddress, toAddress, subject, body);
                message.IsBodyHtml = true;
                smtp.Send(message);
            }

            catch (Exception e)
            {
                result = "Error sending Email!";
                return result;
            }

            return result;
        }

        public void getPageCount(string SearchUrl)
        {
            string pageCount;
            int indexOfQuestionMark = SearchUrl.IndexOf('?') + 1;
            SearchUrl = SearchUrl.Insert(indexOfQuestionMark, "perPage=48");
            HtmlDocument htmlDocument = htmlWebContext.Load(SearchUrl);
            IEnumerable<HtmlNode> lastPageLink = htmlDocument.DocumentNode.SelectNodes("//*[@id='bodyCol1']/div[@class='srpMiddleColumn']/div[@class='solidNavBar']//a");
            if(lastPageLink != null)
            {
                HtmlNode lastLink = lastPageLink.ElementAt((lastPageLink.Count() - 2));
                pageCount = lastLink.InnerText;
                this.pageCount = Convert.ToInt32(lastLink.InnerText);
            }
            else
            {
                this.pageCount = 0;
            }
            
            
            this.searchCounter = 0;
            this.pageToSearch = SearchUrl;
            this.SearchURL = SearchUrl;
        }

        public void updateSearchUrl()
        {

            this.pageToSearch = this.SearchURL + string.Format("&page={0}", this.searchCounter);
            this.searchCounter += 1;
        }

        public void getAllCustomerSearches()
        {
            searchIDs = this.db.Searches.AsEnumerable().Select(s => s).ToList();
        }

        public void bigScrape()
        {
            //go through for each search
            foreach(Search search in searchIDs)
            {
                string userEmail = (string)this.db.Users.Where(u => u.Id == search.UserID).AsEnumerable().Select(u => u.Email).First();

                List< Classified > newListings = new List<Classified>();

                List<int> CurrentListings = this.db.Classifieds.Where(c => c.SearchId == search.SearchId).AsEnumerable().Select(c => (int)c.kslId).ToList();
                this.searchCounter = 0;
                this.getPageCount(search.SearchURL);

                while(this.searchCounter<this.pageCount + 1)
                { 
                HtmlDocument searchResults = htmlWebContext.Load(search.SearchURL);
                IEnumerable<HtmlNode> rightSection = searchResults.DocumentNode.SelectNodes("//div[@class='srp-listing-body']");
                foreach (var listing in rightSection)
                {
                    Classified listItem = new Classified();
                    listItem.imageUrl = searchResults.DocumentNode.SelectSingleNode(".//div[@class='srp-listing-bodyLeft']/div/a/img").Attributes["src"].Value;
                    listItem.cityToParse = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-city']").InnerText;
                    listItem.title = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-title']/a").InnerText;
                    listItem.link = "www.ksl.com" + listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-title']/a").Attributes["href"].Value;
                    listItem.priceToParse = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-price']").InnerText;
                    listItem.search = search;
                    listItem.ParseListing();
                    allListings.Add(listItem);
                    //if they item is not there then add it to the the classified object
                    if (!CurrentListings.Contains(listItem.kslId) | CurrentListings == null)
                    {
                        newListings.Add(listItem);
                        this.db.Classifieds.Add(listItem);
                        this.db.SaveChanges();
                    }
                    //otherwise, remove it from our copy of the list, so that at the end we will have a list of items to delete
                    else
                    {
                        CurrentListings.Remove(listItem.kslId);
                    }

                }
                if(CurrentListings.Count > 0)
                    { 
                foreach (var ID in CurrentListings)
                {
                    var toDelete = this.db.Classifieds.Where(s => s.SearchId == search.SearchId & s.kslId == ID).First();
                    this.db.Classifieds.Remove(toDelete);
                    this.db.SaveChanges();
                }
                    }
                    this.updateSearchUrl();
                }

                //this is where you will loop through NewListings and format the email
                string body = "";
                if(newListings.Count > 0)
                {
                    foreach (var obj in newListings)
                    {
                        body += "<table border='0' cellpadding='0' width='100%'><tr><td><table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border-collapse: collapse;'><tr><td bgcolor='#70bbd9' align='center' style='padding: 40px 0 40px 0;'><h1>FoxConsulting</h1><img src='http://static1.squarespace.com/static/53b9a5dde4b0cdea486f7b53/t/53d041f1e4b0625ce7a9a4c5/1406157308476/' alt='Creating Email Magic' width='300' height='230' style='display: block;' /></td></tr><tr><td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'><b>" + obj.title + "</b></td></tr><tr><td style='padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'><b>Link: </b>" + obj.link + "</td></tr><tr><td><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td width='260' valign='top'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td><img src='" + obj.imageUrl + "' alt='' width='100%' height='140' style='display: block;' /></td></tr><tr><td style='padding: 25px 0 0 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'><b>Price: </b>" + obj.priceToParse + "<br><b>City: </b>" + obj.city + "</td></tr></table></td><td style='font-size: 0; line-height: 0;' width='20'>&nbsp;</td><td width='260' valign='top'></td></tr><tr><td bgcolor='#ee4c50' style='padding: 30px 30px 30px 30px;'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td width='75%' style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;'>FoxConsulting &copy;2015<br/><a href='#' style='color: #ffffff;'><font color='#ffffff'>Unsubscribe</font></a> to this newsletter instantly</td><td align='right'><table border='0' cellpadding='0' cellspacing='0'><tr><td><a href='http://www.twitter.com/'><img src='http://cdn.mysitemyway.com/etc-mysitemyway/icons/legacy-previews/icons-256/3d-transparent-glass-icons-social-media-logos/097305-3d-transparent-glass-icon-social-media-logos-twitter.png' alt='Twitter' width='38' height='38' style='display: block;' border='0' /></a></td><td style='font-size: 0; line-height: 0;' width='20'>&nbsp;</td><td><a href='http://www.twitter.com/'><img src='http://cdn.mysitemyway.com/etc-mysitemyway/icons/legacy-previews/icons/3d-transparent-glass-icons-social-media-logos/097233-3d-transparent-glass-icon-social-media-logos-facebook-logo.png' alt='Facebook' width='38' height='38' style='display: block;' border='0' /></a></td></tr></table></td></tr></table></td></tr></table></td></tr>";
                    }

                    sendEmail(userEmail, "New KSL Listings", body);
                    sendEmail("tanner.sawyer@gmail.com", "New KSL Listings", body);
                }
                
                GC.Collect();
                
                this.db.SaveChanges();

            }
        }


        /*public void scrapeData()
        {
            //get list of all IDs
            List<int> CurrentListings = this.db.Classifieds.AsEnumerable().Select(c => (int)c.kslId).ToList();

            HtmlDocument searchResults = htmlWebContext.Load(pageToSearch);            
            IEnumerable<HtmlNode> rightSection = searchResults.DocumentNode.SelectNodes("//div[@class='srp-listing-body']");
            foreach(var listing in rightSection)
            {
                Classified listItem = new Classified();
                listItem.imageUrl = searchResults.DocumentNode.SelectSingleNode(".//div[@class='srp-listing-bodyLeft']/div/a/img").Attributes["src"].Value;
                listItem.cityToParse = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-city']").InnerText;
                listItem.title = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-title']/a").InnerText;
                listItem.link = "www.ksl.com"+ listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-title']/a").Attributes["href"].Value;
                listItem.priceToParse = listing.SelectSingleNode(".//div[@class='srp-listing-body-right']/div[@class='srp-listing-price']").InnerText;
                listItem.ParseListing();
                allListings.Add(listItem);
                //if they item is not there then add it to the the classified object
                if(!CurrentListings.Contains(listItem.kslId))
                {
                    this.db.Classifieds.Add(listItem);
                    this.db.SaveChanges();
                }
                //otherwise, remove it from our copy of the list, so that at the end we will have a list of 
                else
                {
                    CurrentListings.Remove(listItem.kslId);
                }
                
            }

            foreach(var ID in CurrentListings)
            {
                var toDelete = new Classified { kslId = ID };
                this.db.Classifieds.Remove(toDelete);
            }

            searchResults = null;
            GC.Collect();
            this.updateSearchUrl();
            this.db.SaveChanges();
        }
        */
    }
}
