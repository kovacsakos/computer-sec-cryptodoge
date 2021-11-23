using CryptoDoge.BLL;
using CryptoDoge.BLL.Interfaces;
using CryptoDoge.BLL.Services;
using CryptoDoge.BLL.ValidationDtos;
using CryptoDoge.DAL;
using CryptoDoge.DAL.Repositories;
using CryptoDoge.Model.Entities;
using CryptoDoge.Model.Interfaces;
using CryptoDoge.ParserService;
using CryptoDoge.Server.Infrastructure.Services;
using CryptoDoge.Shared;
using FluentValidation.AspNetCore;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoDoge.Server
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
            var allowedCorsOrigins = Configuration.GetSection("AllowedCorsOrigins").Value.Split(";");
            services.AddCors(opt => opt.AddPolicy("CorsPolicy", builder => builder.WithOrigins(allowedCorsOrigins).AllowAnyMethod().AllowAnyHeader()));

            #region Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;


                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "Aa¡·BbCcsDdzEe…ÈFfGgyHhIiÕÌJjKkLlMmNnOo”Û÷ˆ’ıPpQqRrSTtUu⁄˙‹¸€˚VvWwXxYZ-. ";

            })
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders();
            #endregion

            #region JWT
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Default", cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetSection("Authentication")["JwtDefaultIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration.GetSection("Authentication")["JwtDefaultIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Authentication")["JwtKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                })
                ;
            #endregion

            #region Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireLogin", new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .AddAuthenticationSchemes("Default")
                     .Build());
            });
            #endregion

            services.AddAutoMapper(typeof(MapperProfiles));

            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Default")));

            #region Controllers
            services.AddControllers()
                    .AddFluentValidation(options =>
                    {
                        options.RegisterValidatorsFromAssemblyContaining(typeof(LoginDtoValidator));
                        options.RegisterValidatorsFromAssemblyContaining(typeof(RegisterDtoValidator));
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoDoge.Api", Version = "v1" });
                c.CustomOperationIds(e => GetOperationId(e));
                c.OperationFilter<AuthResponsesOperationFilter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            });
            #endregion

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddSingleton<ICryptoRandomGenerator, CryptoRandomGenerator>();

            services.AddTransient<IAuthAppService, AuthAppService>();
            services.AddTransient<ITokenAppService, TokenAppService>();
            services.AddTransient<IAuthRepository, AuthRepository>();


            services.AddScoped<IParserService, ParserService.ParserService>();

        }
        private string GetOperationId(ApiDescription e)
        {
            var controllerName = e.ActionDescriptor.RouteValues["controller"];

            if (e.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                return $"{controllerName}_{controllerActionDescriptor.MethodInfo.Name}";
            }

            else
            {
                var pathParts = e.RelativePath.Trim('/').Split('/').ToList();
                pathParts.RemoveAll(p => p.StartsWith('{'));
                return $"{controllerName}_{e.HttpMethod.ToLower()}_{pathParts.Last()}";
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoDoge.Server v1"));
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
