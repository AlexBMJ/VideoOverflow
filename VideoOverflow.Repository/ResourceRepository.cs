using VideoOverflow.Repository.Infrastructure.Context;

namespace VideoOverflow.Repository;

public class ResourceRepository
{
    private readonly VideoOverflowContext _context;

    public ResourceRepository(VideoOverflowContext context)
    {
        _context = context;
    }
}