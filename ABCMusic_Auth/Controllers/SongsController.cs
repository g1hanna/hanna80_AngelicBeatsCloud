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

namespace ABCMusic_Auth.Controllers
{
	public class SongsController : Controller
	{
		private readonly AngelicBeatsDbContext _context;

		public SongsController(AngelicBeatsDbContext context)
		{
			_context = context;
		}

		// GET: Songs
		public async Task<IActionResult> Index(SongSearchViewModel searchModel)
		{
			IEnumerable<Song> songs = await _context.Songs.ToListAsync();

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
					break;
				case "artist":
					songs = songs.OrderBy(s => s.Artist.GetName());
					break;
				case "album-name":
					songs = songs.OrderBy(s => {
						if (s.Album != null) return s.Album.Name;
						else return "Unknown Album";
					});
					break;
				case "release-date":
					songs = songs.OrderBy(s => {
						if (s.ReleaseDate != null) return s.ReleaseDate.Value.ToString();
						else return "";
					});
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

			// send the paginated items to the view
			return View(paginator.GetItems());
		}

		// GET: Songs/Details/5
		public async Task<IActionResult> Details(int? id)
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

		// GET: Songs/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Songs/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher")] Song song)
		{
			if (ModelState.IsValid)
			{
				_context.Add(song);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(song);
		}

		// GET: Songs/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var song = await _context.Songs.FirstOrDefaultAsync(m => m.Id == id);
			if (song == null)
			{
				return NotFound();
			}
			return View(song);
		}

		// POST: Songs/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArtistName,Length,ReleaseDate,TrackNumber,Publisher")] Song song)
		{
			if (id != song.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
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
				return RedirectToAction(nameof(Index));
			}
			return View(song);
		}

		// GET: Songs/Delete/5
		public async Task<IActionResult> Delete(int? id)
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

		// POST: Songs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var song = await _context.Songs.FirstOrDefaultAsync(m => m.Id == id);
			_context.Songs.Remove(song);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool SongExists(int id)
		{
			return _context.Songs.Any(e => e.Id == id);
		}
	}
}
