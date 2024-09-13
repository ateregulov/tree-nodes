using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Threading.Tasks;

namespace TreesNodes.DAL
{
    public class MyContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbSet<Node> Nodes { get; set; }
        public DbSet<JournalItem> Journal { get; set; }

        public MyContext(DbContextOptions<MyContext> options, IConfiguration configuration)
        : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
