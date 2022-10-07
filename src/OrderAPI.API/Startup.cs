using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderAPI.API.Configurations;
using OrderAPI.API.Services;
using OrderAPI.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderAPI.API 
{

    public class Startup 
    {

        private IConfiguration _configuration { get; }
        
        public Startup(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) 
        {
            
            AuthenticationConfig authenticationConfig = new AuthenticationConfig();
            _configuration.Bind("Authentication", authenticationConfig);

            /** Database Context */
            string connectionString = _configuration.GetConnectionString("MySQLConnection");
            services.AddDbContextPool<OrderAPIContext>(ops => ops.UseMySQL(connectionString));
            
            /** Configuration Class **/
            services.AddSingleton(authenticationConfig);

            /** Scoped Services **/
            services.AddScoped<TokenService>();
            services.AddScoped<FileService>();
            services.AddScoped<ModelService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<UtilsService>();
            services.AddScoped<MailService>();

            /** Singleton Services **/
            
            
            /** Others **/
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.AddControllers();
            services.AddOptions();
           
           /** Swagger **/
            services.AddSwaggerGen(ops => 
            {
                ops.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Autenticação baseada em Json Web Token (JWT)",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    }
                );

                ops.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference()
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                },
                                Scheme = "oauth2",
                                Name = JwtBearerDefaults.AuthenticationScheme,
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    }
                );

                ops.SwaggerDoc("v1", 
                    new Microsoft.OpenApi.Models.OpenApiInfo 
                    {
                        Version = "v0.0.5",
                        Title = "Documentação OrderAPI",
                        Description = "Documentação da api para uso do Instituto Federeal de São Paulo - Campus Boituva",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "gbr@gabrielalves.net",
                            Name = "Gabriel Alves de Moura",
                            Url = new Uri("https://github.com/gbrextreme"),
                        }                       
                    }
                );
            });

            /** JWT Authentication **/
            services.AddAuthentication(ops => 
            {
                ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(ops => 
            {
                ops.RequireHttpsMetadata = false;
                ops.SaveToken = true;
                ops.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey( Encoding.ASCII.GetBytes(authenticationConfig.AccessTokenSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });   
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCors(ops => 
            {
                ops.AllowAnyOrigin();
                ops.AllowAnyMethod();
                ops.AllowAnyHeader();
            });

            app.UseSwagger();
            app.UseSwaggerUI(ops => 
            {
                ops.SwaggerEndpoint("swagger/v1/swagger.json", "OrderAPI");
                ops.RoutePrefix = String.Empty;
            });

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
            });
        }
    }
}
