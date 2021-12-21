namespace Server.Controllers;

/// <summary>
/// A controller for our category repository
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("Api/[controller]")]
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
    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<CategoryDTO>> GetAll()
        => await _repository.GetAll();
}