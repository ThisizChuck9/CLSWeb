namespace CLSWeb.Models
{
    public class Listing
    {
        public int ListingID { get; set; }
        public int CLSNumber { get; set; }
        public int MemberID { get; set; }

        public string PropertyType { get; set; } = string.Empty;
        public string PropertyStatus { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

        public decimal ListPrice { get; set; }
        public int Bedrooms { get; set; }
        public decimal Bathrooms { get; set; }

        public int? SquareFeet { get; set; }
        public decimal? HOAFees { get; set; }
        public int? YearBuilt { get; set; }
        public string? ListingDescription { get; set; }

        public DateTime DateListed { get; set; }

        // Member who owns/entered this listing
        public Member Member { get; set; } = null!;

        // Photos associated with this listing
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}