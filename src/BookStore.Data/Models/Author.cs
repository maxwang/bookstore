using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Data.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public string Name { get; set; }
    }
}
