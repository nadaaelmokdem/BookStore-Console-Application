using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BookStore_Console_Application.Domain
{
    public class Purchase
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public List<Book> Items { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime PurchaseDate { get; private set; }

        public Purchase(Guid customerId, List<Book> books)
        {
            if (books == null || !books.Any())
                throw new ArgumentException("A purchase must contain at least one book.");

            Id = Guid.NewGuid();
            CustomerId = customerId;
            Items = new List<Book>(books);
            PurchaseDate = DateTime.Now;

            TotalAmount = Items.Sum(b => b.Price);
        }
    }
}
