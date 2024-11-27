using System.ComponentModel.DataAnnotations;

namespace KeyMaster_MVC.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Nhập UseName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Nhập Email"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Nhập Password ")]
        public string Password { get; set; }

    }
}
