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
	}
}