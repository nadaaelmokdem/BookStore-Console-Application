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

        public StoreManager(IRepository<Book> bookRepository, IRepository<Customer> customerRepository, IRepository<Purchase> purchaseRepository)
        {
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _purchaseRepository = purchaseRepository;
        }


        public void RegisterCustomer(string name, string address, string city, string region, string postalCode, string country, string phone)
        {
            var customer = new Customer(name, address, city, region, postalCode, country, phone);
            _customerRepository.Add(customer);
        }

        public void RecordPurchase(Guid customerId, List<Book> books)
        {
            var customer = _customerRepository.GetById(customerId);
            if (customer == null)
                throw new InvalidOperationException("Customer not found.");

            if (books == null || !books.Any())
                throw new ArgumentException("A purchase must contain at least one book.");

            foreach (var book in books)
            {
                if (book.Stock <= 0)
                    throw new InvalidOperationException($"Cannot sell '{book.Title}' because it is out of stock.");
            }

            foreach (var book in books)
            {
                book.ReduceStock(1);
            }

            // Create and store purchase
            var purchase = new Purchase(customerId, books);
            _purchaseRepository.Add(purchase);
        }


        public void AddBook(Book book) => _bookRepository.Add(book);

        public void RemoveBook(Book book) => _bookRepository.Remove(book);

        public IEnumerable<Book> GetAllBooks() => _bookRepository.GetAll();

        public IEnumerable<Book> FilterBooksByCategory(string category)
            => _bookRepository.GetAll().Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Book> FilterBooksByAuthor(string author)
            => _bookRepository.GetAll().Where(b => b.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Book> FilterBooksByPriceRange(decimal minPrice, decimal maxPrice)
            => _bookRepository.GetAll().Where(b => b.Price >= minPrice && b.Price <= maxPrice);

        public void ApplyDeveloperRuleToBooks(Action<Book> rule)
        {
            foreach (var book in _bookRepository.GetAll())
            {
                rule(book);
            }
        }


        public decimal CalculateTotalRevenue()
        {
            return _purchaseRepository.GetAll().Sum(p => p.TotalAmount);
        }

        public Book GetBestSellingBook()
        {
            var allSoldBooks = _purchaseRepository.GetAll().SelectMany(p => p.Items);
            if (!allSoldBooks.Any()) return null;

            return allSoldBooks
                .GroupBy(b => b.Id)
                .OrderByDescending(g => g.Count())
                .Select(g => g.First())
                .FirstOrDefault();
        }

        public Customer GetTopCustomer()
        {
            var allPurchases = _purchaseRepository.GetAll();
            if (!allPurchases.Any()) return null;

            var topCustomerId = allPurchases
                .GroupBy(p => p.CustomerId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return _customerRepository.GetById(topCustomerId);
        }
    }
}