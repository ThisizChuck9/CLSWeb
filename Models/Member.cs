namespace CLSWeb.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public int MemberNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string BillType { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}