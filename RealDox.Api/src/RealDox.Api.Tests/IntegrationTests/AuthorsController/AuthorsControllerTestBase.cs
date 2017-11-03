using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using RealDox.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IntegrationTests.AuthorsController
{
    public class AuthorsControllerTestBase
    {
        
        protected HttpClient GetClient()
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseStartup<Startup>();
                
            var server = new TestServer(builder);
            var client = server.CreateClient();
            client.BaseAddress = new Uri("http://localhost");
            
            /*Bellow property is used to specify the maximum number of bytes to buffer
            when reading the content in the HTTP response message.
            The default size of this property is the maximum size of an integer.
            Therefore, the property is set to a smaller value, as a safeguard,
            in order to limit the amount of data that the application will accept as a response
            from the web service. */
            //client.MaxResponseContentBufferSize = 256000;

            // client always expects json results
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        /*protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;

            // Inject a custom application part manager. 
            // Overrides AddMvcCore() because it uses TryAdd().
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }*/
    }
}