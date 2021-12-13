namespace Server.Controllers;

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
    
    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<CategoryDTO>> GetAll()
        => await _repository.GetAll();
}