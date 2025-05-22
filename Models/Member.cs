namespace BniConnect.Models
{
    public class Member
    {
        public string UserId { get; set; } 
        public string user_id { get; set; }
        public string Name { get; set; }
        public string Chapter { get; set; }
        public string Company { get; set; }
        public string City { get; set; }
        public string IndustryClassification { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public bool ConnectionSent { get; set; }
        public bool IsSuccess{ get; set; }

        public int Id { get; set; }
        public string MemberCategory { get; set; }
        public string MemberSubcategory { get; set; }
        //public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MobileNumber { get; set; }
        public string Country { get; set; }
        public string LinkedinUrl { get; set; }
        public string Website { get; set; }
        public string Goals { get; set; }
        public string Accomplishments { get; set; }
        public string Interests { get; set; }
        public string Networks { get; set; }
        public string Skills { get; set; }
        public string IdealReferral { get; set; }
        public string TopProduct { get; set; }
        public string TopProblemSolved { get; set; }
        public string FavoriteBniStory { get; set; }
        public string IdealReferralPartner { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
