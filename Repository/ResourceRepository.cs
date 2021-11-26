using Repository.Infrastructure.Context;

namespace Repository.ResourceRepo;

public class ResourceRepository
{
    private readonly VideoOverflowContext _context;

    public ResourceRepository(VideoOverflowContext context)
    {
        _context = context;
    }
}