using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.Functions.Core
{
    [Authorize]
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var job = await _jobService.FindAsync(id);
            
            if (job == null)
                return NotFound();

            return Ok(job);
        }
        
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string filter, [FromQuery]bool desc = true, [FromQuery]long? skipToken = null, [FromQuery]int limit = 10)
        {
            return Ok((await _jobService.GetAsync(filter, desc, skipToken, limit)).Select(job => new JobViewModel(job)));
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JobCreationViewModel model)
        {
            if (ModelState.IsValid == false)
                return BadRequest(model);
            
            var id = await _jobService.ExclusiveCreateAsync(model.Name, model.Parameters);

            if (id.HasValue == false)
                return StatusCode((int)HttpStatusCode.Conflict);

            return CreatedAtAction("Get", new { id });   
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _jobService.Cancel(id);
            return Ok();
        }
    }
}