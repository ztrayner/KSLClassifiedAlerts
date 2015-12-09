using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KSLClassifiedAlerts.Context.Models;
using KSLClassifiedAlerts.Context.DAL;

namespace BasicNavigation
{
    class NavigationBasics
    {
        public KSLClassifiedAlertsContext db = new KSLClassifiedAlertsContext();
        public List<int> searchIDs = new List<int>();
        public string SearchURL { get; set; }
        public string pageToSearch { get; set; }
        public int pageCount { get; set; }
        public int searchCounter { get; set; }
        public HtmlWeb htmlWebContext = new HtmlWeb();
        public List<Classified> allListings = new List<Classified>();


        public string getPageCount(string SearchUrl)
        {
            string pageCount;
            int indexOfQuestionMark = SearchUrl.IndexOf('?') + 1;
            SearchUrl = SearchUrl.Insert(indexOfQuestionMark, "perPage=48");
            HtmlDocument htmlDocument = htmlWebContext.Load(SearchUrl);
            IEnumerable<HtmlNode> lastPageLink = htmlDocument.DocumentNode.SelectNodes("//*[@id='bodyCol1']/div[@class='srpMiddleColumn']/div[@class='solidNavBar']//a");
            HtmlNode lastLink = lastPageLink.ElementAt((lastPageLink.Count() - 2));
            pageCount = lastLink.InnerText;
            this.pageCount = Convert.ToInt32(lastLink.InnerText);
            this.searchCounter = 0;
            this.pageToSearch = SearchUrl;
            this.SearchURL = SearchUrl;
            return pageCount;
        }

        public void updateSearchUrl()
        {
            this.pageToSearch = this.SearchURL + string.Format("&page={0}", this.searchCounter);
            this.searchCounter += 1;
        }

        //customer saves search
        //search is passed into this console program
        //cars are scraped
        //duplicates are removed
        //object saved with new dataset

        public void scrapeData()
        {
            //get list of all IDs
            int[] CurrentListings = this.db.Classifieds.AsEnumerable().Select(c => (int)c.listingId).ToArray();

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
                if(!CurrentListings.Contains(listItem.listingId))
                {
                    this.db.Classifieds.Add(listItem);
                }
                
            }
            searchResults = null;
            GC.Collect();
            this.updateSearchUrl();
            this.db.SaveChanges();
        }

    }
}
