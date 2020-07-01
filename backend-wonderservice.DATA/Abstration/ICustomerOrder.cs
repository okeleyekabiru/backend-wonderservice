using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Models;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Abstration
{
    public interface ICustomerOrder
    {
        Task<ReturnResult> Post(Customer customer);
        Task<List<Customer>> GetAllOrder();
        Task<Customer> GetOrder(string id);
        Task Delete(Customer customer);
        Task Update(Customer customer);
        Task<bool> SaveChanges();

        Task<List<Customer>> OrderNotification();
        Task<List<Customer>> OrderNotificationOnTime();


    }
}
