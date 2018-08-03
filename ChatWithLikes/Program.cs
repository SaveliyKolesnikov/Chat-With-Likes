using System;
using Microsoft.Extensions.Configuration;

namespace ChatWithLikes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Chat with likes!");
            Console.ForegroundColor = ConsoleColor.White;

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            var conStr = config["connectionString"];
        }
    }
}
