using BookStore_API.Contracts;
using BookStore_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        public Task<bool> Create(Book entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Book entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Book>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<Book> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> isExist(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Book entity)
        {
            throw new NotImplementedException();
        }
    }
}
