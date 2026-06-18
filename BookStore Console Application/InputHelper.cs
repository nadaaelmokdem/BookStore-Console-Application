using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application
{
   
        public static class InputHelper
        {
            public static string ReadString(string prompt)
            {
                string input;
                do
                {
                    Console.Write(prompt);
                    input = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(input));
                return input;
            }

            public static int ReadInt(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (int.TryParse(Console.ReadLine(), out int result)) return result;
                    Console.WriteLine("Invalid number. Please try again.");
                }
            }

            public static decimal ReadDecimal(string prompt)
            {
                while (true)
                {
                    Console.Write(prompt);
                    if (decimal.TryParse(Console.ReadLine(), out decimal result)) return result;
                    Console.WriteLine("Invalid decimal. Please try again.");
                }
            }
        }
    }