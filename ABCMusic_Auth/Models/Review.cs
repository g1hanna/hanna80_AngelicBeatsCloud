using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ABCMusic_Auth.Models
{
	public class Review
	{
		#region PROPERTIES
		public int ReviewId { get; set; }

		[DataType("VARCHAR(120)")]
		public string Subject { get; set; }

		[Range(0, 5)]
		public byte Rating { get; set; }

		[DataType("NVARCHAR(MAX)")]
		public string Content { get; set; }

		[Display(Name = "Reviewable")]
		public int ReviewableId { get; set; }

		[DataType("NVARCHAR(128)")]
		[Display(Name = "Author")]
		public string AuthorId { get; set; }
		#endregion

		#region NAVIGATION PROPERTIES
		public Reviewable Reviewable { get; set; }

		public ApplicationUser Author { get; set; }
		#endregion
	}
}
