using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderAPI.Configs;
using OrderAPI.Database;
using System;
using System.Text;

namespace OrderAPI {
    public class Startup {

        private IConfiguration _config { get; }
        
        public Startup(IConfiguration configuration) {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<DBContext>(ops => ops.UseMySQL(_config.GetConnectionString("MySQLConnection")));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.AddControllers();
            services.AddOptions();

            var key = Encoding.ASCII.GetBytes(TokenConfigs.Secret);
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
            //app.UseHttpsRedirection();
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
        }
    }
}
