using DbUp;
using DbUp.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Database
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: true)
                   .AddEnvironmentVariables()
                   .Build();

            var connectionString =
                    configuration.GetConnectionString("Database");

            var builder = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole();

            builder = builder.JournalToSqlTable("dbo", "DatabaseMigrations");

            var executor = builder.Build();
            var result = executor.PerformUpgrade();

            WriteToConsole(result.Successful ? "Migration has been successful!" : "Migration failed");
        }

        private static void WriteToConsole(string msg, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
