using System;
using System.ComponentModel.DataAnnotations;

namespace ABCMusic_Auth.Models.AdminViewModels
{
	public class RoleViewModel
	{
		public RoleViewModel() {}

		[Key]
		public string Id { get; set; }

		[Required]
		[Display(Name = "Role Name")]
		public string Name { get; set; }
	}
}