using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace KSLClassifiedAlerts.Context.Models
{
    public class Classified
    {
        [Key]
        public int ClassifiedId { get; set; }
        public string Title { get; set; }
        [Display(Name = "Link URL")]
        public string LinkUrl { get; set; }
        public int? Miles { get; set; }
        public string Location { get; set; }
        public double? Price { get; set; }
        public DateTime DatePosted { get; set; }
        [Display(Name = "For Sale By")]
        public string ForSaleBy { get; set; }
        public string Description { get; set; }
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        public virtual Specification Specifications { get; set; }
        public virtual ICollection<Search> Search { get; set; }

    }
    public class Search
    {
        [Key]
        public int SearchId { get; set; }
        public string searchURL { get; set; }
        //I'm not sure how in depth we want to get with users and such but either way we wil need to referenc the userID
        public int userID { get; set; }
        
        public virtual ICollection<Classified> Classified { get; set; }
        
    }
    
    public class SearchClassified
    {
        [Key, Column(Order = 0)]
        public int SearchId { get; set; }
        [Key, Column(Order = 1)]
        public int ClassifiedId { get; set; }
        public virtual Search Search { get; set; }
        public virtual Classified Classified { get; set; }
    }
}