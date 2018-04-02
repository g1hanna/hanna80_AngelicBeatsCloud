using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ABCMusic_Auth.Models.SearchViewModels
{
    public class SongSearchViewModel
    {
        public int? PageNumber { get; set; }

		public int? PageSize { get; set; }

		public string SortOrder { get; set; }

		public bool FlipOrder { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public string SearchCriteria { get; set; }

		public byte Rating { get; set; }
    }
}
