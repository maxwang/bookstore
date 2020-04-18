using BookStore.Core.Repository;
using BookStore.Data.Models;
using System;
using System.Threading.Tasks;

namespace BookStoreConsole
{
    public class App
    {
        private readonly IBookStore _bookStore;

        public App(IBookStore bookStore)
        {
            _bookStore = bookStore ?? throw new ArgumentNullException(nameof(bookStore));
        }
        public async Task Run()
        {
            Book book = new Book
            {
                BookName = "Dapper Best Practice",
                Publisher = new Publisher
                {
                    Name = "Addison Wesley"
                }
            };

            try
            {
                var bookId = await _bookStore.InsertBook(book);
                Console.WriteLine($"Book inserted: {bookId}:{book.BookName}");
            }

            catch (Exception ex)
            {

                Console.WriteLine("Exception:" + ex.Message);
            }


            Console.WriteLine(await Task.FromResult("From App.cs"));
        }
    }
}
