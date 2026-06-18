using System;
using System.Collections.Generic;
using System.Linq;
using BookStore_Console_Application;
using BookStore_Console_Application.Domain;
using BookStore_Console_Application.Logic;

// Initialize Repositories and StoreManager
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
            case "1": // ADMIN MENU
                bool adminRunning = true;
                while (adminRunning)
                {
                    Console.WriteLine("\n--- Admin Panel ---");
                    Console.WriteLine("1. Add Book | 2. List Books | 3. Record Purchase | 4. Back");
                    string adminChoice = Console.ReadLine();

                    switch (adminChoice)
                    {
                        case "1":
                            string title = InputHelper.ReadString("Enter Title: ");
                            string author = InputHelper.ReadString("Enter Author: ");
                            string cat = InputHelper.ReadString("Enter Category: ");
                            decimal price = InputHelper.ReadDecimal("Enter Price: ");
                            int stock = InputHelper.ReadInt("Enter Stock: ");

                            var newBook = new Paperback(title, author, cat, price, stock, 100);
                            bookRepo.Add(newBook);
                            Console.WriteLine("Book added successfully!");
                            break;

                        case "2":
                            Console.WriteLine("\n--- Available Books ---");
                            foreach (var b in bookRepo.GetAll()) Console.WriteLine(b.ToString());
                            break;

                        case "3":
                            string cidInput = InputHelper.ReadString("Enter Customer ID: ");
                            if (Guid.TryParse(cidInput, out Guid custId))
                            {
                                var bookToBuy = bookRepo.GetAll().FirstOrDefault();
                                if (bookToBuy != null)
                                {
                                    storeManager.ProcessPurchase(custId, new List<Book> { bookToBuy });
                                    Console.WriteLine("Purchase recorded!");
                                }
                                else Console.WriteLine("No books available.");
                            }
                            break;

                        case "4": adminRunning = false; break;
                    }
                }
                break;

            case "2": // CUSTOMER MENU
                Console.WriteLine("\n1. New Account | 2. Existing Account | 3. Back");
                string custChoice = Console.ReadLine();

                if (custChoice == "1") // Register
                {
                    string name = InputHelper.ReadString("Name: ");
                    string addr = InputHelper.ReadString("Address: ");
                    string city = InputHelper.ReadString("City: ");
                    string reg = InputHelper.ReadString("Region: ");
                    string zip = InputHelper.ReadString("Postal Code: ");
                    string country = InputHelper.ReadString("Country: ");
                    string phone = InputHelper.ReadString("Phone: ");

                    // Note: Your Customer constructor is protected in the file provided, 
                    // ensure your StoreManager handles the instantiation.
                    storeManager.RegisterCustomer(new Customer(name, addr, city, reg, zip, country, phone));
                    Console.WriteLine("Account created!");
                }
                else if (custChoice == "2") // Login
                {
                    string idInput = InputHelper.ReadString("Enter your Customer ID: ");
                    if (Guid.TryParse(idInput, out Guid loginId))
                    {
                        var cust = customerRepo.GetById(loginId);
                        Console.WriteLine(cust != null ? $"Welcome back, {cust.Name}!" : "Account not found.");
                    }
                }
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