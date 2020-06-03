using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingApp.api.Data;
using MessagingApp.api.Dtos;
using MessagingApp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MessagingApp.api.Controllers
{   
    //talepler yapılırken kimlik doğrulma yapılır
    
    [Route("api/[controller]")]
    [ApiController]
    //validationların kullanılabilmesi için ve dtos la bağlantı kurmak için
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister){
                //validate input
         userForRegister.Username = userForRegister.Username.ToLower();
        if(await _repo.UserExists(userForRegister.Username)){
            return BadRequest("Username already exists");
        }
         var userToCreate = new User(){
             UserName = userForRegister.Username
         };//Register
         var createdUser = _repo.Register(userToCreate,userForRegister.Password);
         return StatusCode(201);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login (UserForLoginDto userForLoginDto)
        {

            

            var userFromRepo = await _repo.Login(userForLoginDto.UserName,userForLoginDto.Password);
            if(userFromRepo == null)
                return Unauthorized();
            //nameIden. veritabanındaki kullanıcın id si
            //claims gereksiz bilgiler
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)

            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:token").Value));
            
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(claims),
                //token gecerlilik süresi 24 saat
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };
            //token oluşturucu
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
            });

    }
}
}