namespace Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ResourceController : ControllerBase
    {
        private readonly ILogger<ResourceController> _logger;
        private readonly IResourceRepository _repository;
        private readonly ITagRepository _tagRepo;
        private readonly QueryParser _queryParser;

        public ResourceController(ILogger<ResourceController> logger, IResourceRepository repository, ITagRepository tagRepository)
        {
            _logger = logger;
            _repository = repository;
            _tagRepo = tagRepository;
            _queryParser = new QueryParser(_tagRepo, _repository);
        }

      
        [HttpGet]
        public async Task<IEnumerable<ResourceDTO>> GetAll()
            => await _repository.GetAll();

        [Authorize]
        [HttpGet("Search")]
        public async Task<IEnumerable<ResourceDTO>> GetResources(int Category, string Query, int Count, int Page)
            => await _repository.GetResources(Category, Query, _queryParser.ParseTags(Query), Count, Math.Max(0, Count*(Page-1)));

        [Authorize]

        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ResourceDetailsDTO), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDetailsDTO>> Get(int id)
            => (await _repository.Get(id)).ToActionResult();

        
      
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