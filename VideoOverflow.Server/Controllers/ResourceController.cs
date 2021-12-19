namespace Server.Controllers
{

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

      
        [HttpGet]
        public async Task<IEnumerable<ResourceDTO>> GetAll()
            => await _repository.GetAll();

       
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ResourceDetailsDTO), 200)]
        [HttpGet("{id}")]
        public async Task<ResourceDetailsDTO> Get(int id)
            => (await _repository.Get(id));

        
      
        [HttpPost]  
        [ProducesResponseType(typeof(ResourceDTO), 201)]
        public async Task<IActionResult> Post(ResourceCreateDTO resource)
        {
            var created = await _repository.Push(resource);
    
            return CreatedAtAction(nameof(Get), new { created.Id }, created);
        }

      
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put([FromBody] ResourceUpdateDTO resource)
            => (await _repository.Update(resource)).ToActionResult();
        
    
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
            => (await _repository.Delete(id)).ToActionResult();
    }
}