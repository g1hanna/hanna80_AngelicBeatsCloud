using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ABCMusic_Auth.Models
{
	public abstract class Reviewable
	{
		#region PROPERTIES
		public virtual int Id { get; set; }

		public virtual string ArtistId { get; set; }

		[Required()]
		[StringLength(100, ErrorMessage = "Song name cannot be more than 100 characters long.")]
		public virtual string Name { get; set; }

		[Required()]
		[Display(Name = "Artist Name")]
		public virtual string ArtistName { get; set; }

		[Column(TypeName = "date")]
		[Display(Name = "Release Date")]
		[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
		public virtual DateTime? ReleaseDate { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public virtual ApplicationUser Artist { get; set; }

		public virtual ICollection<ReviewableArtist> Contributors { get; set; }

		public virtual ICollection<Review> Reviews { get; set; }
		#endregion

		#region METHODS
		[NotMapped]
		[Display(Name = "Item Name")]
		public abstract string DisplayName { get; }
		#endregion
	}
}
