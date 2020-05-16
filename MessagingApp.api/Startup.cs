using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagingApp.api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MessagingApp.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //ConnectionString Buraya
            services.AddControllers();
            services.AddDbContext<DataContext>(x=>x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IAuthRepository,AuthRepository>();
          
          //Authentication ayarları
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme
            ).AddJwtBearer(
                options => {
                    options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,                        
                            ValidateIssuer=false,
                            ValidateAudience=false,
                           IssuerSigningKey= new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:token").Value))                        
                    };

                }

                
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //geliştirme mi debug mı 
            if (env.IsDevelopment())
            {
                //develeopr ortamında geliştiricye daha detaylı bilgi verir
                app.UseDeveloperExceptionPage();
            }
            //talepleri https e yönlendirir
           // app.UseHttpsRedirection();

            //spa ve api portları farklı olduğundan ikisi 
            //arasında iletişimi sağlamak için eklendi
            app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
           
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
