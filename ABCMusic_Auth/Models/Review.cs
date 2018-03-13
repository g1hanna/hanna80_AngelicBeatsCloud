using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hanna80_ABCMusic_Auth.Models
{
    public class Review
    {
		public int ReviewId { get; set; }
		public string Content { get; set; }
		public int SongId { get; set; }

		public Song Song { get; set; }
    }
}
