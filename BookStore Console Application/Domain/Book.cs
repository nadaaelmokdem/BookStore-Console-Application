using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Domain
{
    public abstract class Book
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Title { get; set; }
        string Category { get; set; }

        string Author { get; set; }

        decimal Price { get; set; }

        int Stock { get; set; }

        public EventHandler<string>? Outofstock;

        protected Book(string title, string author, string category, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty.");
            if (price < 0)
                throw new ArgumentException("Price cannot be negative.");
            if (stock < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");

            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            Category = category;
            Price = price;
            Stock = stock;
        }
        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.");
            if (quantity > Stock)
                throw new InvalidOperationException($"Not enough stock for '{Title}'. Available: {Stock}.");

            Stock -= quantity;

            if (Stock == 0)
                Outofstock?.Invoke(this, Title);
        }
        public abstract string GetFormatDetails();

        public override string ToString()
            => $"{Title} by {Author} [{Category}] - ${Price:F2} ({Stock} in stock) - {GetFormatDetails()}";
    }
}
   