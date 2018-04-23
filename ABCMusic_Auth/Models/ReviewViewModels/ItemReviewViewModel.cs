using System;
using ABCMusic_Auth.Models;
using ABCMusic_Auth.Data;
using System.ComponentModel.DataAnnotations;

namespace ABCMusic_Auth.Models.ReviewViewModels
{
	public class ItemReviewViewModel
	{
		public int Id { get; set; }
		
		[Required]
		public string Subject { get; set; }

		[Required]
		[Range(0, 5)]
		public byte Rating { get; set; }

		public string Content { get; set; }

		public string AuthorId { get; set; }

		[Display(Name = "Author")]
		public string AuthorUserName { get; set; }

		public bool ReviewableIsAlbum { get; set; }

		[Display(Name = "Item")]
		public int ReviewableId { get; set; }

		[Display(Name = "Item Name")]
		public string ReviewableName { get; set; }
	}
}