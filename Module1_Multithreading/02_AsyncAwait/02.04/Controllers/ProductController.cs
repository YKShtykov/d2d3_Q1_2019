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
        private static readonly EntityRepository _repository = new EntityRepository();

        [HttpGet]
        public async Task<IEnumerable<Entity>> GetAsync()
        {
            return await _repository.GetAsync();
        }

        [HttpPost]
        public async Task PostAsync([FromBody] Entity entity)
        {
            await _repository.PostAsync(entity);
        }

        [HttpPut]
        public async Task PutAsync([FromBody] Entity entity)
        {

            await _repository.PostAsync(entity);
        }

        [HttpDelete]
        public async Task DeleteAsync([FromBody]Entity entity)
        {
            await _repository.PostAsync(entity);
        }
    }
}
