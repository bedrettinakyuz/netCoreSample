using System.ComponentModel.DataAnnotations;

namespace MessagingApp.api.Dtos
{
    public class UserForLoginDto
    {
      
        public string  UserName { get; set; }

        public string Password { get; set; }
    
    }
}