using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ABCMusic_Auth.Models;

namespace ABCMusic_Auth.Data
{
	public class AngelicBeatsDbContext : IdentityDbContext<ApplicationUser>
	{
		public AngelicBeatsDbContext(DbContextOptions<AngelicBeatsDbContext> options)
			: base(options)
		{
		}

		public DbSet<Song> Songs { get; set; }
		public DbSet<Album> Albums { get; set; }
		public DbSet<Review> Reviews { get; set; }
		//public DbSet<Reviewable> Reviewables { get; set; }
		public DbSet<ReviewableArtist> ReviewableArtists { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			// use Fluent API to create tables and relationships

			// === create tables ===
			builder.Entity<Review>().ToTable("Review");
			//builder.Entity<Reviewable>().ToTable("Reviewable");
			builder.Entity<Song>().ToTable("Song");
			builder.Entity<Album>().ToTable("Album");
			builder.Entity<ReviewableArtist>().ToTable("ReviewableArtist");

			// configure base types
			// 1) Song : Reviewable
			//builder.Entity<Song>().HasBaseType<Reviewable>();
			// 2) Album : Reviewable
			//builder.Entity<Album>().HasBaseType<Reviewable>();

			// // === ignore converted properties ===
			// //-Application User
			// builder.Entity<ApplicationUser>().Ignore(au => au.Songs);
			// builder.Entity<ApplicationUser>().Ignore(au => au.Albums);
			// //-Review
			// builder.Entity<Review>().Ignore(r => r.Song);
			// builder.Entity<Review>().Ignore(r => r.Album);
			// //-Song
			// builder.Entity<Song>().Ignore(s => s.Albums);

			// === set primary keys ===
			builder.Entity<Review>().HasKey(r => r.ReviewId);
			builder.Entity<Reviewable>().HasKey(r => r.Id);
			// builder.Entity<Song>().HasKey(s => s.Id);
			builder.Entity<Song>().HasBaseType<Reviewable>();
			// builder.Entity<Album>().HasKey(a => a.Id);
			builder.Entity<Album>().HasBaseType<Reviewable>();
			builder.Entity<ReviewableArtist>().HasKey(ra => new { ra.ReviewableId, ra.ArtistId });

			builder.Entity<Review>().Property(r => r.ReviewId).ValueGeneratedOnAdd();
			builder.Entity<Reviewable>().Property(r => r.Id).ValueGeneratedOnAdd();
			// builder.Entity<Song>().Property(s => s.Id).ValueGeneratedNever();
			// builder.Entity<Album>().Property(a => a.Id).ValueGeneratedNever();
			builder.Entity<ReviewableArtist>().Property(ra => ra.ReviewableId).ValueGeneratedNever();
			builder.Entity<ReviewableArtist>().Property(ra => ra.ArtistId).ValueGeneratedNever();

			// === set foreign keys and navigation properties ===
			// // Song to Reviewable (1:1)
			// builder.Entity<Song>()
			// 	.HasOne(s => s.Reviewable)
			// 	.WithOne(r => r.Song)
			// 	.HasForeignKey(typeof(Song), nameof(Song.Id));
			// // Album to Reviewable (1:1)
			// builder.Entity<Album>()
			// 	.HasOne(a => a.Reviewable)
			// 	.WithOne(r => r.Album)
			// 	.HasForeignKey(typeof(Album), nameof(Album.Id));
			// --- Review to Reviewable (1:M) ---
			builder.Entity<Review>()
				.HasOne(r => r.Reviewable)
				.WithMany(r => r.Reviews)
				.HasForeignKey(r => r.ReviewableId)
				.OnDelete(DeleteBehavior.Restrict);
			// --- Review to ApplicationUser (1:M) ---
			builder.Entity<Review>()
				.HasOne(r => r.Author)
				.WithMany(au => au.Reviews)
				.HasForeignKey(r => r.AuthorId)
				.OnDelete(DeleteBehavior.Restrict);
			// --- ApplicationUser (Artist) to Reviewable (1:M)
			builder.Entity<Reviewable>()
				.HasOne(r => r.Artist)
				.WithMany(au => au.Reviewables)
				.HasForeignKey(r => r.ArtistId);
			// --- ApplicationUser to Reviewable (M:M) ---
			// 1) ApplicationUser to ReviewableArtist (1:M)
			builder.Entity<ReviewableArtist>()
				.HasOne(ra => ra.Artist)
				.WithMany(a => a.ReviewableArtists)
				.HasForeignKey(ra => ra.ArtistId)
				.OnDelete(DeleteBehavior.Restrict);
			// 2) Reviewable to ReviewableArtist (1:M)
			builder.Entity<ReviewableArtist>()
				.HasOne(ra => ra.Reviewable)
				.WithMany(r => r.Contributors)
				.HasForeignKey(ra => ra.ReviewableId)
				.OnDelete(DeleteBehavior.Restrict);
			// --- Album to Song (1:M) ---
			builder.Entity<Album>()
				.HasMany(a => a.Songs)
				.WithOne(s => s.Album)
				.HasForeignKey(s => s.AlbumId);

			// make an index for an application user
			builder.Entity<ApplicationUser>()
				.HasIndex(au => au.UserName)
				.IsUnique(true);
		}
	}
}
