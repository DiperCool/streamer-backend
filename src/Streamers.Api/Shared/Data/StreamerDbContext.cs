using Microsoft.EntityFrameworkCore;
using Streamers.Api.Streamers.Models;

namespace Streamers.Api.Shared.Data;

public class StreamerDbContext : DbContext
{
    public DbSet<Streamer> Streamers { get; set; }
    public StreamerDbContext(DbContextOptions<StreamerDbContext> options, IConfiguration configuration) : base(options)
    {
    }

}
