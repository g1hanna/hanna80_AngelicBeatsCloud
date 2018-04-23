using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ABCMusic_Auth.Data;
using Microsoft.AspNetCore.Identity;
using ABCMusic_Auth.Models;
using ABCMusic_Auth.Models.AdminViewModels;
using Microsoft.EntityFrameworkCore;

namespace ABCMusic_Auth.Controllers
{
	public class UserRolesController : Controller
	{
		private readonly AngelicBeatsDbContext _dataContext;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		// add the db context and user manager through dependency injection
		public UserRolesController(
			AngelicBeatsDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_dataContext = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Index()
		{
			return View(buildRoleViewModelList(await _dataContext.Roles.ToListAsync()));
		}

		[ActionName("Details")]
		public async Task<IActionResult> DetailsAsync(string id)
		{
			if (id == null)
			{
				// return bad request object
				return new StatusCodeResult(400);
			}

			IdentityRole role = await _dataContext.Roles.FindAsync(id);

			if (role == null)
			{
				return NotFound();
			}
			
			return View(buildRoleViewModel(role));
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAsync([Bind("Id,Name")] IdentityRole role)
		{
			if (ModelState.IsValid)
			{
				await _roleManager.CreateAsync(role);
				await _dataContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			return View(role);
		}

		public IActionResult Edit(string id)
		{
			if (id == null)
			{
				return new StatusCodeResult(400);
			}

			IdentityRole role = _dataContext.Roles.Find(id);
			if (role == null)
			{
				return NotFound();
			}

			return View(buildRoleViewModel(role));
		}

		[HttpPost]
		[ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAsync([Bind("Id,Name")] IdentityRole roleModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var role = await _roleManager.FindByIdAsync(roleModel.Id);

					role.Name = roleModel.Name;

					await _roleManager.UpdateAsync(role);
					await _dataContext.SaveChangesAsync();
					return RedirectToAction("Index");
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				ViewBag.ErrorMessage = "An error occured while updating the role.";
			}

			return View(buildRoleViewModel(roleModel));
		}

		[HttpGet]
		public IActionResult Delete(string id)
		{
			if (id == null)
			{
				return new StatusCodeResult(400);
			}

			IdentityRole role = _dataContext.Roles.Find(id);

			if (role == null)
			{
				return NotFound();
			}

			return View(buildRoleViewModel(role));
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			IdentityRole identityRoleTemp = _dataContext.Roles.Find(id);

			// get users in the role and remove them from the role
			foreach (var user in await _userManager.GetUsersInRoleAsync(identityRoleTemp.Name))
			{
				await _userManager.RemoveFromRoleAsync(user, identityRoleTemp.Name);
			}

			await _roleManager.DeleteAsync(identityRoleTemp);
			await _dataContext.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		#region NONACTIONS
		[NonAction]
		private IEnumerable<RoleViewModel> buildRoleViewModelList(IEnumerable<IdentityRole> roles)
		{
			IList<RoleViewModel> roleViewModels = new List<RoleViewModel>();

			foreach (var role in roles)
			{
				RoleViewModel vm = new RoleViewModel()
				{
					Id = role.Id,
					Name = role.Name
				};

				roleViewModels.Add(vm);
			}

			return roleViewModels;
		}

		[NonAction]
		private RoleViewModel buildRoleViewModel(IdentityRole role)
		{
			return buildRoleViewModelList(new IdentityRole[1] { role }).ElementAt(0);
		}
		#endregion
	}
}