using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ABCMusic_Auth.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		#region PROPERTIES
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public byte Age { get; set; }
		public string Gender { get; set; }

		[Display(Name = "User Name")]
		public override string UserName { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public ICollection<ReviewableArtist> ReviewableArtists { get; set; }

		public ICollection<Review> Reviews { get; set; }

		public ICollection<Reviewable> Reviewables { get; set; }

		// [NotMapped]
		// public IEnumerable<Song> Songs
		// {
		// 	get
		// 	{
		// 		// query over reviewables; if null or empty, query over empty list
		// 		// cast each item in the query into song objects
		// 		return (Reviewables ?? new Reviewable[0]).Where(r => r is Song).Cast<Song>();
		// 	}
		// }

		// [NotMapped]
		// public IEnumerable<Album> Albums
		// {
		// 	get
		// 	{
		// 		// query over reviewables; if null or empty, query over empty list
		// 		// cast each item in the query into album objects
		// 		return (Reviewables ?? new Reviewable[0]).Where(r => r is Album).Cast<Album>();
		// 	}
		// }
		#endregion

		public string GetName()
		{
			// <First Name> <Last Name>
			if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName))
				return FirstName + " " + LastName;
			// <First Name>
			else if (!string.IsNullOrEmpty(FirstName))
				return FirstName;
			// <User Name>
			else
				return UserName;
		}
	}
}
