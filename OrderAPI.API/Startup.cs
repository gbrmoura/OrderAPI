using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderAPI.API.Configurations;
using OrderAPI.API.Services;
using OrderAPI.Data;
using System;
using System.Text;

namespace OrderAPI.API {

    public class Startup {

        private IConfiguration _configuration { get; }
        
        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {
            AuthenticationConfig authenticationConfig = new AuthenticationConfig();
            _configuration.Bind("Authentication", authenticationConfig);

            string connectionString = _configuration.GetConnectionString("MySQLConnection");
            services.AddDbContextPool<OrderAPIContext>(
                ops => ops.UseMySQL(connectionString)
            );

            services.AddSingleton<TokenService>();
            services.AddSingleton(authenticationConfig);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.AddControllers();
            services.AddOptions();
           
            services.AddSwaggerGen(ops => {
                ops.SwaggerDoc("v1", 
                    new Microsoft.OpenApi.Models.OpenApiInfo {
                        Title = "Swagger Demo API",
                        Description = "Demo API for showing Swagger",
                        Version = "v1"
                    }
                );
            });

            var key = Encoding.ASCII.GetBytes(authenticationConfig.AccessTokenSecret);
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
                    // ValidateIssuer = true,
                    // ValidateAudience = true,
                    // ValidIssuer = authenticationConfig.Issuer,
                    // ValidAudience = authenticationConfig.Audience
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
