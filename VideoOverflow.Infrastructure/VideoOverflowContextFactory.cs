using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VideoOverflow.Infrastructure;


    public class VideoOverflowContextFactory : IDesignTimeDbContextFactory<VideoOverflowContext>
    {
        public VideoOverflowContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>()
                .Build();

            var connectionString = configuration.GetConnectionString("VideoOverflow");
            
            var optionsBuilder = new DbContextOptionsBuilder<VideoOverflowContext>()
                .UseNpgsql(connectionString);

            return new VideoOverflowContext(optionsBuilder.Options);
            
        }
    }
