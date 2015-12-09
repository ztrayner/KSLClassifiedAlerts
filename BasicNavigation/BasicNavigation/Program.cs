using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSLClassifiedAlerts.Context.Models;

namespace BasicNavigation
{
    class Program
    {
        static void Main(string[] args)
        {


            NavigationBasics getPageInfo = new NavigationBasics();
            getPageInfo.getAllCustomerSearches();
            getPageInfo.bigScrape();
            /*string PageCount = getPageInfo.getPageCount("https://www.ksl.com/auto/search/index?keyword=&make%5B%5D=Mazda&yearFrom=&yearTo=&mileageFrom=&mileageTo=&priceFrom=1&priceTo=1000&zip=&miles=25&newUsed%5B%5D=All&page=0&sellerType=&postedTime=&titleType=&body=&transmission=&cylinders=&liters=&fuel=&drive=&numberDoors=&exteriorCondition=&interiorCondition=&cx_navSource=hp_search&search.x=37&search.y=17&search=Search+raquo%3B");
            Console.WriteLine(PageCount);
            getPageInfo.updateSearchUrl();
            while(getPageInfo.searchCounter < getPageInfo.pageCount + 1)
            {
                getPageInfo.scrapeData();
            }

            List<Classified> newList = getPageInfo.allListings;
            Console.WriteLine(newList.Count);*/
            
        }
    }
}
