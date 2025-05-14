namespace BniConnect.Models
{
    public class MemberSearchModel
    {
        public string MemberKeywords { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string MemberCompanyName { get; set; }
        public string MemberCity { get; set; }
        public string MemberState { get; set; }
        public string MemberPrimaryCategory { get; set; }
        public string MemberSecondaryCategory { get; set; }
        public string MemberCountryId { get; set; }
        public int? CurrentPage { get; set; } = 1;

        public int? TotalPages { get; set; } = 2;


    }

}
