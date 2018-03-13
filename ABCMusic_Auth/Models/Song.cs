using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace hanna80_ABCMusic_Auth.Models
{
    public class Song
    {
		#region PROPERTIES
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int SongId { get; set; }

		[Required()]
		public string Name { get; set; }

		[Required()]
		public string ArtistName { get; set; }

		public string AlbumName { get; set; }

		public int? TrackNumber { get; set; }
		
		[Column(TypeName = "date")]
		public DateTime? ReleaseDate { get; set; }

		public string Publisher { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public ICollection<Review> Reviews { get; set; }
		#endregion
	}
}
