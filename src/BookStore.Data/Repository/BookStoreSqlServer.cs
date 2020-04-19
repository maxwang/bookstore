using BookStore.Data.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Models
{
    public class BookStoreSqlServer : IBookStoreData
    {
        private readonly string _connectionString;
        public BookStoreSqlServer(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
        }

        public async Task<bool> AnyBookWithId(int bookId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.ExecuteScalarAsync<bool>($"SELECT count(1) from Books WHERE BookId = {bookId}");
            }
        }

        public async Task<int> InsertBook(Book book)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    await conn.InsertItem<Publisher>(book.Publisher, "Publishers", "PublisherId", transaction, (pubblisherId) => book.PublisherId = pubblisherId);
                    await conn.InsertItem<Book>(book, "Books", "BookId", transaction, setIdentityInsert: true);
                    transaction.Commit();
                    return book.BookId;
                }
            }
        }

        public async Task<int> UpdateBook(int bookId, Book book)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    var currentBook = conn.QueryFirstOrDefault<Book>("SELECT * FROM Books where BookId = @bookId", new { bookId });
                    if(currentBook == null)
                    {
                        throw new ArgumentException($"Could not find book for id: {bookId}");
                    }

                    //this logic should in UpsertOneToOneSubItem
                    //await RunFunctionWhenIsNotNull(book.Publisher, () =>
                    //{
                    //    return conn.UpsertOneToOneSubItem(book.Publisher, "Publishers", "PublisherId", transaction);
                    //});

                    await conn.UpsertOneToOneSubItem(book.Publisher, "Publishers", "PublisherId", transaction);
                    return await conn.UpdateItem<Book>(book, "Books", "BookId", transaction);
                }
            }
        }

        private async Task RunFunctionWhenIsNotNull(object data, Func<Task<int>> func)
        {
            if(data != null && func != null)
            {
                await func.Invoke();
            }
        }
    }
}
