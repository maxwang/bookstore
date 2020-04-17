using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using BookStore.Core.Repository;

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

            Console.WriteLine(await Task.FromResult("From App.cs"));
        }
    }
}
