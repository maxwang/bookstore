﻿using BookStore.Core.Models;
using BookStore.Core.Repository;
using BookStore.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookStoreConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<App>().Run();
            Console.ReadLine();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            string conStr = config.GetConnectionString("DefaultConnection");
            services.AddScoped<IBookStoreData>(c => new BookStoreSqlServer(conStr));
            services.Configure<BookStoreOptions>(config.GetSection("BookStoreOptions"));
            services.AddTransient<IBookStore, BookStoreService>();
            services.AddTransient<App>();
            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var environmentName =
                Environment.GetEnvironmentVariable("CORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

    }
}
