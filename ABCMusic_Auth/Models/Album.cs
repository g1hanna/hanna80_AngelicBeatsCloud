using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ABCMusic_Auth.Models
{
    public class Album : Reviewable
    {
		#region PROPERTIES
		public string Publisher { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public ICollection<Song> Songs { get; set; }
		#endregion

		#region METHODS
		[NotMapped]
		public override string DisplayName
		{
			get {
				string displayName = $"Album \"{this.Name}\"";

				if (Artist != null) displayName += $" by {this.Artist.UserName}";

				return displayName;
			}
		}
		#endregion
	}
}