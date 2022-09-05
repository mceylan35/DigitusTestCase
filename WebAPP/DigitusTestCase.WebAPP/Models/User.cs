using System.ComponentModel.DataAnnotations;

namespace DigitusTestCase.WebAPP.Models
{
    public class User
    {
        //Adını, soyadını, e-posta adresini ve şifresini kullanarak
      //  [Required]
        public string Name { get; set; }
      //  [Required]
        public string SurName { get; set; }
       // [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

       // [Required]
        public string Password { get; set; }
        public string MailCode { get; set; }
        public bool IsActive { get; set; }

    }
}
