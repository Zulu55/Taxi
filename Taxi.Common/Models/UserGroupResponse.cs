using System.Collections.Generic;

namespace Taxi.Common.Models
{
    public class UserGroupResponse
    {
        public int Id { get; set; }

        public UserResponse User { get; set; }

        public List<UserGroupDetailResponse> Users { get; set; }
    }
}
