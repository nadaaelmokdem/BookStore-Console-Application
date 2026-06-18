using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Domain
{
    public class Audiobook : Book
    {
        public TimeSpan Duration { get; private set; }
        public string Narrator { get; private set; }

        public Audiobook(string title, string author, string category, decimal price, int stock, TimeSpan duration, string narrator)
            : base(title, author, category, price, stock)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("Duration must be greater than zero.");
            if (string.IsNullOrWhiteSpace(narrator))
                throw new ArgumentException("Narrator cannot be empty.");

            Duration = duration;
            Narrator = narrator;
        }

        public override string GetFormatDetails()
            => $"Audiobook, {Duration.TotalHours:F1} hrs, narrated by {Narrator}";
    }
}
