using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ABCMusic_Auth.Data;
using ABCMusic_Auth.Models;
using ABCMusic_Auth.Models.SearchViewModels;
using ABCMusic_Auth.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ABCMusic_Auth.Controllers
{
	[Authorize]
	public class SongsController : Controller
	{
		private readonly AngelicBeatsDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public SongsController(AngelicBeatsDbContext context, UserManager<ApplicationUser> userManager)
		{
			if (context == null) throw new Exception("Null database context supplied.");
			if (userManager == null) throw new Exception("Null user manager supplied.");

			_context = context;
			_userManager = userManager;
		}

		// GET: Songs
		[ActionName("Index")]
		public async Task<IActionResult> IndexAsync(SongSearchViewModel searchModel)
		{
			int? emphasisHeaderNum = null;

			// load songs and related albums
			IEnumerable<Song> songs = await _context.Songs
				.Include(s => s.Album)
				.ToListAsync();

			// get current user
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			if (currentUser == null)
			{
				throw new Exception("No current user.");
			}

			// filter by current owner
			songs = songs.Where(s => s.ArtistId == currentUser.Id);

			// if album supplied, filter by that album
			if (searchModel.AlbumId != null)
			{
				Album album = _context.Albums.FirstOrDefault(a => a.Id == searchModel.AlbumId);

				if (album == null) throw new Exception("That album doesn't exist.");

				songs = songs.Where(s => s.AlbumId == album.Id);
			}

			// search criteria
			if (!string.IsNullOrWhiteSpace(searchModel.SearchCriteria))
			{
				songs = songs.Where(item => item.Name.ToUpper().Contains(searchModel.SearchCriteria.ToUpper()));
			}

			// date filtering
			// start date
			if (!string.IsNullOrEmpty(searchModel.StartDate))
			{
				DateTime startDate = DateTime.Parse(searchModel.StartDate);
				songs = songs.Where(item => item.ReleaseDate.Value.CompareTo(startDate) >= 0);
			}

			// end date
			if (!string.IsNullOrEmpty(searchModel.EndDate))
			{
				DateTime endDate = DateTime.Parse(searchModel.EndDate);
				songs = songs.Where(item => item.ReleaseDate.Value.CompareTo(endDate) <= 0);
			}

			// sort order
			switch (searchModel.SortOrder)
			{
				case "song-name":
					songs = songs.OrderBy(s => s.Name);
					emphasisHeaderNum = 0;
					break;
				case "artist":
					songs = songs.OrderBy(s => s.ArtistName);
					emphasisHeaderNum = 1;
					break;
				case "album-name":
					songs = songs.OrderBy(s => {
						if (s.Album != null) return s.Album.Name;
						else return "Unknown Album";
					});
					emphasisHeaderNum = 2;
					break;
				case "release-date":
					songs = songs.OrderBy(s => {
						if (s.ReleaseDate != null) return s.ReleaseDate.Value.ToString();
						else return "";
					});
					emphasisHeaderNum = 3;
					break;
				default:
					songs = songs.OrderBy(s => s.Id);
					break;
			}

			// flip order
			if (searchModel.FlipOrder)
			{
				songs = songs.Reverse();
			}

			// pagination
			int pageNumber = (searchModel.PageNumber ?? 1);
			int pageSize = (searchModel.PageSize ?? 10);

			IPaginator<Song> paginator = new Paginator<Song>(songs, pageSize, pageNumber);

			ViewData["paginator"] = paginator;
			ViewData["searchSettings"] = searchModel;
			ViewData["emphasisHeaderNum"] = emphasisHeaderNum;

			// send the paginated items to the view
			return View(paginator.GetItems());
		}

		[ActionName("Admin")]
		public async Task<IActionResult> AdminAsync(AdminSongSearchViewModel searchModel)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			int? emphasisHeaderNum = null;

			// load songs and related albums and artists
			IEnumerable<Song> songs = await _context.Songs
				.Include(s => s.Album)
				.Include(s => s.Artist)
				.ToListAsync();


			if (currentUser == null)
			{
				throw new Exception("No current user.");
			}

			// filter by owner in model
			if (!string.IsNullOrEmpty(searchModel.UserName))
			{
				songs = songs.Where(s => s.Artist.UserName == searchModel.UserName);
			}

			// if album supplied, filter by that album
			if (searchModel.AlbumId != null)
			{
				Album album = _context.Albums.FirstOrDefault(a => a.Id == searchModel.AlbumId);

				if (album == null) {
					return NotFound();
				}

				songs = songs.Where(s => s.AlbumId == album.Id);
			}

			// search criteria
			if (!string.IsNullOrWhiteSpace(searchModel.SearchCriteria))
			{
				songs = songs.Where(item => item.Name.ToUpper().Contains(searchModel.SearchCriteria.ToUpper()));
			}

			// date filtering
			// start date
			if (!string.IsNullOrEmpty(searchModel.StartDate))
			{
				DateTime startDate = DateTime.Parse(searchModel.StartDate);
				songs = songs.Where(item => item.ReleaseDate.Value.CompareTo(startDate) >= 0);
			}

			// end date
			if (!string.IsNullOrEmpty(searchModel.EndDate))
			{
				DateTime endDate = DateTime.Parse(searchModel.EndDate);
				songs = songs.Where(item => item.ReleaseDate.Value.CompareTo(endDate) <= 0);
			}

			// sort order
			switch (searchModel.SortOrder)
			{
				case "song-name":
					songs = songs.OrderBy(s => s.Name);
					emphasisHeaderNum = 0;
					break;
				case "artist":
					songs = songs.OrderBy(s => s.ArtistName);
					emphasisHeaderNum = 1;
					break;
				case "album-name":
					songs = songs.OrderBy(s => {
						if (s.Album != null) return s.Album.Name;
						else return "Unknown Album";
					});
					emphasisHeaderNum = 2;
					break;
				case "release-date":
					songs = songs.OrderBy(s => {
						if (s.ReleaseDate != null) return s.ReleaseDate.Value.ToString();
						else return "";
					});
					emphasisHeaderNum = 3;
					break;
				default:
					songs = songs.OrderBy(s => s.Id);
					break;
			}

			// flip order
			if (searchModel.FlipOrder)
			{
				songs = songs.Reverse();
			}

			// pagination
			int pageNumber = (searchModel.PageNumber ?? 1);
			int pageSize = (searchModel.PageSize ?? 10);

			IPaginator<Song> paginator = new Paginator<Song>(songs, pageSize, pageNumber);

			ViewData["paginator"] = paginator;
			ViewData["searchSettings"] = searchModel;
			ViewData["emphasisHeaderNum"] = emphasisHeaderNum;
			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildAlbumsDropDownList();
			ViewData["ArtistUsername"] = (IEnumerable<SelectListItem>)BuildArtistsDropDownList(true);

			// send the paginated items to the view
			return View(paginator.GetItems());
		}

		// GET: Songs/Details/5
		[ActionName("Details")]
		public async Task<IActionResult> DetailsAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs
				.Include(s => s.Album)
				.Include(s => s.Album.Artist)
				.Include(s => s.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
				
			if (song == null)
			{
				return NotFound();
			}

			return View(song);
		}

		[ActionName("AdminDetails")]
		public async Task<IActionResult> AdminDetailsAsync(int? id)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs
				.Include(s => s.Album)
				.Include(s => s.Album.Artist)
				.Include(s => s.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
				
			if (song == null)
			{
				return NotFound();
			}

			return View(song);
		}

		// GET: Songs/Create
		[HttpGet, ActionName("Create")]
		public async Task<IActionResult> CreateAsync()
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);

			return View();
		}

		[HttpGet, ActionName("AdminCreate")]
		public async Task<IActionResult> AdminCreateAsync()
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildAlbumsDropDownList();
			ViewData["ArtistUsername"] = (IEnumerable<SelectListItem>)BuildArtistsDropDownList();
			return View();
		}

		// POST: Songs/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAsync([Bind("Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher,AlbumId")] Song song)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			if (ModelState.IsValid)
			{
				song.ArtistId = currentUser.Id;
				song.Artist = currentUser;

				_context.Add(song);
				await _context.SaveChangesAsync();
				return RedirectToAction("Admin");
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);
			return View(song);
		}

		[HttpPost, ActionName("AdminCreate")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AdminCreateAsync([Bind("Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher,AlbumId,ArtistId")] Song song)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			if (ModelState.IsValid)
			{
				_context.Add(song);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);
			return View(song);
		}

		// GET: Songs/Edit/5
		[ActionName("Edit")]
		public async Task<IActionResult> EditAsync(int? id)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs
				.Include(s => s.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (song == null)
			{
				return NotFound();
			}

			if (song.ArtistId != currentUser.Id)
			{
				if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
				{
					return RedirectToAction("AdminEdit", new { id = id });
				}

				return RedirectToAction("AccessDenied", "Error");
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);
			return View(song);
		}

		[ActionName("AdminEdit")]
		public async Task<IActionResult> AdminEditAsync(int? id)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			var song = await _context.Songs
				.Include(s => s.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (id == null)
			{
				return NotFound();
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildAlbumsDropDownList();
			//ViewData["ArtistUsername"] = (IEnumerable<SelectListItem>)BuildArtistsDropDownList();
			return View(song);
		}

		// POST: Songs/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditAsync(int id, [Bind("Id,Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher,AlbumId")] Song song)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			if (id != song.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					//ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

					song.ArtistId = currentUser.Id;
					song.Artist = currentUser;

					_context.Update(song);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!SongExists(song.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("Details", new { id = id });
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);
			return View(song);
		}

		[HttpPost, ActionName("AdminEdit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AdminEditAsync(int id, [Bind("Id,Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher,AlbumId")] Song song)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			if (id != song.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					//ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
					_context.Update(song);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!SongExists(song.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction("AdminDetails", new { id = id });
			}

			ViewData["AlbumId"] = (IEnumerable<SelectListItem>)BuildUserAlbumsDropDownList(currentUser);
			return View(song);
		}

		// GET: Songs/Delete/5
		[HttpGet, ActionName("Delete")]
		public async Task<IActionResult> DeleteAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs
				.FirstOrDefaultAsync(m => m.Id == id);
			if (song == null)
			{
				return NotFound();
			}

			return View(song);
		}

		[HttpGet, ActionName("AdminDelete")]
		public async Task<IActionResult> AdminDeleteAsync(int? id)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs
				.FirstOrDefaultAsync(m => m.Id == id);
			if (song == null)
			{
				return NotFound();
			}

			return View(song);
		}

		// POST: Songs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmedAsync(int id)
		{
			var song = await _context.Songs
				.Include(_ => _.Reviews)
				.FirstOrDefaultAsync(m => m.Id == id);

			// remove reviews
			// TODO: Update to detach reviews
			if (song.Reviews.Count > 0) {
				_context.Reviews.RemoveRange(song.Reviews);
				await _context.SaveChangesAsync();
			}

			// remove song
			_context.Songs.Remove(song);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		[HttpPost, ActionName("AdminDelete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AdminDeleteConfirmed(int id)
		{
			// ensure user has admin functionality
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				return RedirectToAction("AccessDenied", "Error");
			}

			var song = await _context.Songs
				.Include(_ => _.Reviews)
				.FirstOrDefaultAsync(m => m.Id == id);

			// remove reviews
			// TODO: Update to detach reviews
			if (song.Reviews.Count > 0) {
				_context.Reviews.RemoveRange(song.Reviews);
				await _context.SaveChangesAsync();
			}

			// remove song
			_context.Songs.Remove(song);
			await _context.SaveChangesAsync();

			return RedirectToAction("Admin");
		}

		private bool SongExists(int id)
		{
			return _context.Songs.Any(e => e.Id == id);
		}

		[NonAction]
		private List<SelectListItem> BuildAlbumsDropDownList()
		{
			return BuildAlbumsDropDownList(
				// load albums, songs, and their related data
				_context.Albums
					.Include(a => a.Artist)
					.ToList());
		}

		[NonAction]
		private List<SelectListItem> BuildUserAlbumsDropDownList(ApplicationUser user)
		{
			return BuildAlbumsDropDownList(
				// load albums, songs, and their related data
				// ensure albums belong to specified user
				_context.Albums
					.Where(a => a.ArtistId == user.Id)
					.Include(a => a.Artist)
					.ToList());
		}

		[NonAction]
		private List<SelectListItem> BuildAlbumsDropDownList(IEnumerable<Album> albums)
		{
			// drop down list for reviewables
			var reviewableSelectList = new List<SelectListItem>();
			
			// add a blank item
			reviewableSelectList.Add(new SelectListItem() {
				Selected = true,
				Text = "None",
				Value = ""
			});

			// convert albums into select items
			//var albums = _context.Albums.ToList();
			foreach (var album in albums)
			{
				string reviewableString = $"Album \"{album.Name}\"";
				if (album.Artist != null) reviewableString +=  $" by {album.Artist.UserName}";

				reviewableSelectList.Add(new SelectListItem() {
					Text = reviewableString, Value = album.Id.ToString()
				});
			}

			return reviewableSelectList;
		}

		[NonAction]
		private List<SelectListItem> BuildArtistsDropDownList(IEnumerable<ApplicationUser> users, bool noneOption = false)
		{
			var artistSelectList = new List<SelectListItem>();

			if (noneOption)
			{
				artistSelectList.Add(new SelectListItem() {
					Text = "Choose...",
					Value = ""
				});
			}

			foreach (var user in users)
			{
				string artistName = user.UserName;

				artistSelectList.Add(new SelectListItem() {
					Text = artistName, Value = artistName
				});
			}

			return artistSelectList;
		}

		[NonAction]
		private List<SelectListItem> BuildArtistsDropDownList(bool noneOption = false)
		{
			return BuildArtistsDropDownList(_context.Users.ToList(), noneOption);
		}
	}
}
