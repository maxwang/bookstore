﻿using BookStore.Core.Repository;
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
            
        }

        public async Task UpdateReocrd()
        {
            Book book = new Book
            {
                BookId = 1,
                BookName = "My Books",
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

        public async Task InsertRecord()
        {
            Book book = new Book
            {
                BookId = 2,
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
