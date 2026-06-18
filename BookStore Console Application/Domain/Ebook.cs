using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Domain
{
   public class Ebook : Book
    {
        public double FileSizeMb { get; private set; }
        public string FileFormat { get; private set; } // e.g. EPUB, PDF, MOBI

        public Ebook(string title, string author, string category, decimal price, int stock, double fileSizeMb, string fileFormat)
            : base(title, author, category, price, stock)
        {
            if (fileSizeMb <= 0)
                throw new ArgumentException("File size must be greater than zero.");
            if (string.IsNullOrWhiteSpace(fileFormat))
                throw new ArgumentException("File format cannot be empty.");

            FileSizeMb = fileSizeMb;
            FileFormat = fileFormat;
        }

        public override string GetFormatDetails()
            => $"Ebook, {FileSizeMb} MB, format: {FileFormat}";
    }
}

