using System.Collections.Generic;
using System.Threading.Tasks;
using RealDox.Core.Models;
using Microsoft.AspNetCore.Mvc;
using RealDox.Api.Filters;
using RealDox.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RealDox.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ValidateModel]
    public class Authors2Controller : Controller
    {
        private readonly IAuthorRepository _authorRepository;

        public Authors2Controller(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        // GET: api/authors2
        [HttpGet]
        public async Task<List<Author>> Get() => await _authorRepository.ListAsync();

        // GET api/authors2/5
        [HttpGet("{id}")]
        [ValidateAuthorExists]
        public async Task<IActionResult> Get(int id) => Ok(await _authorRepository.GetByIdAsync(id));

        // POST api/authors2
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Author author)
        {
            await _authorRepository.AddAsync(author);
            return Ok(author);
        }

        // PUT api/authors2/5
        [HttpPut("{id}")]
        [ValidateAuthorExists]
        public async Task<IActionResult> Put(int id, [FromBody]Author author)
        {
            await _authorRepository.UpdateAsync(author);
            return Ok();
        }

        // DELETE api/authors2/5
        [HttpDelete("{id}")]
        [ValidateAuthorExists]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorRepository.DeleteAsync(id);
            return Ok();
        }
    }
}