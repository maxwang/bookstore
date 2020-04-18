using BookStore.Data.Extensions;
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
    }
}
