using BookStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Repository
{
    public interface IBookStore
    {
        Task<int> InsertBook(Book book);
        Task<int> UpdateBook(int bookId, Book book);
    }
}
