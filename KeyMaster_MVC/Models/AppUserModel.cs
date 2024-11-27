using Microsoft.AspNetCore.Identity;

namespace KeyMaster_MVC.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
    }
}
