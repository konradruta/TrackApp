using Microsoft.Extensions.Primitives;

namespace TrackAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
