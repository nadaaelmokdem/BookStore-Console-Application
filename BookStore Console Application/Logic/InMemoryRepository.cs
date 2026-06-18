using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BookStore_Console_Application.Logic
{
    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T entity) => _items.Add(entity);
        public void Remove(T entity) => _items.Remove(entity);
        public IEnumerable<T> GetAll() => _items.ToList();

        public T GetById(Guid id) => _items.FirstOrDefault(x => (Guid)x.GetType().GetProperty("Id").GetValue(x) == id);
    }
}