using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABCMusic_Auth.Models;

namespace ABCMusic_Auth.Data
{
    public static class AngelicBeatsDbInitializer
    {
		public static void Initialize(AngelicBeatsDbContext context)
		{
			// //
			// // recreate database
			// // (commented out due to migrations)
			// //
			// //context.Database.EnsureDeleted();
			// //context.Database.EnsureCreated();

			// //
			// // Check if there are any songs
			// //
			// if (context.Users.Any() && context.Songs.Any())
			// {
			// 	return;
			// }

			// //
			// // seed starter users into the db
			// //
			// ApplicationUser[] users = new ApplicationUser[]
			// {
			// 	//new ApplicationUser { UserName = "DiscoDave", FirstName = "Dave", LastName = "Darrell", Age = 18, Gender = "M" }
			// };

			// foreach (ApplicationUser u in users) context.Users.Add(u);
			// context.SaveChanges();

			// //
			// // seed starter albums into the db
			// //
			// Album[] albums = new Album[]
			// {
			// 	new Album { Id = 1, Reviewable = new Reviewable { Name = "Frostfall", ArtistName = "Disco Dave",
			// 		ReleaseDate = DateTime.Parse("1/8/2017"), ReviewableArtists = new ReviewableArtist[0],
			// 		Reviews = new Review[0] }, Publisher = "" },
			// };

			// foreach (Album a in albums) context.Albums.Add(a);
			// context.SaveChanges();

			// //
			// // seed starter songs into the db
			// //
			// Song[] songs = new Song[]
			// {
			// 	new Song { Id = 1, Reviewable = new Reviewable { Name = "Carry On", ArtistName = "Disco Dave" }, TrackNumber = 2 },
			// 	new Song { Id = 2, Reviewable = new Reviewable { Name = "Black Ice", ArtistName = "Disco Dave" }, TrackNumber = 4 },
			// 	new Song { Id = 3, Reviewable = new Reviewable { Name = "Charismatic", ArtistName = "Disco Dave" }, TrackNumber = 5 }
			// };

			// foreach (Song s in songs) context.Songs.Add(s);
			// context.SaveChanges();

			// //
			// // seed starter reviews into the db
			// //
			// Review[] reviews = new Review[]
			// {

			// };

			// foreach (Review r in reviews) context.Reviews.Add(r);
			// context.SaveChanges();
		}
    }
}
