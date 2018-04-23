using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ABCMusic_Auth.Data;
using ABCMusic_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ABCMusic_Auth.Models.SearchViewModels;
using ABCMusic_Auth.Utilities;

namespace ABCMusic_Auth.Controllers
{
	[Authorize]
	public class AlbumsController : Controller
	{
		private readonly AngelicBeatsDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public AlbumsController(AngelicBeatsDbContext context, UserManager<ApplicationUser> userManager)
		{
			if (context == null) throw new Exception("Null database context supplied.");
			if (userManager == null) throw new Exception("Null user manager supplied.");

			_context = context;
			_userManager = userManager;
		}

		// GET: Albums
		public async Task<IActionResult> Index(AlbumSearchViewModel searchModel)
		{
			int? emphasisHeaderNum = null;
			IEnumerable<Album> albums = await _context.Albums.ToListAsync();

			// get current user
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

			if (currentUser == null) {
				throw new Exception("No current user.");
			}

			// filter by current owner
			albums = albums.Where(s => s.ArtistId == currentUser.Id);

			// search criteria
			if (!string.IsNullOrWhiteSpace(searchModel.SearchCriteria))
			{
				albums = albums.Where(item => item.Name.ToUpper().Contains(searchModel.SearchCriteria.ToUpper()));
			}

			// date filtering
			// start date
			if (!string.IsNullOrEmpty(searchModel.StartDate))
			{
				DateTime startDate = DateTime.Parse(searchModel.StartDate);
				albums = albums.Where(item => item.ReleaseDate.Value.CompareTo(startDate) >= 0);
			}

			// end date
			if (!string.IsNullOrEmpty(searchModel.EndDate))
			{
				DateTime endDate = DateTime.Parse(searchModel.EndDate);
				albums = albums.Where(item => item.ReleaseDate.Value.CompareTo(endDate) <= 0);
			}

			// sort order
			switch (searchModel.SortOrder)
			{
				case "album-name":
					albums = albums.OrderBy(s => s.Name);
					emphasisHeaderNum = 0;
					break;
				case "artist":
					albums = albums.OrderBy(s => s.ArtistName);
					emphasisHeaderNum = 1;
					break;
				// case "album-name":
				// 	albums = albums.OrderBy(s => {
				// 		if (s.Album != null) return s.Album.Name;
				// 		else return "Unknown Album";
				// 	});
				// 	emphasisHeaderNum = 2;
				// 	break;
				case "release-date":
					albums = albums.OrderBy(s => {
						if (s.ReleaseDate != null) return s.ReleaseDate.Value.ToString();
						else return "";
					});
					emphasisHeaderNum = 2;
					break;
				default:
					albums = albums.OrderBy(s => s.Id);
					break;
			}

			// flip order
			if (searchModel.FlipOrder)
			{
				albums = albums.Reverse();
			}

			// pagination
			int pageNumber = (searchModel.PageNumber ?? 1);
			int pageSize = (searchModel.PageSize ?? 10);

			IPaginator<Album> paginator = new Paginator<Album>(albums, pageSize, pageNumber);

			ViewData["paginator"] = paginator;
			ViewData["searchSettings"] = searchModel;
			ViewData["emphasisHeaderNum"] = emphasisHeaderNum;

			// send the paginated items to the view
			return View(paginator.GetItems());
		}

		// GET: Albums/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var album = await _context.Albums
				.Include(a => a.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (album == null)
			{
				return NotFound();
			}

			return View(album);
		}

		// GET: Albums/Create
		public IActionResult Create()
		{
			//ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id");
			return View();
		}

		// POST: Albums/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Publisher,Name,ArtistName,ReleaseDate")] Album album)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (currentUser == null) throw new Exception("Cannot find user.");

			album.ArtistId = currentUser.Id;
			album.Artist = currentUser;

			if (ModelState.IsValid)
			{
				_context.Add(album);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			
			//ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", album.ArtistId);
			return View(album);
		}

		// GET: Albums/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var album = await _context.Albums.FirstOrDefaultAsync(m => m.Id == id);
			if (album == null)
			{
				return NotFound();
			}
			
			//ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", album.ArtistId);
			return View(album);
		}

		// POST: Albums/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Publisher,Id,ArtistId,Name,ArtistName,ReleaseDate")] Album album)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
			if (currentUser == null) throw new Exception("Current user not found.");
					
			album.ArtistId = currentUser.Id;
			album.Artist = currentUser;

			if (id != album.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(album);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!AlbumExists(album.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}

			//ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", album.ArtistId);
			return View(album);
		}

		// GET: Albums/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var album = await _context.Albums
				.Include(a => a.Artist)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (album == null)
			{
				return NotFound();
			}

			return View(album);
		}

		// POST: Albums/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var album = await _context.Albums.FirstOrDefaultAsync(m => m.Id == id);
			_context.Albums.Remove(album);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool AlbumExists(int id)
		{
			return _context.Albums.Any(e => e.Id == id);
		}
	}
}
