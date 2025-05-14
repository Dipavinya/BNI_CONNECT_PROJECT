using BniConnect.Models;
using System.Collections.Generic;

namespace BniConnect.ViewModel
{
    public class ShowTableViewModel
    {

        public int LastTemplateId {  get; set; }
        public List<TemplateMaster> AvailableTemplates { get; set; }
        public List<MailSettings> MailSettings { get; set; } 
        public List<Member> Members { get; set; }  
        public List<WhatsAppSetting> WhatsAppSetting { get; set; }
        public List<FileViewModel> Files { get; set; }
        public MemberSearchModel SearchParams { get; set; }
        public List<CountryMst> CountryMst { get; set; }
        public List<CategoryMst> CategoryMst { get; set; }
        public List<SubCategoryMst> SubCategoryMst { get; set; }
    }
}
