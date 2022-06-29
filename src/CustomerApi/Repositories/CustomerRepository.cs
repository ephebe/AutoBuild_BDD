using CustomerApi.ErrorHandling;
using CustomerApi.Models;
using CustomerApi.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Repositories
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly BDDContext _context;

        public CustomerRepository(BDDContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
           
                var existCustomer = _context.Customers.FirstOrDefault(s => s.Email == customer.Email);
                if (existCustomer != null) {
                    throw new EmailAlreadyInUseException();
                }
                customer.Id = Guid.NewGuid();

                _context.Add(customer);

                await _context.SaveChangesAsync();

                return customer;            
        }

        public async Task<Customer> FindCustomerById(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            return customer;
        }

        public async Task <bool> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            return _context.SaveChanges() > 0;
        }
    }
}