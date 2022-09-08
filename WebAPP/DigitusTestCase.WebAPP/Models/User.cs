using System.ComponentModel.DataAnnotations;

namespace DigitusTestCase.WebAPP.Models
{
    public class User
    {
    
        public string Name { get; set; } 
        public string SurName { get; set; } 
        public string Email { get; set; }

       
        public string Password { get; set; }
         public string UserName { get; set; } 

    }
}
