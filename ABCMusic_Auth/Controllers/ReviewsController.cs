using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ABCMusic_Auth.Data;
using ABCMusic_Auth.Models;
using ABCMusic_Auth.Models.ReviewViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ABCMusic_Auth.Controllers
{
	[Authorize]
	public class ReviewsController : Controller
	{
		private readonly AngelicBeatsDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ReviewsController(AngelicBeatsDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Reviews
		public async Task<IActionResult> Index()
		{
			var angelicBeatsDbContext = _context.Reviews.Include(r => r.Author).Include(r => r.Reviewable);
			return View(await BuildItemReviewViewModelListAsync(await angelicBeatsDbContext.ToListAsync()));
		}

		// GET: Reviews/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var review = await _context.Reviews
				.Include(r => r.Author)
				.Include(r => r.Reviewable)
				.SingleOrDefaultAsync(m => m.ReviewId == id);
			if (review == null)
			{
				return NotFound();
			}

			bool editPermission;
			if (review.Author.UserName != User.Identity.Name)
				editPermission = false;
			else
				editPermission = true;

			ViewBag.CanEdit = editPermission;
			return View(await BuildItemReviewViewModelAsync(review));
		}

		// GET: Reviews/Create
		public IActionResult Create()
		{
			// drop down list for authors
			// var authorSelectList = new List<SelectListItem>();
			// var users = _context.Users.ToList();
			// foreach (var user in users)
			// 	authorSelectList.Add(new SelectListItem() {
			// 		Text = user.UserName, Value = user.Id
			// 	});
			
			// drop down list for reviewables
			// var reviewableSelectList = new List<SelectListItem>();
			// // convert albums into select items
			// var albums = _context.Albums.ToList();
			// foreach (var album in albums)
			// 	reviewableSelectList.Add(new SelectListItem() {
			// 		Text = $"Album \"{album.Name}\"", Value = album.Id.ToString()
			// 	});
			
			// // convert songs into select items
			// var songs = _context.Songs.ToList();
			// foreach (var song in songs) {
			// 	string listItemText = $"Song \"{song.Name}\"";

			// 	// append album name if album is not null
			// 	if (song.Album != null) {
			// 		listItemText += $" in \"{song.Album.Name}\"";
			// 	}

			// 	reviewableSelectList.Add(new SelectListItem() {
			// 		Text = listItemText, Value = song.Id.ToString()
			// 	});
			// }

			// send select lists to the view
			//ViewData["AuthorId"] = (IEnumerable<SelectListItem>)authorSelectList;
			ViewData["ReviewableId"] = (IEnumerable<SelectListItem>)BuildReviewablesDropDownList();

			return View();
		}

		// POST: Reviews/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Subject,Rating,Content,ReviewableId")] Review review)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
				if (currentUser == null) throw new Exception("Current user not found.");

				review.Author = currentUser;
				review.AuthorId = currentUser.Id;

				_context.Reviews.Add(review);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			//ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
			ViewData["ReviewableId"] = (IEnumerable<SelectListItem>)BuildReviewablesDropDownList();

			return View(await BuildItemReviewViewModelAsync(review));
		}

		// GET: Reviews/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var review = await _context.Reviews
				.Include(r => r.Author)
				.Include(r => r.Reviewable)
				.SingleOrDefaultAsync(m => m.ReviewId == id);
			if (review == null)
			{
				return NotFound();
			}
			
			//ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
			ViewData["ReviewableId"] = (IEnumerable<SelectListItem>)BuildReviewablesDropDownList();
			return View(await BuildItemReviewViewModelAsync(review));
		}

		// POST: Reviews/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Subject,Rating,Content,ReviewableId")] ItemReviewViewModel reviewModel)
		{
			ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
				if (currentUser == null) throw new Exception("Current user not found.");

			Review review = new Review {
				ReviewId = reviewModel.Id,
				Subject = reviewModel.Subject,
				Rating = reviewModel.Rating,
				Content = reviewModel.Content,
				ReviewableId = reviewModel.ReviewableId
			};

			// current user is author
			review.Author = currentUser;
			review.AuthorId = currentUser.Id;

			if (id != reviewModel.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Reviews.Update(review);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ReviewExists(review.ReviewId))
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
			
			//ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", review.AuthorId);
			ViewData["ReviewableId"] = (IEnumerable<SelectListItem>)BuildReviewablesDropDownList();
			return View(await BuildItemReviewViewModelAsync(review));
		}

		// GET: Reviews/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var review = await _context.Reviews
				.Include(r => r.Author)
				.Include(r => r.Reviewable)
				.SingleOrDefaultAsync(m => m.ReviewId == id);
			
			if (review == null)
			{
				return NotFound();
			}

			ItemReviewViewModel reviewModel = await BuildItemReviewViewModelAsync(review);

			return View(reviewModel);
		}

		// POST: Reviews/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var review = await _context.Reviews.SingleOrDefaultAsync(m => m.ReviewId == id);
			_context.Reviews.Remove(review);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// get a list of reviews for a song
		public async Task<IActionResult> ReviewsBySong(int id)
		{
			var songReviews = await _context.Reviews
				.Where(r => r.ReviewableId == id)
				.ToListAsync();

			// get song
			Song song = _context.Songs.FirstOrDefault(s => s.Id == id);

			if (song != null)
			{
				ViewBag.Item = song;
				return View(songReviews);
			}
			else
			{
				ViewBag.ErrorMessage = "Song not found.";
				return View("Error");
			}
		}

		// get a list of reviews for an album
		public async Task<IActionResult> ReviewsByAlbum(int id)
		{
			var itemReviews = await _context.Reviews
				.Where(r => r.ReviewableId == id)
				.Include(r => r.Author)
				.ToListAsync();

			// get song
			Album album = _context.Albums.FirstOrDefault(s => s.Id == id);

			if (album != null)
			{
				ViewBag.Item = album;
				return View(itemReviews);
			}
			else
			{
				ViewBag.ErrorMessage = "Album not found.";
				return View("Error");
			}
		}

		public IActionResult UserCreateBySong(int id)
		{
			// load songs with artist and album
			Song song = _context.Songs
				.Include(s => s.Artist)
				.Include(s => s.Album)
				.FirstOrDefault(s => s.Id == id);

			if (song == null)
			{
				return NotFound();
			}

			ViewBag.Item = song;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UserCreateBySong([Bind("Id,Subject,Rating,Content,ReviewableId")] Review review)
		{
			if (ModelState.IsValid)
			{
				string userName = User.Identity.Name;
				ApplicationUser currentUser = await _userManager.FindByNameAsync(userName);

				review.Author = currentUser;
				review.AuthorId = currentUser.Id;

				_context.Reviews.Add(review);
				_context.SaveChanges();

				return RedirectToAction(nameof(ReviewsBySong), new { id = review.ReviewableId });
			}

			Song song = _context.Songs.FirstOrDefault(s => s.Id == review.ReviewableId);
			
			if (song == null)
			{
				return NotFound();
			}

			ViewBag.Item = song;

			return View(review);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UserCreateByAlbum([Bind("Id,Subject,Rating,Content,ReviewableId")] Review review)
		{
			if (ModelState.IsValid)
			{
				_context.Reviews.Add(review);
				_context.SaveChanges();

				return RedirectToAction(nameof(ReviewsByAlbum), new { id = review.ReviewableId });
			}

			return View(review);
		}

		#region NONACTIONS
		[NonAction]
		private bool ReviewExists(int id)
		{
			return _context.Reviews.Any(e => e.ReviewId == id);
		}

		[NonAction]
		private List<SelectListItem> BuildAuthorsDropDownList()
		{
			// drop down list for authors
			var authorSelectList = new List<SelectListItem>();
			var users = _context.Users.ToList();
			foreach (var user in users)
				authorSelectList.Add(new SelectListItem() {
					Text = user.UserName, Value = user.Id
				});
			
			return authorSelectList;
		}

		[NonAction]
		private List<SelectListItem> BuildReviewablesDropDownList()
		{
			return BuildReviewablesDropDownList(
				// load albums, songs, and their related data
				_context.Albums
					.Include(a => a.Artist)
					.ToList(),
				_context.Songs
					.Include(s => s.Artist)
					.Include(s => s.Album)
					.ToList());
		}

		[NonAction]
		private List<SelectListItem> BuildReviewablesDropDownList(IEnumerable<Album> albums, IEnumerable<Song> songs)
		{
			// drop down list for reviewables
			var reviewableSelectList = new List<SelectListItem>();
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
			
			// convert songs into select items
			//var songs = _context.Songs.ToList();
			foreach (var song in songs) {
				string listItemText = $"Song \"{song.Name}\"";

				// append album name if album is not null
				if (song.Album != null) {
					listItemText += $" in \"{song.Album.Name}\"";
				}

				// append artist name if artist is not null
				if (song.Artist != null) {
					listItemText += $" by {song.Artist.UserName}";
				}

				reviewableSelectList.Add(new SelectListItem() {
					Text = listItemText, Value = song.Id.ToString()
				});
			}

			return reviewableSelectList;
		}

		[NonAction]
		private async Task<ICollection<ItemReviewViewModel>> BuildItemReviewViewModelListAsync(IEnumerable<Review> itemReviews)
		{
			IList<ItemReviewViewModel> itemReviewsViewModelList = new List<ItemReviewViewModel>();

			// generate a dictionary with song and album ids and names
			// this will serve as a lookup
			var albumLookup = await _context.Albums.ToDictionaryAsync(a => a.Id, a => a.Name);

			var songLookup = await _context.Songs.ToDictionaryAsync(s => s.Id, s => s.Name);

			foreach (Review review in itemReviews)
			{
				int reviewableId = review.ReviewableId;
				ItemReviewViewModel reviewViewModel;

				if (review.Reviewable is Song)
				{
					Song song = review.Reviewable as Song;

					string reviewableName = $"Song \"{songLookup[reviewableId]}\"";
					if (song.Album != null) reviewableName += $" in \"{song.Album.Name}\"";
					if (song.Artist != null) reviewableName += $" by {song.Artist.UserName}";

					reviewViewModel = new ItemReviewViewModel()
					{
						Id = review.ReviewId,
						Subject = review.Subject,
						Rating = review.Rating,
						Content = review.Content,
						AuthorId = review.AuthorId,
						AuthorUserName = review.Author.UserName,
						ReviewableIsAlbum = false,
						ReviewableId = reviewableId,
						ReviewableName = reviewableName
					};
				}
				else
				{
					Album album = review.Reviewable as Album;

					string reviewableName = $"Album \"{albumLookup[reviewableId]}\"";
					if (album.Artist != null) reviewableName += $" by {album.Artist.UserName}";

					reviewViewModel = new ItemReviewViewModel()
					{
						Id = review.ReviewId,
						Subject = review.Subject,
						Rating = review.Rating,
						Content = review.Content,
						AuthorId = review.AuthorId,
						AuthorUserName = review.Author.UserName,
						ReviewableIsAlbum = true,
						ReviewableId = reviewableId,
						ReviewableName = reviewableName
					};
				}

				itemReviewsViewModelList.Add(reviewViewModel);
			}

			return itemReviewsViewModelList;
		}

		[NonAction]
		private async Task<ItemReviewViewModel> BuildItemReviewViewModelAsync(Review review)
		{
			// generate a dictionary with song and album ids and names
			// this will serve as a lookup
			var albumLookup = await _context.Albums.ToDictionaryAsync(a => a.Id, a => a.Name);

			var songLookup = await _context.Songs.ToDictionaryAsync(s => s.Id, s => s.Name);

			int reviewableId = review.ReviewableId;

			// reload reviewable if null
			if (review.Reviewable == null) {
				Reviewable reviewable = _context.Set<Reviewable>()
					.FirstOrDefault(r => r.Id == review.ReviewableId);

				// if reviewable is song, load song and includeables
				if (reviewable is Song) {
					reviewable = _context.Songs
						// include album
						.Include(s => s.Album)
						// include artist
						.Include(s => s.Artist)
						.FirstOrDefault(s => s.Id == reviewableId);
				}
				// if reviewable is album, load album and includeables
				else {
					reviewable = _context.Albums
						.Include(a => a.Artist)
						.FirstOrDefault(a => a.Id == reviewableId);
				}

				review.Reviewable = reviewable;
			}

			if (review.Reviewable is Song)
			{
				Song song = review.Reviewable as Song;

				string reviewableName = $"Song \"{songLookup[reviewableId]}\"";
				if (song.Album != null) reviewableName += $" in \"{song.Album.Name}\"";
				if (song.Artist != null) reviewableName += $" by {song.Artist.UserName}";

				string reviewAuthorUsername = (await _userManager.FindByIdAsync(review.AuthorId)).UserName;

				return new ItemReviewViewModel()
				{
					Id = review.ReviewId,
					Subject = review.Subject,
					Rating = review.Rating,
					Content = review.Content,
					AuthorId = review.AuthorId,
					AuthorUserName = reviewAuthorUsername,
					ReviewableIsAlbum = false,
					ReviewableId = reviewableId,
					ReviewableName = reviewableName
				};
			}
			else if (review.Reviewable is Album)
			{
				Album album = review.Reviewable as Album;

				string reviewableName = $"Album \"{albumLookup[reviewableId]}\"";
				if (album.Artist != null) reviewableName += $" by {album.Artist.UserName}";

				return new ItemReviewViewModel()
				{
					Id = review.ReviewId,
					Subject = review.Subject,
					Rating = review.Rating,
					Content = review.Content,
					AuthorId = review.AuthorId,
					AuthorUserName = review.Author.UserName,
					ReviewableIsAlbum = true,
					ReviewableId = reviewableId,
					ReviewableName = reviewableName
				};
			}
			else
			{
				throw new Exception("Reviewable is null.");
			}
		}

		[NonAction]
		private bool? CheckUserReviewPermission(Review review)
		{
			if (review == null) return null;
			if (review.Author == null) return null;
			
			return review.Author.UserName == User.Identity.Name;
		}
		#endregion

		#region COMMENTED ACTIONS
		// [NonAction]
		// private ICollection<ItemReviewViewModel> BuildItemReviewViewModelList(IEnumerable<Review> itemReviews)
		// {
		// 	IList<ItemReviewViewModel> itemReviewsViewModelList = new List<ItemReviewViewModel>();

		// 	// generate a dictionary with song and album ids and names
		// 	// this will serve as a lookup
		// 	var albumLookup = _context.Albums.ToDictionary(a => a.Id, a => a.Name);

		// 	var songLookup = _context.Songs.ToDictionary(s => s.Id, s => s.Name);

		// 	foreach (Review review in itemReviews)
		// 	{
		// 		int reviewableId = review.ReviewableId;
		// 		ItemReviewViewModel reviewViewModel;

		// 		if (review.Reviewable is Song)
		// 		{
		// 			Song song = review.Reviewable as Song;

		// 			string reviewableName = $"Album \"{songLookup[reviewableId]}\"";
		// 			if (song.Album != null) reviewableName += $" in \"{song.Album.Name}\"";
		// 			if (song.Artist != null) reviewableName += $" by {song.Artist.UserName}";

		// 			reviewViewModel = new ItemReviewViewModel()
		// 			{
		// 				ReviewId = review.ReviewId,
		// 				Subject = review.Subject,
		// 				Rating = review.Rating,
		// 				Content = review.Content,
		// 				AuthorId = review.AuthorId,
		// 				AuthorUserName = review.Author.UserName,
		// 				ReviewableIsAlbum = false,
		// 				ReviewableId = reviewableId,
		// 				ReviewableName = reviewableName
		// 			};
		// 		}
		// 		else
		// 		{
		// 			Album album = review.Reviewable as Album;

		// 			string reviewableName = $"Album \"{albumLookup[reviewableId]}\"";
		// 			if (album.Artist != null) reviewableName += $" by {album.Artist.UserName}";

		// 			reviewViewModel = new ItemReviewViewModel()
		// 			{
		// 				ReviewId = review.ReviewId,
		// 				Subject = review.Subject,
		// 				Rating = review.Rating,
		// 				Content = review.Content,
		// 				AuthorId = review.AuthorId,
		// 				AuthorUserName = review.Author.UserName,
		// 				ReviewableIsAlbum = true,
		// 				ReviewableId = reviewableId,
		// 				ReviewableName = reviewableName
		// 			};
		// 		}

		// 		itemReviewsViewModelList.Add(reviewViewModel);
		// 	}

		// 	return itemReviewsViewModelList;
		// }

		// [NonAction]
		// private ItemReviewViewModel BuildItemReviewViewModel(Review review)
		// {
		// 	// generate a dictionary with song and album ids and names
		// 	// this will serve as a lookup
		// 	var albumLookup = _context.Albums.ToDictionary(a => a.Id, a => a.Name);

		// 	var songLookup = _context.Songs.ToDictionary(s => s.Id, s => s.Name);

		// 	int reviewableId = review.ReviewableId;

		// 	if (review.Reviewable is Song)
		// 	{
		// 		Song song = review.Reviewable as Song;

		// 		string reviewableName = $"Album \"{songLookup[reviewableId]}\"";
		// 		if (song.Album != null) reviewableName += $" in \"{song.Album.Name}\"";
		// 		if (song.Artist != null) reviewableName += $" by {song.Artist.UserName}";

		// 		return new ItemReviewViewModel()
		// 		{
		// 			ReviewId = review.ReviewId,
		// 			Subject = review.Subject,
		// 			Rating = review.Rating,
		// 			Content = review.Content,
		// 			AuthorId = review.AuthorId,
		// 			AuthorUserName = _userManager.,
		// 			ReviewableIsAlbum = false,
		// 			ReviewableId = reviewableId,
		// 			ReviewableName = reviewableName
		// 		};
		// 	}
		// 	else
		// 	{
		// 		Album album = review.Reviewable as Album;

		// 		string reviewableName = $"Album \"{albumLookup[reviewableId]}\"";
		// 		if (album.Artist != null) reviewableName += $" by {album.Artist.UserName}";

		// 		return new ItemReviewViewModel()
		// 		{
		// 			ReviewId = review.ReviewId,
		// 			Subject = review.Subject,
		// 			Rating = review.Rating,
		// 			Content = review.Content,
		// 			AuthorId = review.AuthorId,
		// 			AuthorUserName = review.Author.UserName,
		// 			ReviewableIsAlbum = true,
		// 			ReviewableId = reviewableId,
		// 			ReviewableName = reviewableName
		// 		};
		// 	}
		// }
		#endregion
	}
}
