using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace ABCMusic_Auth.Utilities
{
	public class Paginator<T> : IPaginator<T>
	{
		private int _currentPageNum;
		private int _pageCount;
		private int _itemsPerPage;
		private readonly IEnumerable<T> _items;
		private int _itemCount;

		public int CurrentPageNumber
		{
			get => _currentPageNum;
			set
			{
				if (value > _pageCount) _currentPageNum = _pageCount;
				else if (value < 1) _currentPageNum = 1;
				else _currentPageNum = value;
			}
		}
		public int PageCount
		{
			get => _pageCount;
		}
		public int ItemsPerPage
		{
			get => _itemsPerPage;
			set
			{
				if (value <= 0) _itemsPerPage = 1;
				else _itemsPerPage = value;
			}
		}
		public int ItemCount { get => _itemCount; }

		public Paginator(IEnumerable<T> originalItems, int itemsPerPage, int pageNumber)
		{
			if (originalItems == null)
				originalItems = new T[0];

			_itemCount = originalItems.Count();

			// maximize item count
			// round up to nearest page
			int maximumItemCount = 0;

			while (maximumItemCount < _itemCount) maximumItemCount += itemsPerPage;

			_pageCount = maximumItemCount / itemsPerPage;

			ItemsPerPage = itemsPerPage;
			CurrentPageNumber = pageNumber;

			List<T> pagedItems = new List<T>(itemsPerPage);

			if (pageNumber == 0) {
				_items = new T[0];
				return;
			}

			for (int i = (pageNumber - 1) * itemsPerPage; (i < pageNumber * itemsPerPage) && (i < _itemCount); i++)
			{
				pagedItems.Add(originalItems.ElementAt(i));
			}

			_items = pagedItems.ToArray() as IEnumerable<T>;
		}

		public string RenderPaginatorNav(Func<int, string> actionGenerator)
		{
			StringBuilder htmlPaginatorNav = new StringBuilder();

			htmlPaginatorNav.AppendLine("<div class=\"abc-paginator btn-toolbar\" role=\"toolbar\" aria-label=\"Toolbar with button groups\">");

			//
			// start pre-group
			//
			htmlPaginatorNav.AppendLine("\t<div class=\"btn-group mr-2\" role=\"group\" aria-label=\"Pre-group\">");

			//
			// generate first button
			//
			string firstButtonHtml = $"<button type=\"button\" class=\"btn btn-secondary\"" +
				$"onclick=\"location.href = '{actionGenerator(1)}';\">&#124;&lt;</button>";

			htmlPaginatorNav.AppendLine($"\t\t{firstButtonHtml}");

			//
			// generate previous button
			//
			string prevButtonHtml = "";

			if (_currentPageNum > 1)
			{
				prevButtonHtml = $"<button type=\"button\" class=\"btn btn-secondary\"" +
					$"onclick=\"location.href = '{actionGenerator(_currentPageNum - 1)}';\">&lt;&lt;</button>";
			}
			else
			{
				prevButtonHtml = $"<button disabled type=\"button\" class=\"btn btn-outline\">&lt;&lt;</button>";
			}

			htmlPaginatorNav.AppendLine($"\t\t{prevButtonHtml}");

			//
			// end pre-group
			// 
			htmlPaginatorNav.AppendLine("\t</div>");

			//
			// start main-group
			//
			htmlPaginatorNav.AppendLine("\t<div class=\"btn-group mr-2\" role=\"group\" aria-label=\"Main-group\">");

			//
			// generate pagination buttons
			//
			int start = 1;
			int end = _pageCount;

			// keep displayed page count to a limit of 5
			if (_pageCount > 5)
			{
				if (_currentPageNum > 3) start = _currentPageNum - 2;

				if (_currentPageNum < _pageCount - 2) end = _currentPageNum + 2;
			}

			if (_currentPageNum > 3 && _pageCount > 5) htmlPaginatorNav.AppendLine($"\t\t<span>...</span>");

			// display the nearest five buttons
			for (int i = start; i <= end; i++)
			{
				// generate the on-click action for each pagination button
				string buttonHtml;

				if (i == _currentPageNum)
				{
					buttonHtml = $"<button type=\"button\" class=\"btn btn-primary\">{i}</button>";
				}
				else
				{
					string buttonOnClick = actionGenerator(i);

					buttonHtml = $"<button type=\"button\" class=\"btn btn-secondary\"" +
						$"onclick=\"location.href = '{buttonOnClick}';\">{i}</button>";
				}

				htmlPaginatorNav.AppendLine($"\t\t{buttonHtml}");
			}

			if (_currentPageNum < _pageCount - 2 && _pageCount > 5) htmlPaginatorNav.AppendLine($"\t\t<span>...</span>");

			//
			// end main-group
			//
			htmlPaginatorNav.AppendLine("\t</div>");

			//
			// start post-group
			//
			htmlPaginatorNav.AppendLine("\t<div class=\"btn-group mr-2\" role=\"group\" aria-label=\"Post-group\">");

			//
			// generate next button
			//
			string nextButtonHtml = "";

			if (_currentPageNum < _pageCount)
			{
				nextButtonHtml = $"<button type=\"button\" class=\"btn btn-secondary\"" +
					$"onclick=\"location.href = '{actionGenerator(_currentPageNum + 1)}';\">&gt;&gt;</button>";
			}
			else
			{
				nextButtonHtml = $"<button disabled type=\"button\" class=\"btn btn-outline\">&gt;&gt;</button>";
			}

			htmlPaginatorNav.AppendLine($"\t\t{nextButtonHtml}");

			//
			// generate last button
			//
			string lastButtonHtml = $"<button type=\"button\" class=\"btn btn-secondary\"" +
				$"onclick=\"location.href = '{actionGenerator(_pageCount)}';\">&gt;&#124;</button>";

			htmlPaginatorNav.AppendLine($"\t\t{lastButtonHtml}");

			//
			// end post-group
			//
			htmlPaginatorNav.AppendLine("\t</div>");

			htmlPaginatorNav.AppendLine("</div>");

			return htmlPaginatorNav.ToString();
		}

		public string RenderPaginatorSizeControl(Func<int, string> actionGenerator, int pageSize = 10)
		{
			// build out an HTML widget for controlling size
			StringBuilder htmlBuilder = new StringBuilder();

			

			return htmlBuilder.ToString();
		}

		public IEnumerable<T> GetItems()
		{
			return _items;
		}
	}
}
