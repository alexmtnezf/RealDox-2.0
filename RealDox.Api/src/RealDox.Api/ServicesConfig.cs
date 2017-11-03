using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealDox.Core.Data;
using RealDox.Core.Models;
using RealDox.Core.Data.IdentityProviders;
using RealDox.Api.Security;
using Microsoft.EntityFrameworkCore;
using RealDox.Core.Interfaces;
using RealDox.Api.Filters;
using RealDox.Core.Data.Repositories;
using RealDox.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealDox.Api
{
    public static class ServicesConfig
    {
        public static IServiceCollection AddRealDoxServices(this IServiceCollection services, IConfiguration config)
        {
            //
            // Configuration.
            //
            services.AddOptions();
            services.Configure<JwtOptions>(config.GetSection("JwtSecurityToken"));

            //
            // Security.
            //
            // We use token (JWT) based authentication here.
            // For a better understanding regarding token authentication, see below page.
            // https://stormpath.com/blog/token-authentication-asp-net-core
            //

            // Turn off Microsoft's JWT handler that maps claim types to .NET's long claim type names
            // See more details at https://github.com/IdentityServer/IdentityServer3.Samples/issues/173
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var jwtOptions = config.GetSection("JwtSecurityToken").Get<JwtOptions>();
            var invalidTokenDictionary = new InvalidTokenDictionary();
            var jwtValidator = new JwtValidator(jwtOptions.SigningAlgorithm, invalidTokenDictionary);
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Check if the token is issued by us.
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                // Check if the token is expired.
                ValidateLifetime = true, //validate the expiration and not before values in the token
                // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                // machines which should have synchronized time, this can be set to zero. and default value will be 5minutes
                ClockSkew = TimeSpan.Zero,
            };

            // For demo purpose, we will show two ways to store the token: cookie or HTTP Authorization Header.
            // We will use these two authentication schemes on different Controllers.

            // Note: AddIdentity() will enable Cookies as default authentication scheme.
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<SecurityDbContext>() // Switches between EF and MongoDB with below line.
                                                               //.AddMongoDbStores(options => options.ConnectionString = config.GetConnectionString("Mongo"))
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => {
                // Prevents cookies from client script access. So we are safe for XSS attack.
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = jwtOptions.CookieName;
                options.Cookie.Domain = jwtOptions.DomainName;
                options.Cookie.Path = jwtOptions.CookiePath;
                //options.Cookie.Expiration = new DateTimeOffset(DateTime.Now.AddDays(jwtOptions.RememberMeExpireInDays)).To(), //TimeSpan.FromMinutes(jwtOptions.ExpireInMinutes);
                options.Cookie.SameSite = SameSiteMode.Strict;
                // Tells system how to verify.
                //options.TicketDataFormat = new JwtSecureDataFormat(tokenValidationParameters, jwtValidator);
                // Stops redirections for api.
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied),
                    OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin),
                };
                options.SlidingExpiration = true;
            });

            /*Reference this package in project.json: Microsoft.AspNetCore.Identity.MongoDB
            Then, in ConfigureServices--or wherever you are registering services--include the following to register both the Identity services and MongoDB stores:*/
            //services.AddIdentityWithMongoStores("mongodb://localhost/myDB");
            /*services.AddIdentityWithMongoStoresUsingCustomTypes<ApplicationUser, ApplicationRole>(config.GetConnectionString("Mongo"))
                .AddDefaultTokenProviders();*/

            // Since AddIdentity() has already added cookies, here we only change some settings.
            // Check below page for details on how to add cookie authentication w/ or w/o Identity framework.
            // https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x
            services.AddAuthentication() 
                /*.AddCookie(options => {
                // Prevents cookies from client script access. So we are safe for XSS attack.
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = jwtOptions.CookieName;
                options.Cookie.Domain = jwtOptions.DomainName;
                options.Cookie.Path = jwtOptions.CookiePath;
                //options.Cookie.Expiration = new DateTimeOffset(DateTime.Now.AddDays(jwtOptions.RememberMeExpireInDays)).To(), //TimeSpan.FromMinutes(jwtOptions.ExpireInMinutes);
                options.Cookie.SameSite = SameSiteMode.Strict;
                // Tells system how to verify.
                //options.TicketDataFormat = new JwtSecureDataFormat(tokenValidationParameters, jwtValidator);
                // Stops redirections for api.
                options.Events = new CookieAuthenticationEvents  
                {
                    OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied),
                    OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin),
                };
                options.SlidingExpiration = true;
            })*/
            
            // Adds authentication via Jwt to validate token.
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false; // Todo: remove this line in production.
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
                // Since we have custom validation logics so remove the default validator.
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(jwtValidator);
            });

            Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(
                HttpStatusCode statusCode,
                Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
                context =>
                {
                    // Does not touch non-api calls.
                    if (!context.Request.Path.StartsWithSegments("/api")) return existingRedirector(context);

                    // No direction and directly return code.
                    context.Response.StatusCode = (int)statusCode;
                    return Task.CompletedTask;
                };

            // Adds token black list, used to revoke or logout.
            // Todo: need a background worker to clean this list to remove tokens that already expired.
            services.AddSingleton(invalidTokenDictionary);
            services.AddSingleton(jwtValidator);
            
            // Custom Policy-Based Authorization.
            services.AddAuthorization(options =>
            {
                
                options.AddPolicy(AuthorizationPolicies.Admin,
                    builder => builder.RequireClaim(CustomClaimTypes.Operator).RequireClaim(CustomClaimTypes.User));

                options.AddPolicy("DisneyUser",
                          policy => policy.RequireClaim("DisneyCharacter", "IAmMickey"));

            });




            //Repository and InitofWork, and other application services
            //Adding repositories and UnitofWork
            services.AddScoped<IToDoRepository, ToDoRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            //Add application services            
            services.AddTransient<RandomNumberProviderFilter>();
            services.AddTransient<RandomNumberService>();
            
            services.AddTransient<IEmailSender, EmailSender>();

            //services.AddScoped<ISeedDataService, SeedDataService>();


            return services;
        }
    }
}