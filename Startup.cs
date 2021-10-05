using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderAPI.Database;
using System;
using System.Text;
using OrderAPI.Services;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace OrderAPI {
    public class Startup {

        private IConfiguration _configuration { get; }
        
        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<TokenService>(); 
            services.AddDbContext<DBContext>(ops => ops.UseMySQL(_configuration.GetConnectionString("MySQLConnection")));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.AddControllers();
            services.AddOptions();

            // services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            
            services.AddSwaggerGen(ops => {
                ops.SwaggerDoc("v1", 
                    new Microsoft.OpenApi.Models.OpenApiInfo {
                        Title = "Swagger Demo API",
                        Description = "Demo API for showing Swagger",
                        Version = "v1"
                    }
                );
            });

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings:Secret").Value);
            services.AddAuthentication(ops => {
                ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(ops => {
                ops.RequireHttpsMetadata = false;
                ops.SaveToken = true;
                ops.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });   
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors(ops => {
                ops.AllowAnyOrigin();
                ops.AllowAnyMethod();
                ops.AllowAnyHeader();
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(ops => {
                ops.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });
        }
    }
}
