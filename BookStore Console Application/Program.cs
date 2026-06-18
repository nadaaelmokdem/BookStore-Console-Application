using System;
using System.Collections.Generic;
using System.Linq;
using BookStore_Console_Application;
using BookStore_Console_Application.Domain;
using BookStore_Console_Application.Logic;

var bookRepo = new InMemoryRepository<Book>();
var customerRepo = new InMemoryRepository<Customer>();
var purchaseRepo = new InMemoryRepository<Purchase>();
var storeManager = new StoreManager(bookRepo, customerRepo, purchaseRepo);

bool running = true;

while (running)
{
    Console.WriteLine("\n--- MAIN MENU ---");
    Console.WriteLine("1. Admin | 2. Customer | 3. Exit");
    string mainChoice = Console.ReadLine();

    try
    {
        switch (mainChoice)
        {
            case "1":
                RunAdminMenu();
                break;

            case "2":
                RunCustomerEntryMenu();
                break;

            case "3":
                running = false;
                break;

            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// ===================== ADMIN =====================

void RunAdminMenu()
{
    bool adminRunning = true;
    while (adminRunning)
    {
        Console.WriteLine("\n--- Admin Panel ---");
        Console.WriteLine("1. Add Book");
        Console.WriteLine("2. List Books");
        Console.WriteLine("3. Remove Book");
        Console.WriteLine("4. Filter Books");
        Console.WriteLine("5. View Stats (Revenue / Best Seller / Top Customer)");
        Console.WriteLine("6. Apply Price Rule (Discount/Markup)");
        Console.WriteLine("7. Back");
        string adminChoice = Console.ReadLine();

        try
        {
            switch (adminChoice)
            {
                case "1": AddBookFlow(); break;
                case "2": ListAllBooks(); break;
                case "3": RemoveBookFlow(); break;
                case "4": FilterBooksFlow(); break;
                case "5": ShowStats(); break;
                case "6": ApplyPriceRuleFlow(); break;
                case "7": adminRunning = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

void AddBookFlow()
{
    string title = InputHelper.ReadString("Enter Title: ");
    string author = InputHelper.ReadString("Enter Author: ");
    string cat = InputHelper.ReadString("Enter Category: ");
    decimal price = InputHelper.ReadDecimal("Enter Price: ");
    int stock = InputHelper.ReadInt("Enter Stock: ");

    Console.WriteLine("Format: 1. Paperback | 2. Ebook | 3. Audiobook");
    string formatChoice = Console.ReadLine();

    Book newBook;
    switch (formatChoice)
    {
        case "2":
            double sizeMb = (double)InputHelper.ReadDecimal("File Size (MB): ");
            string fileFormat = InputHelper.ReadString("File Format (e.g. EPUB, PDF): ");
            newBook = new Ebook(title, author, cat, price, stock, sizeMb, fileFormat);
            break;

        case "3":
            int hours = InputHelper.ReadInt("Duration (hours): ");
            string narrator = InputHelper.ReadString("Narrator: ");
            newBook = new Audiobook(title, author, cat, price, stock, TimeSpan.FromHours(hours), narrator);
            break;

        default:
            int pages = InputHelper.ReadInt("Page Count: ");
            newBook = new Paperback(title, author, cat, price, stock, pages);
            break;
    }

    newBook.Outofstock += (sender, bookTitle) =>
        Console.WriteLine($"[ALERT] '{bookTitle}' just went out of stock!");

    bookRepo.Add(newBook);
    Console.WriteLine("Book added successfully!");
}

void ListAllBooks()
{
    var books = bookRepo.GetAll().ToList();
    if (!books.Any())
    {
        Console.WriteLine("No books in the store yet.");
        return;
    }

    Console.WriteLine("\n--- Available Books ---");
    for (int i = 0; i < books.Count; i++)
        Console.WriteLine($"{i + 1}. {books[i]}");
}

void RemoveBookFlow()
{
    var books = bookRepo.GetAll().ToList();
    if (!books.Any())
    {
        Console.WriteLine("No books to remove.");
        return;
    }

    ListAllBooks();
    int choice = InputHelper.ReadInt("Enter the number of the book to remove: ");

    if (choice < 1 || choice > books.Count)
    {
        Console.WriteLine("Invalid selection.");
        return;
    }

    bookRepo.Remove(books[choice - 1]);
    Console.WriteLine("Book removed.");
}

void FilterBooksFlow()
{
    Console.WriteLine("Filter by: 1. Category | 2. Author | 3. Price Range");
    string choice = Console.ReadLine();

    IEnumerable<Book> results;
    switch (choice)
    {
        case "1":
            string category = InputHelper.ReadString("Category: ");
            results = storeManager.FilterBooksByCategory(category);
            break;

        case "2":
            string author = InputHelper.ReadString("Author: ");
            results = storeManager.FilterBooksByAuthor(author);
            break;

        case "3":
            decimal min = InputHelper.ReadDecimal("Min Price: ");
            decimal max = InputHelper.ReadDecimal("Max Price: ");
            results = storeManager.FilterBooksByPriceRange(min, max);
            break;

        default:
            Console.WriteLine("Invalid option.");
            return;
    }

    var resultList = results.ToList();
    if (!resultList.Any())
    {
        Console.WriteLine("No books matched.");
        return;
    }

    Console.WriteLine("\n--- Matching Books ---");
    foreach (var b in resultList)
        Console.WriteLine(b.ToString());
}

void ShowStats()
{
    Console.WriteLine("\n--- Store Stats ---");
    Console.WriteLine($"Total Revenue: ${storeManager.CalculateTotalRevenue():F2}");

    var bestSeller = storeManager.GetBestSellingBook();
    Console.WriteLine(bestSeller != null
        ? $"Best-Selling Book: {bestSeller.Title}"
        : "Best-Selling Book: N/A (no purchases yet)");

    var topCustomer = storeManager.GetTopCustomer();
    Console.WriteLine(topCustomer != null
        ? $"Top Customer: {topCustomer.Name}"
        : "Top Customer: N/A (no purchases yet)");
}
void ApplyPriceRuleFlow()
{
    var books = bookRepo.GetAll().ToList();
    if (!books.Any())
    {
        Console.WriteLine("No books to adjust.");
        return;
    }

    Console.WriteLine("1. Apply Discount | 2. Apply Markup");
    string choice = Console.ReadLine();

    decimal percent = InputHelper.ReadDecimal("Enter percentage: ");

    if (choice == "1")
        books.ApplyDiscount(percent);
    else if (choice == "2")
        books.ApplyMarkup(percent);
    else
    {
        Console.WriteLine("Invalid option.");
        return;
    }

    Console.WriteLine("Price rule applied. Updated prices:");
    books.PrintNumbered();
}

// ===================== CUSTOMER =====================

void RunCustomerEntryMenu()
{
    Console.WriteLine("\n1. New Account | 2. Existing Account | 3. Back");
    string custChoice = Console.ReadLine();

    if (custChoice == "1")
    {
        var newCustomer = RegisterCustomerFlow();
        if (newCustomer != null)
            RunCustomerSessionMenu(newCustomer);
    }
    else if (custChoice == "2")
    {
        string idInput = InputHelper.ReadString("Enter your Customer ID: ");
        if (!Guid.TryParse(idInput, out Guid loginId))
        {
            Console.WriteLine("That doesn't look like a valid ID.");
            return;
        }

        var cust = customerRepo.GetById(loginId);
        if (cust == null)
        {
            Console.WriteLine("Account not found.");
            return;
        }

        Console.WriteLine($"Welcome back, {cust.Name}!");
        RunCustomerSessionMenu(cust);
    }
    // "3" or anything else just falls back to the main menu
}

Customer RegisterCustomerFlow()
{
    string name = InputHelper.ReadString("Name: ");
    string addr = InputHelper.ReadString("Address: ");
    string city = InputHelper.ReadString("City: ");
    string reg = InputHelper.ReadString("Region: ");
    string zip = InputHelper.ReadString("Postal Code: ");
    string country = InputHelper.ReadString("Country: ");
    string phone = InputHelper.ReadString("Phone: ");

    var newCustomer = new Customer(name, addr, city, reg, zip, country, phone);
    storeManager.RegisterCustomer(newCustomer);

    Console.WriteLine("Account created!");
    Console.WriteLine($"Your Customer ID is: {newCustomer.Id}");
    Console.WriteLine("Save this ID — you'll need it to log in next time.");

    return newCustomer;
}

void RunCustomerSessionMenu(Customer customer)
{
    bool sessionRunning = true;
    while (sessionRunning)
    {
        Console.WriteLine($"\n--- Customer Menu ({customer.Name}) ---");
        Console.WriteLine("1. Browse / Search Books");
        Console.WriteLine("2. Make a Purchase");
        Console.WriteLine("3. View My Past Orders");
        Console.WriteLine("4. Back");
        string choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1": BrowseBooksFlow(); break;
                case "2": MakePurchaseFlow(customer); break;
                case "3": ViewOrderHistoryFlow(customer); break;
                case "4": sessionRunning = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

void BrowseBooksFlow()
{
    Console.WriteLine("\n1. View All Books | 2. Search by Category | 3. Search by Author | 4. Search by Price Range");
    string choice = Console.ReadLine();

    IEnumerable<Book> results;
    switch (choice)
    {
        case "2":
            results = storeManager.FilterBooksByCategory(InputHelper.ReadString("Category: "));
            break;
        case "3":
            results = storeManager.FilterBooksByAuthor(InputHelper.ReadString("Author: "));
            break;
        case "4":
            decimal min = InputHelper.ReadDecimal("Min Price: ");
            decimal max = InputHelper.ReadDecimal("Max Price: ");
            results = storeManager.FilterBooksByPriceRange(min, max);
            break;
        default:
            results = bookRepo.GetAll();
            break;
    }

    var resultList = results.ToList();
    if (!resultList.Any())
    {
        Console.WriteLine("No books found.");
        return;
    }

    Console.WriteLine("\n--- Books ---");
    for (int i = 0; i < resultList.Count; i++)
        Console.WriteLine($"{i + 1}. {resultList[i]}");
}

void MakePurchaseFlow(Customer customer)
{
    var books = bookRepo.GetAll().ToList();
    if (!books.Any())
    {
        Console.WriteLine("No books available to purchase.");
        return;
    }

    Console.WriteLine("\n--- Available Books ---");
    for (int i = 0; i < books.Count; i++)
        Console.WriteLine($"{i + 1}. {books[i]}");

    Console.Write("\nEnter the numbers of the books you want, separated by commas (e.g. 1,3,4): ");
    string input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("No selection made.");
        return;
    }

    var selectedBooks = new List<Book>();
    var invalidEntries = new List<string>();

    foreach (var token in input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
    {
        if (int.TryParse(token, out int index) && index >= 1 && index <= books.Count)
        {
            selectedBooks.Add(books[index - 1]);
        }
        else
        {
            invalidEntries.Add(token);
        }
    }

    if (invalidEntries.Any())
        Console.WriteLine($"Ignored invalid entries: {string.Join(", ", invalidEntries)}");

    if (!selectedBooks.Any())
    {
        Console.WriteLine("No valid books were selected. Purchase cancelled.");
        return;
    }

    // Show a summary before committing, since this can fail on stock validation
    Console.WriteLine("\nYou selected:");
    foreach (var b in selectedBooks)
        Console.WriteLine($"  - {b.Title} (${b.Price:F2})");

    storeManager.ProcessPurchase(customer.Id, selectedBooks);

    decimal total = selectedBooks.Sum(b => b.Price);
    Console.WriteLine($"Purchase successful! Total: ${total:F2}");
}

void ViewOrderHistoryFlow(Customer customer)
{
    var orders = storeManager.GetPurchasesByCustomer(customer.Id).ToList();
    if (!orders.Any())
    {
        Console.WriteLine("You haven't made any purchases yet.");
        return;
    }

    Console.WriteLine($"\n--- Order History for {customer.Name} ---");
    foreach (var order in orders.OrderByDescending(o => o.PurchaseDate))
    {
        Console.WriteLine($"\nOrder {order.Id} - {order.PurchaseDate:g} - Total: ${order.TotalAmount:F2}");
        foreach (var item in order.Items)
            Console.WriteLine($"   • {item.Title} (${item.Price:F2})");
    }
}