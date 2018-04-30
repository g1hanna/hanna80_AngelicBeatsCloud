using System.ComponentModel.DataAnnotations;

namespace ABCMusic_Auth.Models.AdminViewModels
{
	public class EditUserViewModel
	{
		public EditUserViewModel() {}

		// allow initialization with an instance of application user:
		public EditUserViewModel(ApplicationUser user)
		{
			this.UserName = user.UserName;
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.Age = user.Age;
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
		// Source: regexlib.com/Search.aspx?k=email
		//[RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$")]
		[EmailAddress]
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