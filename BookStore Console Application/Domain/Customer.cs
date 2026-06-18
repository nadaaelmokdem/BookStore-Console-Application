using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Domain
{
    public  class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        protected Customer(string name, string address, string city, string region, string postalCode, string country, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.");
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.");
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region cannot be empty.");
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty.");
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            City = city;
            Region = region;
            PostalCode = postalCode;
            Country = country;
            Phone = phone;
        }
        public override string ToString()
        {
            return $"{Name} - {City}, {Country} (Ph: {Phone})";
        }

    }
}
