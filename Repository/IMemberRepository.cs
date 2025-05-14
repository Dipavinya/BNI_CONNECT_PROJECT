using BniConnect.Models;

namespace BniConnect.Repository
{
    public interface IMemberRepository
    {
       Task<List<Member>> SearchMembersAsync(MemberSearchModel model);

    }
}