using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _02._04.Controllers
{
    using Model;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly EntityRepository _repository = new EntityRepository();

        // GET: api/Product
        [HttpGet]
        public async Task<IEnumerable<Entity>> GetAsync()
        {
            return await _repository.GetAsync();
        }

        // POST: api/Product
        [HttpPost]
        public async Task PostAsync([FromBody] Entity entity)
        {
            await _repository.PostAsync(entity);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task PutAsync([FromBody] Entity entity)
        {

            await _repository.PostAsync(entity);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(Entity entity)
        {
            await _repository.PostAsync(entity);
        }
    }
}
