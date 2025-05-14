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
        SELECT *
        FROM members_profiles
        WHERE 
            (@MemberKeywords IS NULL OR 
                (display_name LIKE '%' + @MemberKeywords + '%' OR 
                 company LIKE '%' + @MemberKeywords + '%'))
            AND (@MemberFirstName IS NULL OR display_name LIKE '%' + @MemberFirstName + '%')
            AND (@MemberLastName IS NULL OR display_name LIKE '%' + @MemberLastName + '%')
            AND (@MemberCompanyName IS NULL OR company LIKE '%' + @MemberCompanyName + '%')
            AND (@MemberCountryId IS NULL OR country = @MemberCountryId)
            AND (@MemberCity IS NULL OR city LIKE '%' + @MemberCity + '%')
            AND (@MemberPrimaryCategory IS NULL OR member_category = @MemberPrimaryCategory)
            AND (@MemberSecondaryCategory IS NULL OR member_subcategory = @MemberSecondaryCategory)
        ORDER BY date_created DESC;
    ";
        //OFFSET @Offset ROWS FETCH NEXT @PerPage ROWS ONLY;

            //return await connection.QueryAsync<Member>(sql, new
            //{

            //    model.MemberKeywords,
            //    model.MemberFirstName,
            //    model.MemberLastName,
            //    model.MemberCompanyName,
            //    model.MemberCountryId,
            //    model.MemberCity,
            //    model.MemberPrimaryCategory,
            //    model.MemberSecondaryCategory,
            //    Offset = (model.CurrentPage - 1) * 25,
            //    PerPage = 25
            //});

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
                Offset = (model.CurrentPage - 1) * 25,
                PerPage = 25
            });
            return members.ToList();
        }

    }
}
