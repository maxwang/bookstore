using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Repository
{
    public interface IBookStore
    {
        Task<int> InsertBook(Book book);
    }
}
