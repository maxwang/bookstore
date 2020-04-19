using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Models
{
    public interface IBookStoreData
    {
        Task<int> InsertBook(Book book);
        Task<int> UpdateBook(int bookId, Book book);
        Task<bool> AnyBookWithId(int bookId);
    }
}
