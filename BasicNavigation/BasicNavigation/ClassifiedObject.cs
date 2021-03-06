﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNavigation
{
    class ClassifiedObject
    {
        public int listingId {get; set;}
		public string cityToParse { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string imageUrl { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public int mileage { get; set; }
        public string priceToParse { get; set; }
        public int price { get; set; }
        public bool parsed { get; set; }

        public void ParseListing()
        {
            this.ParseCity();
            this.ParsePrice();
            this.ParseId();
            this.parsed = true;
        }

        public void ParseCity()
        {
            if(!this.parsed)
            {
                string[] substrings = this.cityToParse.Split('|');
                string mileage = substrings[0];
                string toRemove = "Miles";
                int index = mileage.IndexOf(toRemove);
                mileage = mileage.Remove(index, toRemove.Length).Replace(",", "");
                this.mileage = Int32.Parse(mileage);
                string cityState = substrings[1];
                string[] cityStatStrings = cityState.Split(',');
                this.city = cityStatStrings[0].Trim();
                this.state = cityStatStrings[1].Trim();
            }
            
        }

        public void ParsePrice()
        {
            if(!this.parsed)
            {
                this.price = (int)Int64.Parse(priceToParse.Trim('$').Replace(",", ""));
            }
            
        }

        public void ParseId()
        {
            string[] idSubstrings = this.link.Split('/');
            int index = idSubstrings[3].IndexOf('?');
            int LengthToDelete = idSubstrings[3].Length - index;
            this.listingId = Int32.Parse(idSubstrings[3].Remove(index,LengthToDelete));
        }

                
    }
}
