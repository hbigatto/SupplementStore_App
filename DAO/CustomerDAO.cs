using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;


namespace Casestudy.DAL.DAO
{
    public class CustomerDAO
    {
        private AppDbContext _db;
        public CustomerDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<Customer> Register(Customer customer)
        {
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer;
        }
        public async Task<Customer> GetByEmail(string email)
        {
            Customer customer = await _db.Customers.FirstOrDefaultAsync(u => u.Email == email);
            return customer;
        }
    }
}
