﻿using System.Text.Json.Serialization;

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
        public string MemberPrimaryCategoryId { get; set; }

        public string MemberSecondaryCategory { get; set; }
        public string MemberCountryId { get; set; }
        public int? CurrentPage { get; set; } = 1;

        public int? TotalPages { get; set; } = 2;


        //[JsonPropertyName("memberKeywords")]
        //public string MemberKeywords { get; set; }

        //[JsonPropertyName("memberFirstName")]
        //public string MemberFirstName { get; set; }

        //[JsonPropertyName("memberLastName")]
        //public string MemberLastName { get; set; }

        //[JsonPropertyName("memberCompanyName")]
        //public string MemberCompanyName { get; set; }

        //[JsonPropertyName("memberCity")]
        //public string MemberCity { get; set; }

        //[JsonPropertyName("memberState")]
        //public string MemberState { get; set; }

        //[JsonPropertyName("memberPrimaryCategory")]
        //public string MemberPrimaryCategory { get; set; }

        //[JsonPropertyName("memberSecondaryCategory")]
        //public string MemberSecondaryCategory { get; set; }

        //[JsonPropertyName("memberCountryId")]
        //public string MemberCountryId { get; set; }

        //[JsonPropertyName("currentPage")]
        //public int? CurrentPage { get; set; } = 1;

        //[JsonPropertyName("totalPages")]
        //public int? TotalPages { get; set; } = 2;

    }

}
