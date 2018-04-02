using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ABCMusic_Auth.Models
{
	public class ReviewableArtist
    {
		#region PROPERTIES
		public int ReviewableId { get; set; }

		public string ArtistId { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public Reviewable Reviewable { get; set; }

		public ApplicationUser Artist { get; set; }
		#endregion
	}
}