﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VideoOverflow.Infrastructure;

public class VideoOverflowContextFactory
{
    public class Builer : IDesignTimeDbContextFactory<VideoOverflowContext>
    {
        public VideoOverflowContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VideoOverflowContext>();
            optionsBuilder.UseNpgsql("VideoOverflow");

            return new VideoOverflowContext(optionsBuilder.Options);
        }
    }
}