using System.ComponentModel.DataAnnotations;

namespace CustomIdentityUser.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
