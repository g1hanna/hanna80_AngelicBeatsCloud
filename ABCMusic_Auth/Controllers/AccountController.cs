using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ABCMusic_Auth.Models;
using ABCMusic_Auth.Models.AccountViewModels;
using ABCMusic_Auth.Services;
using ABCMusic_Auth.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ABCMusic_Auth.Models.AdminViewModels;

namespace ABCMusic_Auth.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller
	{
		#region INJECTED RESOURCES
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly ILogger _logger;
		private readonly AngelicBeatsDbContext _dataContext;
		#endregion

		public AccountController(
			AngelicBeatsDbContext dataContext,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailSender emailSender,
			ILogger<AccountController> logger
		)
		{
			_dataContext = dataContext;
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_logger = logger;
		}

		[TempData]
		public string ErrorMessage { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					return RedirectToLocal(returnUrl);
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToAction(nameof(Lockout));
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			var model = new LoginWith2faViewModel { RememberMe = rememberMe };
			ViewData["ReturnUrl"] = returnUrl;

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

			var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

			if (result.Succeeded)
			{
				_logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
				return RedirectToLocal(returnUrl);
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
				return RedirectToAction(nameof(Lockout));
			}
			else
			{
				_logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
				ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
				return View();
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
		{
			// Ensure the user has gone through the username & password screen first
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			ViewData["ReturnUrl"] = returnUrl;

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				throw new ApplicationException($"Unable to load two-factor authentication user.");
			}

			var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

			var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

			if (result.Succeeded)
			{
				_logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
				return RedirectToLocal(returnUrl);
			}
			if (result.IsLockedOut)
			{
				_logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
				return RedirectToAction(nameof(Lockout));
			}
			else
			{
				_logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
				ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
				return View();
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Lockout()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email,
					FirstName = model.FirstName, LastName = model.LastName, Age = (byte)model.Age,
					Gender = model.Gender };
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
					await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

					await _signInManager.SignInAsync(user, isPersistent: false);
					_logger.LogInformation("User created a new account with password.");
					return RedirectToLocal(returnUrl);
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult ExternalLogin(string provider, string returnUrl = null)
		{
			// Request a redirect to the external login provider.
			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
		{
			if (remoteError != null)
			{
				ErrorMessage = $"Error from external provider: {remoteError}";
				return RedirectToAction(nameof(Login));
			}
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction(nameof(Login));
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
				return RedirectToLocal(returnUrl);
			}
			if (result.IsLockedOut)
			{
				return RedirectToAction(nameof(Lockout));
			}
			else
			{
				// If the user does not have an account, then ask the user to create an account.
				ViewData["ReturnUrl"] = returnUrl;
				ViewData["LoginProvider"] = info.LoginProvider;
				var email = info.Principal.FindFirstValue(ClaimTypes.Email);
				return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await _signInManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					throw new ApplicationException("Error loading external login information during confirmation.");
				}
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await _userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewData["ReturnUrl"] = returnUrl;
			return View(nameof(ExternalLogin), model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{userId}'.");
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return RedirectToAction(nameof(ForgotPasswordConfirmation));
				}

				// For more information on how to enable account confirmation and password reset please
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
				await _emailSender.SendEmailAsync(model.Email, "Reset Password",
				   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string code = null)
		{
			if (code == null)
			{
				throw new ApplicationException("A code must be supplied for password reset.");
			}
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			AddErrors(result);
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}


		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

		#endregion

		#region USER MANAGEMENT
		//[Authorize(Roles = "Admin")]
		[ActionName("Index")]
		public async Task<IActionResult> IndexAsync()
		{
			var users = await _userManager.Users.ToListAsync();
			var model = new List<EditUserViewModel>();

			foreach (var user in users)
			{
				var u = new EditUserViewModel(user);
				model.Add(u);
			}

			return View(model);
		}

		//[Authorize(Roles = "Admin")]
		[HttpGet]
		[ActionName("Details")]
		public async Task<IActionResult> DetailsAsync(string userName = null)
		{
			if (userName == null)
			{
				return NotFound();
			}

			ApplicationUser user = await _userManager.FindByNameAsync(userName);
			
			if (user == null)
			{
				return NotFound();
			}

			var model = new EditUserViewModel(user);
			return View(model);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Create")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateAsync([Bind("UserName,LastName,FirstName,Email,Password,ConfirmPassword,Age,Gender")] CreateUserViewModel user)
		{
			string userName = User.Identity.Name;
			if (userName == null) { return StatusCode(400); }

			// // validate user's authority to create user
			// // only admin allowed
			// ApplicationUser currentUser = await _userManager.FindByNameAsync(userName);
			// if (await _userManager.IsInRoleAsync(currentUser, "Admin")) {}
			// else
			// {
			// 	return RedirectToAction("AccessDenied", "Error");
			// }

			if (ModelState.IsValid)
			{
				var newUser = new ApplicationUser()
				{
					UserName = user.UserName,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					Age = user.Age,
					Gender = user.Gender
				};

				var result = await _userManager.CreateAsync(newUser);
				if (result.Succeeded)
				{
					PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
					newUser.PasswordHash = ph.HashPassword(newUser, user.Password);
					await _dataContext.SaveChangesAsync();
					return RedirectToAction("Index");
				}

				AddErrors(result);
			}
			
			return View(user);
		}

		[HttpGet]
		[ActionName("Edit")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> EditAsync(string userName = null)
		{
			if (userName == null)
			{
				return new StatusCodeResult(400);
			}

			ApplicationUser user = await _userManager.FindByNameAsync(userName);

			if (user == null)
			{
				return NotFound();
			}

			var model = new EditUserViewModel(user);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Edit")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> EditAsync([Bind("UserName,LastName,FirstName,Email,Password,ConfirmPassword,Age,Gender")] EditUserViewModel userModel)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = await _userManager.FindByNameAsync(userModel.UserName);

				if (user != null)
				{
					user.FirstName = userModel.FirstName;
					user.LastName = userModel.LastName;
					user.Email = userModel.Email;
					user.Age = userModel.Age;
					user.Gender = userModel.Gender;

					if (userModel.Password != null)
					{
						PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
						user.PasswordHash = ph.HashPassword(user, userModel.Password);
					}
				}

				await _userManager.UpdateAsync(user);
				await _dataContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			return View(userModel);
		}

		[HttpGet]
		[ActionName("Delete")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteAsync(string userName = null)
		{
			if (userName == null)
			{
				return StatusCode(400);
			}

			ApplicationUser user = await _userManager.FindByNameAsync(userName);
			if (user == null)
			{
				return NotFound();
			}

			bool isCurrentUser = user.UserName == User.Identity.Name;
			ViewBag.IsCurrentUser = isCurrentUser;

			var model = new EditUserViewModel(user);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmedAsync(string userName)
		{
			//ApplicationUser user = await _userManager.FindByNameAsync(userName);
			ApplicationUser user = await _dataContext.Users
				.Include(u => u.Reviews)
				.Include(u => u.Reviewables)
				.FirstOrDefaultAsync(u => u.UserName == userName);

			// remove user from roles
			await _userManager.RemoveFromRolesAsync(user, _roleManager.Roles.Select(r => r.Name));

			// remove user's songs, albums, and reviews
			if (user.Reviews.Count > 0) {
				_dataContext.Reviews.RemoveRange(user.Reviews);
				await _dataContext.SaveChangesAsync();
			}

			// remove reviewables
			if (user.Reviewables.Count > 0) {
				// load songs in separate collection
				ICollection<Song> songs = new List<Song>();
				foreach (Reviewable r2 in user.Reviewables.Where(r => r is Song))
					songs.Add(await _dataContext.Songs
						.Include(_ => _.Album)
						.Include(_ => _.Reviews)
						.FirstOrDefaultAsync(_ => _.Id == r2.Id));

				// handle songs and related data
				foreach (Song s in songs)
				{
					// remove reviews
					// TODO: Implement model and detach reviews
					if (s.Reviews.Count > 0) {
						_dataContext.Reviews.RemoveRange(s.Reviews);
					}

					// remove song
					_dataContext.Songs.Remove(s);
				}
				await _dataContext.SaveChangesAsync();

				// load albums in separate collection
				ICollection<Album> albums = new List<Album>();
				foreach (Reviewable r1 in user.Reviewables.Where(r => r is Album))
					albums.Add(await _dataContext.Albums
						.Include(_ => _.Songs)
						.Include(_ => _.Reviews)
						.FirstOrDefaultAsync(_ => _.Id == r1.Id));

				// handle albums and related data
				foreach (Album a in albums)
				{
					// remove reviews
					// TODO: Implement model and detach reviews
					if (a.Reviews.Count > 0) {
						_dataContext.Reviews.RemoveRange(a.Reviews);
						//await _dataContext.SaveChangesAsync();
					}
					
					// detach songs
					if (a.Songs.Count > 0) {
						foreach (Song item in a.Songs) { item.AlbumId = null; item.Album = null; }
						_dataContext.Songs.UpdateRange(a.Songs);
						//await _dataContext.SaveChangesAsync();
					}

					// remove album
					_dataContext.Albums.Remove(a);
				}
				await _dataContext.SaveChangesAsync();
			}

			// remove user via user manager
			await _userManager.DeleteAsync(user);
			await _dataContext.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		#endregion

		#region USER ROLE MANAGEMENT
		[ActionName("ViewUsersRoles")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> ViewUsersRolesAsync(string userName = null)
		{
			if (!string.IsNullOrWhiteSpace(userName))
			{
				List<string> userRoles;

				// SEE: Resources stored in class vs. Resources being generated
				// var roleStore = new RoleStore<IdentityRole>(_dataContext);
				// var roleManager = new RoleManager<IdentityRole>(roleStore);

				// var userStore = new UserStore<ApplicationUser>(_dataContext);
				// var userManager = new UserManager<ApplicationUser>(userStore);

				var user = await _userManager.FindByNameAsync(userName);
				if (user == null)
					throw new Exception("User not found!");

				var userRoleNames = (from r in await _userManager.GetRolesAsync(user) select r);
				// userRoles = (from id in userRoleIds
				//                 let r = await _roleManager.FindByIdAsync(id)
				//                 select r).ToList();

				userRoles = new List<string>();
				foreach (var name in userRoleNames)
				{
					var r = await _roleManager.FindByNameAsync(name);
					userRoles.Add(r.Name);
				}

				ViewBag.UserName = userName;
				ViewBag.RolesForUser = userRoles;
			}
			return View();
		}

		[ActionName("AddRoleToUser")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AddRoleToUserAsync(string userName = null)
		{
			List<string> roles;

			// var roleStore = new RoleStore<IdentityRole>(_dataContext);
			// var roleManager = new RoleManager<IdentityRole>(roleStore);

			roles = await (from r in _roleManager.Roles select r.Name).ToListAsync();

			ApplicationUser user = await _userManager.FindByNameAsync(userName);
			if (user == null) return StatusCode(400);

			List<string> availableRoles = new List<string>(roles.Count);

			// skip roles the user already has
			foreach (var role in roles)
			{
				if (!await _userManager.IsInRoleAsync(user, role))
				{
					availableRoles.Add(role);
				}
			}

			ViewBag.Roles = new SelectList(availableRoles);
			ViewBag.UserName = userName;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("AddRoleToUser")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AddRoleToUserAsync(string roleName, string userName)
		{
			List<string> roles;

			// var roleStore = new RoleStore<IdentityRole>(_dataContext);
			// var roleManager = new RoleManager<IdentityRole>(roleStore);

			// var userStore = new UserStore<ApplicationUser>(_dataContext);
			// var userManager = new UserManager<ApplicationUser>(userStore);

			var user = await _userManager.FindByNameAsync(userName);
			if (user == null)
				throw new Exception("User not found!");

			var role = await _roleManager.FindByNameAsync(roleName);
			if (role == null)
				throw new Exception("Role not found!");

			if (await _userManager.IsInRoleAsync(user, role.Name))
			{
				ViewBag.ErrorMessage = "This user already has the role specified !";

				roles = (from r in _roleManager.Roles select r.Name).ToList();
				ViewBag.Roles = new SelectList(roles);

				ViewBag.UserName = userName;

				return View();
			}
			else
			{
				await _userManager.AddToRoleAsync(user, role.Name);
				_dataContext.SaveChanges();

				List<string> userRoles;
				var userRoleNames = (from r in await _userManager.GetRolesAsync(user) select r);
				// userRoles = (from id in userRoleIds
				//                 let r = await _roleManager.FindByIdAsync(id)
				//                 select r).ToList();

				userRoles = new List<string>();
				foreach (var name in userRoleNames)
				{
					var r = await _roleManager.FindByNameAsync(name);
					userRoles.Add(r.Name);
				}

				ViewBag.UserName = userName;
				ViewBag.RolesForUser = userRoles;

				return View("ViewUsersRoles");
			}
		}

		[ActionName("DeleteRoleForUserAsync")]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult> DeleteRoleForUserAsync(string userName = null, string roleName = null)
		{
			if ((!string.IsNullOrWhiteSpace(userName)) || (!string.IsNullOrWhiteSpace(roleName)))
			{
				List<string> userRoles;

				// var roleStore = new RoleStore<IdentityRole>(_dataContext);
				// var roleManager = new RoleManager<IdentityRole>(roleStore);

				// var userStore = new UserStore<ApplicationUser>(_dataContext);
				// var userManager = new UserManager<ApplicationUser>(userStore);

				var user = await _userManager.FindByNameAsync(userName);
				if (user == null)
					throw new Exception("User not found!");

				if (await _userManager.IsInRoleAsync(user, roleName))
				{
					await _userManager.RemoveFromRoleAsync(user, roleName);
					_dataContext.SaveChanges();
				}

				var userRoleNames = (from r in await _userManager.GetRolesAsync(user) select r);
				// userRoles = (from id in userRoleIds
				//                 let r = await _roleManager.FindByIdAsync(id)
				//                 select r).ToList();

				userRoles = new List<string>();
				foreach (var name in userRoleNames)
				{
					var r = await _roleManager.FindByNameAsync(name);
					userRoles.Add(r.Name);
				}

				ViewBag.RolesForUser = userRoles;
				ViewBag.UserName = userName;

				return View("ViewUsersRoles");
			}
			else
			{
				return View("Index");
			}

		}
		#endregion
	}
}
