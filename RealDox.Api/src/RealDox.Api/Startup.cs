using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using RealDox.Api.Filters;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using RealDox.Api.Security;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using Microsoft.EntityFrameworkCore;
using RealDox.Core.Data.IdentityProviders;
using RealDox.Core.Data;
using RealDox.Core.Models;
using RealDox.Core.Interfaces;
using RealDox.Core.Data.Repositories;
using RealDox.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace RealDox.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment __hostingEnv;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            __hostingEnv = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtOptions = Configuration.GetSection("JwtSecurityToken").Get<JwtOptions>();
            //Support CORS (cross origin requests)
            //allowing other different-domain web apps do http requests to my web api
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            //DB contexts
            services.AddDbContext<ToDoContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:LocalDb"],

                    optionns =>
                    {
                        //Refer to: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                        optionns.EnableRetryOnFailure();
                        optionns.MigrationsAssembly("RealDox.Api");
                    })
            );

            services.AddDbContext<SecurityDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:LocalDb"],

                    optionns =>
                    {
                        //Refer to: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                        optionns.EnableRetryOnFailure();
                        optionns.MigrationsAssembly("RealDox.Api");
                    })
            );

            var defaultPolicy = new AuthorizationPolicyBuilder(new[]
            {
                /*IdentityConstants.ApplicationScheme,
                IdentityConstants.ExternalScheme,*/
                JwtBearerDefaults.AuthenticationScheme
            })
            .RequireAuthenticatedUser()
            .Build();


            services.AddMvc(options =>
            {
                //options.Filters.Add(new AuthorizeFilter(defaultPolicy));
                if (jwtOptions.UseCookie)
                {
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                }

                #region snippet_SSL 
                var skipSSL = Configuration.GetValue<bool>("LocalTest:skipSSL");
                // requires using Microsoft.AspNetCore.Mvc;

                // Set LocalTest:skipSSL to true to skip SSL requrement in 
                // debug mode. This is useful when not using Visual Studio.
                if (__hostingEnv.IsDevelopment() && !skipSSL)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }

                #endregion
            });



            services.AddRealDoxServices(Configuration);

            //Add MvcCore service
            //DuractionActionFilter is a service for the same as the Inline middleware bellow
            /*services.AddMvcCore(options =>
            {
                // Adds global filters.
                options.Filters.Add(new DurationActionFilter());
                // requires: using Microsoft.AspNetCore.Authorization;
                //           using Microsoft.AspNetCore.Mvc.Authorization;

                //We don't want unauthorized access to our Controllers's actions,
                //Because we are injecting this filter bellow, we dont have to use [Authorize] attribute in every Controller or Controller action
                //options.Filters.Add(new AuthorizeFilter(defaultPolicy));
                if (jwtOptions.UseCookie)
                {
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                }
            })
            .AddJsonFormatters()
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
            .AddApiExplorer();*/


            /*services.AddAuthentication()        
            .AddAzureAdB2CBearer(options => Configuration.Bind("AzureAdB2C", options));*/



            //Add the response compression service for better performance
            services.AddResponseCompression();

            /*In the ConfigureServices method of Startup.cs, 
            register the Swagger generator, defining one or more Swagger documents.*/
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "RealDox API - V1",
                    Version = "v1",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Alexander Martinez Fajardo", Email = "alexmtnezf@gmail.com", Url = "https://twitter.com/spboyer" },
                    License = new License { Name = "Use under LICX", Url = "https://example.com/license" }
                });

                c.SwaggerDoc("v2", new Info
                {
                    Title = "RealDox API - V2",
                    Version = "v2",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Alexander Martinez Fajardo", Email = "alexmtnezf@gmail.com", Url = "https://twitter.com/spboyer" },
                    License = new License { Name = "Use under LICX", Url = "https://example.com/license" }
                });

                // Set the comments path for the Swagger JSON and UI.

                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, "RealDox.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                        }

                        await context.Response.WriteAsync("There was an error");
                    });
                });
            }

            //Inline middleware to provide a header variable to see the performance of requests.
            app.Use(async (ncontext, next) =>
            {
                var watch = new Stopwatch();

                //To add Headers AFTER everything you need to do this, listening OnStarting event
                //in the response such as setting headers, status code, etc. You can't call next.Invoke after the response has been sent to the client. 
                ncontext.Response.OnStarting(state =>
                {
                    watch.Stop();

                    var httpContext = (HttpContext)state;
                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = watch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    httpContext.Response.Headers.Add("X-ElapsedTime", new[] { elapsedTime });
                    return Task.FromResult(0);
                }, ncontext);

                watch.Start();
                await next.Invoke();
            });

            app.UseStatusCodePages();

            app.UseStaticFiles(); // Add StaticFiles middleware. Return static files (js, css, html) and end pipeline.
            //In the Configure method, enable middleware to expose the generated Swagger as JSON endpoint(s)
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api-docs/{documentName}/swagger.json";
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);

                }
            ); //At this point, you can spin up your application and view the generated Swagger JSON at "/swagger/v1/swagger.json."

            /*Optionally insert the swagger-ui middleware if you want to expose interactive documentation,
            specifying the Swagger JSON endpoint(s) to power it from. */
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
            app.UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "RealDox API - V1 Docs");
                c.SwaggerEndpoint("/api-docs/v2/swagger.json", "RealDox API - V2 Docs");
                //Now you can restart your application and check out the auto-generated, interactive docs at "/swagger".
            });

            // Call UseAuthentication before calling UseMVCWithDefaultRoute or UseMVC.
            /*Although Identity middleware authenticates requests, authorization (and rejection) occurs only after MVC selects a specific Razor Page or controller and action. */
            #region snippet2
            app.UseAuthentication(); // Add Identity middleware for authenticate before you access secure resources.
            #endregion



            app.UseResponseCompression();
            app.UseMvcWithDefaultRoute(); // Add MVC middleware to the request pipeline.



        }
    }
}
