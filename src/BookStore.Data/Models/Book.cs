﻿using System.Collections;
using System.Collections.Generic;

namespace BookStore.Data.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }
        public virtual IEnumerable<Author> Authors { get; set; }
    }
}
