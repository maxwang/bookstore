using System;
using System.Collections.Generic;
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
        public Task<int> InsertBook(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
