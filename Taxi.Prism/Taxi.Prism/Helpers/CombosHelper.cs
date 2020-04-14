using System.Collections.Generic;
using Taxi.Common.Models;

namespace Taxi.Prism.Helpers
{
    public static class CombosHelper
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Id = 1, Name = Languages.User },
                new Role { Id = 2, Name = Languages.Driver }
            };
        }
        public static List<Comment> GetComments()
        {
            return new List<Comment>
            {
                new Comment { Id = 1, Name = Languages.Comment1 },
                new Comment { Id = 2, Name = Languages.Comment2 },
                new Comment { Id = 2, Name = Languages.Comment3 },
                new Comment { Id = 2, Name = Languages.Comment4 },
                new Comment { Id = 2, Name = Languages.Comment5 },
                new Comment { Id = 2, Name = Languages.Comment6 }
            };
        }
    }
}
