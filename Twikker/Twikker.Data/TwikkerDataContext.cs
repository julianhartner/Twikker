using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Twikker.Data.Models;

namespace Twikker.Data
{
    public class TwikkerDataContext : IdentityDbContext<ApplicationUser>
    {
        public TwikkerDataContext()
        {
        }

        public TwikkerDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TwikkerPost> Posts { get; set; }
        public DbSet<TwikkerComment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TwikkerPost>().ToTable("Post");
            builder.Entity<TwikkerComment>().ToTable("Comment");
            builder.Entity<PostLike>().ToTable("PostLike");
            builder.Entity<CommentLike>().ToTable("CommentLike");

            base.OnModelCreating(builder);
        }
    }
}