using System;
using System.ComponentModel.DataAnnotations;
using ABCMusic_Auth.Models;

namespace ABCMusic_Auth
{
	public class CreateUserViewModel
	{
		public CreateUserViewModel() { }

		public CreateUserViewModel(ApplicationUser user)
		{
			this.UserName = user.UserName;
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
		}

		[Key]
		[Required]
		[Display(Name = "User Name")]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		
		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		
		[Required]
		public string Email { get; set; }
	}
}