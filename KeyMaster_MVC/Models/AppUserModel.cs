using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyMaster_MVC.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }

        // ForeignKey relationship to IdentityRole
        [ForeignKey("RoleId")]
        public string RoleId { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}
