namespace Taxi.Web.Data.Entities
{
    public class UserGroupRequestEntity
    {
        public int Id { get; set; }

        public UserEntity ProposalUser { get; set; }

        public UserEntity RequiredUser { get; set; }

        public bool WasAccepted { get; set; }
    }
}
