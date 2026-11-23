namespace CLSWeb.Models
{
    public class Photo
    {
        public int PhotoID { get; set; }
        public int ListingID { get; set; }

        public string PhotoURL { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime UploadDate { get; set; }

        public Listing Listing { get; set; } = null!;
    }
}