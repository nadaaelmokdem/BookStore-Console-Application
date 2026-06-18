using System;
using BookStore_Console_Application.Domain;
using BookStore_Console_Application.Logic;
using System.Collections.Generic;
using BookStore_Console_Application;



var bookRepo = new InMemoryRepository<Book>();
var customerRepo = new InMemoryRepository<Customer>();
var purchaseRepo = new InMemoryRepository<Purchase>();
var storeManager = new StoreManager(bookRepo, customerRepo, purchaseRepo);

bool running = true;
while (running)
{
    Console.WriteLine("1. Add Book | 2. List Books | 3. Record Purchase | 4. Exit");
    string choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1": // Add Book
                string title = InputHelper.ReadString("Enter Title: ");
                string author = InputHelper.ReadString("Enter Author: ");
                string cat = InputHelper.ReadString("Enter Category: ");
                decimal price = InputHelper.ReadDecimal("Enter Price: ");
                int stock = InputHelper.ReadInt("Enter Stock: ");

                // Note: You would instantiate the specific book type here (e.g., Paperback)
                var newBook = new Paperback(title, author, cat, price, stock, 100);
                bookRepo.Add(newBook);
                Console.WriteLine("Book added successfully!");
                break;

            case "2": // List Books
                Console.WriteLine("\n--- Available Books ---");
                foreach (var b in bookRepo.GetAll())
                {
                    Console.WriteLine(b.ToString());
                }
                break;

            case "3":
                Guid custId = Guid.Parse(InputHelper.ReadString("Enter Customer ID: "));
                var bookToBuy = bookRepo.GetAll().FirstOrDefault();
                if (bookToBuy != null)
                {
                    storeManager.RecordPurchase(custId, new List<Book> { bookToBuy });
                    Console.WriteLine("Purchase recorded!");
                }
                else
                {
                    Console.WriteLine("No books available to purchase.");
                }
                break;
            case "4":
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