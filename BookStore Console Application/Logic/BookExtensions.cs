using System;
using System.Collections.Generic;
using System.Linq;
using BookStore_Console_Application.Domain;

namespace BookStore_Console_Application.Logic
{ 
    public static class BookExtensions
    {
     
        public static void ApplyPriceRule(this IEnumerable<Book> books, Func<Book, decimal> rule)
        {
            if (books == null) throw new ArgumentNullException(nameof(books));
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            foreach (var book in books)
            {
                decimal newPrice = rule(book);
                if (newPrice < 0)
                    throw new InvalidOperationException($"Rule produced a negative price for '{book.Title}'.");

                book.Price = newPrice;
            }
        }

       
        public static void ApplyRule(this IEnumerable<Book> books, Action<Book> rule)
        {
            if (books == null) throw new ArgumentNullException(nameof(books));
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            foreach (var book in books)
                rule(book);
        }


        public static void ApplyDiscount(this IEnumerable<Book> books, decimal percentOff)
        {
            if (percentOff < 0 || percentOff > 100)
                throw new ArgumentException("Discount percent must be between 0 and 100.");

            books.ApplyPriceRule(b => Math.Round(b.Price * (1 - percentOff / 100m), 2));
        }

        public static void ApplyMarkup(this IEnumerable<Book> books, decimal percentIncrease)
        {
            if (percentIncrease < 0)
                throw new ArgumentException("Markup percent cannot be negative.");

            books.ApplyPriceRule(b => Math.Round(b.Price * (1 + percentIncrease / 100m), 2));
        }

    
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var words = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();

            return string.Join(' ', words);
        }

        public static string ToCurrency(this decimal amount) => $"${amount:F2}";

        public static void PrintNumbered(this IEnumerable<Book> books)
        {
            var list = books.ToList();
            if (!list.Any())
            {
                Console.WriteLine("No books to display.");
                return;
            }

            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1}. {list[i]}");
        }
    }
}