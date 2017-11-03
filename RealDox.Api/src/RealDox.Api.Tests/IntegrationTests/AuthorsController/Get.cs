using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RealDox.Core.Models;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.AuthorsController
{
    public class Get : AuthorsControllerTestBase
    {
        private readonly HttpClient _client;

        public Get()
        {
            _client = base.GetClient();
        }

        /*[Fact]
        public async Task ReturnHelloWorld()
        {
            // Act
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Hello World!",
                responseString);
        }*/
        

        [Theory]
        [InlineData("authors")]
        [InlineData("authors2")]
        public async Task ReturnsListOfAuthors(string controllerName)
        {
            var response = await _client.GetAsync($"/api/{controllerName}");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<Author>>(stringResponse).ToList();

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.Count(a => a.FullName == "Steve Smith"));
            Assert.Equal(1, result.Count(a => a.FullName == "Neil Gaiman"));
        }
    }
}
