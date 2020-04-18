using BookStore.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Core.Repository
{
    public class BookStoreService : IBookStore
    {
        private readonly IBookStoreData _db;
        public BookStoreService(IBookStoreData bookStoreData)
        {
            _db = bookStoreData ?? throw new ArgumentException(nameof(bookStoreData));
        }

        public async Task<int> InsertBook(Book book)
        {
            bool isValid = await ValidateBookForInsert(book);
            if(!isValid)
            {
                throw new ValidationException($"Check data: {book.ToString()}");
            }

            return await _db.InsertBook(book);
        }

        //will put some business role here
        private async Task<bool> ValidateBookForInsert(Book book)
        {
            return await Task.FromResult(true);
        }
    }
}
