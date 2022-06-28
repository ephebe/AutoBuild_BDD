using System;
using System.Threading.Tasks;
using CustomerApi.Models;

namespace CustomerApi.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> FindCustomerById(Guid id);
        Task<bool> DeleteCustomer(Guid id);
    }
}