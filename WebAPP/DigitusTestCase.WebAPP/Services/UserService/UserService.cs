using DigitusTestCase.WebAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DigitusTestCase.WebAPP.Services.UserService
{
    public class UserService : UserManager<ApplicationUser>
    {
        public UserService(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            RegisterTokenProvider(TokenOptions.DefaultEmailProvider, new EmailTokenProvider<ApplicationUser>());
        }

        public double CompleteLoginAverage(DateTime dateTime)
        {
        
            var userList = this.Users.Where(c => c.EmailConfirmed == true);// && c.CreatedOn.Date == dateTime.Date);
            List<double> dateBetween = new List<double>();
            foreach (var item in userList)
            {
                TimeSpan timeSpan = (DateTime)item.MailConfirmationDate - item.CreatedOn;

                dateBetween.Add(timeSpan.TotalSeconds);
            }
            return dateBetween.Average();
        }
        public int SentVerificationCodeButDidNotRegisterAfter(DateTime date)
        {
          
            var count = this.Users.Count(i => i.EmailConfirmed == false && date > DateTime.Now);
            return count;
        }
        public int ListingByTimeRange(DateTime date)
        {
           
            var oneDate = DateTime.Now.AddDays(-1);
            return this.Users.Count(i => i.CreatedOn > date && i.CreatedOn < DateTime.Now);
            
        }
    }
}
