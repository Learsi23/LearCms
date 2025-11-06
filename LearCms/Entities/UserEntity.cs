using Microsoft.AspNetCore.Identity;

namespace LearCms.Entities
{
    public class UserEntity : IdentityUser
    {
        public string FullName { get; set; }
    }
}
