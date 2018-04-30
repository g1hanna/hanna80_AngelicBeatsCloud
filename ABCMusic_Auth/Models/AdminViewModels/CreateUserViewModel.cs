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

		[Required]
		public byte Age { get; set; }

		[Required]
		[StringLength(10)]
		public string Gender { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}