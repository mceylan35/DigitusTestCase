using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace DigitusTestCase.WebAPP.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; } 
        public DateTime? MailConfirmationDate { get; set; }
    }
}
