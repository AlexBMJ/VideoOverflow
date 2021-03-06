using Server.Model;

namespace Server.Controllers
{
    /// <summary>
    /// Controller for the resourceRepository
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ResourceController : ControllerBase
    {
        private readonly ILogger<ResourceController> _logger;
        private readonly IResourceRepository _repository;
        private readonly ITagRepository _tagRepo;
        private readonly QueryParser _queryParser;
        private readonly SpellChecker _spellChecker;

        public ResourceController(ILogger<ResourceController> logger, IResourceRepository repository, ITagRepository tagRepository)
        {
            _logger = logger;
            _repository = repository;
            _tagRepo = tagRepository;
            _queryParser = new QueryParser(_tagRepo);
            _spellChecker = new SpellChecker();
        }

      
        /// <summary>
        /// Get all resources from the repository
        /// </summary>
        /// <returns>All resources from the repository</returns>
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ResourceDetailsDTO>> GetAll()
            => await _repository.GetAll();

        /// <summary>
        /// Gets all resources based on selected category, query, count and page
        /// </summary>
        /// <param name="Category">The selected category</param>
        /// <param name="Query">The users query</param>
        /// <param name="Count">Number of elements to be returned</param>
        /// <param name="Page">The page of a given list of results</param>
        /// <returns>All resources based on the parameters</returns>
        [AllowAnonymous]
        [HttpGet("Search")]
        public async Task<IEnumerable<ResourceDTO>> GetResources(int Category, string Query, int Count, int Page)
            => await _repository.GetResources(Category, Query, _queryParser.ParseTags(Query), Count, Math.Max(0, Count*(Page-1)));

        /// <summary>
        /// Gets a specific resource based on an id
        /// </summary>
        /// <param name="id">Id of the resource to get</param>
        /// <returns>The resource with specific id</returns>
        [AllowAnonymous]
        [HttpGet("Spelling")]
        public string SuggestSpelling(string Query)
            => _spellChecker.SpellCheck(Query);

        [Authorize]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ResourceDetailsDTO), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDetailsDTO>> Get(int id)
            => (await _repository.Get(id)).ToActionResult();

        
        /// <summary>
        /// Posts a resource to the repository
        /// </summary>
        /// <param name="resource">The resource to post</param>
        /// <returns>The action result of the push</returns>
        [Authorize(Roles = "developer")]
        [HttpPost]  
        [ProducesResponseType(typeof(Status), 201)]
        public async Task<IActionResult> Post(ResourceCreateDTO resource) 
           => (await _repository.Push(resource)).ToActionResult("Api/Resource/",resource);
        
        /// <summary>
        /// Updates a resource based on the input resource
        /// </summary>
        /// <param name="resource">The updated resource</param>
        /// <returns>The action result of the update</returns>
        [Authorize(Roles = "developer")]
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put([FromBody] ResourceUpdateDTO resource)
            => (await _repository.Update(resource)).ToActionResult();
        
        
        /// <summary>
        /// Deletes a resource based on the id of the resource
        /// </summary>
        /// <param name="id">The id of the resource</param>
        /// <returns>The action result of the delete</returns>
        [Authorize(Roles = "developer")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
            => (await _repository.Delete(id)).ToActionResult();
    }
}