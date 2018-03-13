using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using hanna80_ABCMusic_Auth.Models;

namespace hanna80_ABCMusic_Auth.Data
{
    public class AngelicBeatsDbContext : IdentityDbContext<ApplicationUser>
    {
        public AngelicBeatsDbContext(DbContextOptions<AngelicBeatsDbContext> options)
            : base(options)
        {
        }

		public DbSet<Song> Songs { get; set; }
		public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			builder.Entity<Song>().ToTable("Song");
			builder.Entity<Review>().ToTable("Review");
        }
    }
}
