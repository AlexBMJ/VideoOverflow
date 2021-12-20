namespace Server.Controllers;

/// <summary>
/// A controller for our category repository
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly ICategoryRepository _repository;

    public CategoryController(ILogger<CategoryController> logger, ICategoryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    /// <summary>
    /// Gets all categories from the repository
    /// </summary>
    /// <returns>All categories from the repository</returns>
    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<CategoryDTO>> GetAll()
        => await _repository.GetAll();
}