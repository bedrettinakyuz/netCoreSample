using System.Threading.Tasks;
using MessagingApp.api.Models;

namespace MessagingApp.api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username , string password);
         //kullanıcının  var olup olmadığını kontrol eder
         Task<bool> UserExists(string username);
         
    }
}