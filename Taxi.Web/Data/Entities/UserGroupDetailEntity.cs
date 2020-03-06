namespace Taxi.Web.Data.Entities
{
    public class UserGroupDetailEntity
    {
        public int Id { get; set; }

        public UserEntity User { get; set; }

        public UserGroupEntity UserGroup { get; set; }
    }
}
