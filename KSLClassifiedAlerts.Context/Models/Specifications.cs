using System.ComponentModel.DataAnnotations;

namespace KSLClassifiedAlerts.Context.Models
{
    public class Specification
    {
        [Key]
        public int Id { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
        public string Body { get; set; }
        public int? Mileage { get; set; }
        public string VIN { get; set; }
        public string TitleType { get; set; }
        public string ExteriorColor { get; set; }
        public string InteriorColor { get; set; }
        public string Transmission { get; set; }
        public int? Liters { get; set; }
        public int? Cylinders { get; set; }
    }
}