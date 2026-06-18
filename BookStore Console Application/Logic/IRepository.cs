using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Console_Application.Logic
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(T entity);
        IEnumerable<T> GetAll();
        T GetById(Guid id);
    }
}
