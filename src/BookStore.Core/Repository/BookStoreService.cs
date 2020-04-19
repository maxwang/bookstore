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

        public async Task<int> UpdateBook(int bookId, Book book)
        {
            bool isValid = await ValidateBookForUpdate(bookId, book);
            if (!isValid)
            {
                throw new ValidationException($"Check data: {book.ToString()}");
            }

            return await _db.UpdateBook(bookId, book);
        }

        private async Task<bool> ValidateBookForUpdate(int bookId, Book book)
        {
            if (bookId < 0 || book == null)
            {
                return false;
            }

            if(book.BookId != bookId)
            {
                return false;
            }

            return await _db.AnyBookWithId(bookId);
        }

        //will put some business role here
        private async Task<bool> ValidateBookForInsert(Book book)
        {
            return await Task.FromResult(true);
        }
    }
}
