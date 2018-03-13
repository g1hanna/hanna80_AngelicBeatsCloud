using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hanna80_ABCMusic_Auth.Models;

namespace hanna80_ABCMusic_Auth.Data
{
    public static class AngelicBeatsDbInitializer
    {
		public static void Initialize(AngelicBeatsDbContext context)
		{
			//
			// recreate database
			// (commented out due to migrations)
			//
			//context.Database.EnsureDeleted();
			//context.Database.EnsureCreated();

			//
			// Check if there are any songs
			//
			if (context.Songs.Any())
			{
				return;
			}

			//
			// seed starter songs into the db
			//
			Song[] songs = new Song[]
			{
				new Song { SongId = 1, Name = "Carry On", AlbumName = "Frostfall", ArtistName = "Disco Dave", TrackNumber = 2 },
				new Song { SongId = 2, Name = "Black Ice", AlbumName = "Frostfall", ArtistName = "Disco Dave", TrackNumber = 4 },
				new Song { SongId = 3, Name = "Charismatic", AlbumName = "Frostfall", ArtistName = "Disco Dave", TrackNumber = 5 }
			};

			foreach (Song s in songs)
				context.Songs.Add(s);

			context.SaveChanges();

			//
			// seed starter reviews into the db
			//
			Review[] reviews = new Review[]
			{

			};

			foreach (Review r in reviews) context.Reviews.Add(r);

			context.SaveChanges();
		}
    }
}
