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
                List < Classified > newListings = new List<Classified>();

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
                        body += "<p>Title: " + obj.title + "</p>";
                        body += "<p>Price: " + obj.priceToParse + "</p>";
                        body += "<p>City: " + obj.city + "</p>";
                        body += "<p><img src='" + obj.imageUrl + "'/></p>";
                    }

                    sendEmail("tanner.sawyer@gmail.com", "Test", body);
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
