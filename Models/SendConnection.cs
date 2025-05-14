namespace BniConnect.Models
{
    public class SendConnection
    {
        public bool IsFormSubmit { get; set; }

        public List<MemberConnection> Members { get; set; }

        public string AddToMyConnectionsBody { get; set; }

    }
    public class MemberConnection
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
