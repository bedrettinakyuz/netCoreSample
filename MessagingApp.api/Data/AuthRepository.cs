using System;
using System.Threading.Tasks;
using MessagingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
            
        }

        public DataContext Context { get; }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==username);
            if (user==null)
                return null;
            if(!VerifyPassword(password,user.PasswordSalt,user.PasswordHash))   
                return null;

            return user; 
        }

        private bool VerifyPassword(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            //elimizdeki salt değeriyle ve password parametresi kullanılarak oluşturulan hash kullanılarak login
            //yapıldı
            //salt her kullanıcı için özel bir değerdir
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
               
                var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i=0;i<computedHash.Length;i++){
                    if(computedHash[i]!=passwordHash[i]){
                        return false;
                    }
                }
                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            //out anahtar deyimi sayesinde
            //dışardaki metod gövdesinde pasworHash ve paswordSalt değerleri
            //değiştiğinde bu gövde içinde olan değişkenlerin değerleri de değişmişi
            //bulunacak 
            createPasswordHash(password,out passwordHash,out passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //hmacsha512 sınıfına ait dispose metdou bulunuyor
            //using içine alarak bu değişken kullanıldıktan sonra 
            //veritabanından silinmesi sağlanıyor
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt=hmac.Key;
                //compute hash metodu byte istediğinden parametre olarak aldığımızı  password ü byte a çevidik
            
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));  

            };
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x=>x.UserName == username))
                return true;
            return false;    
        }
    }
}