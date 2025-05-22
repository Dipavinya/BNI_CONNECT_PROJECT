using BniConnect.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;


namespace BniConnect.Repository
{
    public class MemberRepository: IMemberRepository
    {
        private readonly IConfiguration _configuration;

        public MemberRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<Member>> SearchMembersAsync(MemberSearchModel model)

        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();
            var sql = @"
        SELECT  
    m.*,
    CASE 
        WHEN c.UserId IS NULL THEN 0 
        ELSE 1 
    END AS IsSuccess
FROM members_profiles AS m 
LEFT JOIN ConnectionSentHistory AS c 
    ON m.user_id = c.UserId
        WHERE 
            (@MemberKeywords IS NULL OR 
                (Name LIKE '%' + @MemberKeywords + '%' OR 
                 company LIKE '%' + @MemberKeywords + '%'))
            AND (@MemberFirstName IS NULL OR Name LIKE '%' + @MemberFirstName + '%')
            AND (@MemberLastName IS NULL OR Name LIKE '%' + @MemberLastName + '%')
            AND (@MemberCompanyName IS NULL OR company LIKE '%' + @MemberCompanyName + '%')
            AND (@MemberCountryId IS NULL OR Country = @MemberCountryId)
            AND (@MemberCity IS NULL OR city LIKE '%' + @MemberCity + '%')
            AND (@MemberPrimaryCategory IS NULL OR Category = @MemberPrimaryCategory)
            AND(@MemberSecondaryCategory IS NULL OR member_subcategory = @MemberSecondaryCategory)
        ORDER BY date_created DESC
    ";
      

            var members = await connection.QueryAsync<Member>(sql, new
            {

                model.MemberKeywords,
                model.MemberFirstName,
                model.MemberLastName,
                model.MemberCompanyName,
                model.MemberCountryId,
                model.MemberCity,
                model.MemberPrimaryCategory,
                model.MemberSecondaryCategory,
               
            });
            return members.ToList();
        }

    }
}
