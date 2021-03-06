﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ABCMusic_Auth.Models
{
    public class Song : Reviewable
    {
		#region PROPERTIES
		public TimeSpan Length { get; set; }

		public string Publisher { get; set; }

		[Display(Name = "Album")]
		public int? AlbumId { get; set; }

		[Display(Name = "Track Number")]
		[Range(0, int.MaxValue, ErrorMessage = "Only non-negative numbers are allowed for track numbers.")]
		public int? TrackNumber { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public Album Album { get; set; }
		#endregion

		#region METHODS
		[NotMapped]
		public override string DisplayName
		{
			get {
				string displayName = $"Song \"{this.Name}\"";

				if (Artist != null) displayName += $" by {this.Artist.UserName}";
				if (Album != null) displayName += $" from {this.Album.Name}";

				return displayName;
			}
		}
		#endregion
	}
}
