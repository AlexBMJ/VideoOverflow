namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ResourceController : ControllerBase
    {
        private readonly ILogger<ResourceController> _logger;
        private readonly IResourceRepository _repository;

        public ResourceController(ILogger<ResourceController> logger, IResourceRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ResourceDTO>> GetAll()
            => await _repository.GetAll();

        [Authorize]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ResourceDetailsDTO), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDetailsDTO>> Get(int id)
            => (await _repository.Get(id)).ToActionResult();

        
        [Authorize(Roles = $"{Developer}")]
        [HttpPost]  
        [ProducesResponseType(typeof(ResourceDTO), 201)]
        public async Task<IActionResult> Post(ResourceCreateDTO resource)
        {
            var created = await _repository.Push(resource);
    
            return CreatedAtAction(nameof(Get), new { created.Id }, created);
        }

        [Authorize(Roles = "Developer")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put([FromBody] ResourceUpdateDTO resource)
            => (await _repository.Update(resource)).ToActionResult();
        
        [Authorize(Roles = $"{Developer}")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
            => (await _repository.Delete(id)).ToActionResult();
    }
}