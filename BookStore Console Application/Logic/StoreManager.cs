using System;
using System.Collections.Generic;
using System.Linq;
using BookStore_Console_Application.Domain;

namespace BookStore_Console_Application.Logic
{
    public class StoreManager
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Purchase> _purchaseRepository;

        public StoreManager(IRepository<Book> bookRepo, IRepository<Customer> custRepo, IRepository<Purchase> purcRepo)
        {
            _bookRepository = bookRepo;
            _customerRepository = custRepo;
            _purchaseRepository = purcRepo;
        }

        // --- Core Operations ---
        public void AddBook(Book book) => _bookRepository.Add(book);

        public void RegisterCustomer(Customer customer) => _customerRepository.Add(customer);

        public void ProcessPurchase(Guid customerId, List<Book> books)
        {
            if (_customerRepository.GetById(customerId) == null)
                throw new InvalidOperationException("Customer not found.");

            if (books == null || !books.Any())
                throw new ArgumentException("A purchase must contain at least one book.");

            // Stock Validation
            foreach (var book in books)
            {
                if (book.Stock <= 0)
                    throw new InvalidOperationException($"'{book.Title}' is out of stock.");

                // This calls the logic in Book.cs, which triggers the Outofstock event if necessary
                book.ReduceStock(1);
            }

            var purchase = new Purchase(customerId, books);
            _purchaseRepository.Add(purchase);
        }

        // --- Filtering (Task 6) ---
        public IEnumerable<Book> FilterBooksByCategory(string category)
            => _bookRepository.GetAll().Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Book> FilterBooksByAuthor(string author)
            => _bookRepository.GetAll().Where(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Book> FilterBooksByPriceRange(decimal min, decimal max)
            => _bookRepository.GetAll().Where(b => b.Price >= min && b.Price <= max);

        // --- Metrics (Task 5) ---
        public decimal CalculateTotalRevenue()
            => _purchaseRepository.GetAll().Sum(p => p.TotalAmount);

        public Book GetBestSellingBook()
        {
            return _purchaseRepository.GetAll()
                .SelectMany(p => p.Items)
                .GroupBy(b => b.Id)
                .OrderByDescending(g => g.Count())
                .Select(g => g.First())
                .FirstOrDefault();
        }

        public Customer GetTopCustomer()
        {
            var topId = _purchaseRepository.GetAll()
                .GroupBy(p => p.CustomerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return _customerRepository.GetById(topId);
        }

        public IEnumerable<Purchase> GetPurchasesByCustomer(Guid customerId)
            => _purchaseRepository.GetAll().Where(p => p.CustomerId == customerId);
    }
}