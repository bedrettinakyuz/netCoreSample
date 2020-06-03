using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MessagingApp.api.Data;
using MessagingApp.api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            }else{
                //Global Exception Handling
                app.UseExceptionHandler(
                    builder =>{
                        builder.Run(
                            //context yapılmış olan hem talepi hem response u ifade ediyor
                            //build bir hata aldığında response ve request ile ilgili nesnelere erişebiliyor
                            //
                            async context => {
                                   context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;     
                                    var error = context.Features.Get<IExceptionHandlerFeature>();

                                    if (error!=null){
                                        context.Response.AddApplicationError(error.Error.Message);
                                        await context.Response.WriteAsync(error.Error.Message);
                                    }
                            
                            }


                        );
                    }
                    
                );
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
