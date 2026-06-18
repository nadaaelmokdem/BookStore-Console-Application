using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Domain
{
    public class Paperback : Book
    {
        public int PageCount { get; private set; }

        public Paperback(string title, string author, string category, decimal price, int stock, int pageCount)
            : base(title, author, category, price, stock)
        {
            if (pageCount <= 0)
                throw new ArgumentException("Page count must be greater than zero.");

            PageCount = pageCount;
        }

        public override string GetFormatDetails()
            => $"Paperback, {PageCount} pages";
    }
}
