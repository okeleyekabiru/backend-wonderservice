using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.EntityFrameworkCore;
using WonderService.Data.Services;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Repo
{
  public class CustomerOrderRepo : ICustomerOrder
  {
    private readonly DataContext _context;
    private readonly IMailService _serviceMail;
    private readonly IServiceType _serviceType;

    public CustomerOrderRepo(DataContext context, IMailService serviceMail, IServiceType serviceType)
    {
      _context = context;
      _serviceMail = serviceMail;
      _serviceType = serviceType;
    }
    public async Task<ReturnResult> Post(Customer customer)
    {

      await _context.AddAsync(customer);
      if (await _context.SaveChangesAsync() > 0)
      {
        var service = _serviceType.Get(customer.ServicesTypeId.ToString());
        _serviceMail.SendMail(customer.Email, _serviceMail.OrderDetails(customer, service.ServiceType), "booking");
        return new ReturnResult
        {
          Succeeded = true
        };
      }
      return new ReturnResult
      {
        Error = "An Error Occured While uploading order"
      };
    }

    public async Task<List<Customer>> GetAllOrder()
    {
      return await _context.Customer.Include(r => r.ServicesType).Include(r => r.LocalGovernment).Include(s => s.State).ToListAsync();
    }

    public async Task<Customer> GetOrder(string id)
    {
      return await _context.Customer.Include(r => r.ServicesType).Include(r => r.State).Include(r => r.LocalGovernment).Where(r => r.Id.Equals(Guid.Parse(id))).FirstOrDefaultAsync();
    }

    public Task Delete(Customer customer)
    {
      _context.Remove(customer);
      return Task.CompletedTask;
    }

    public Task Update(Customer customer)
    {
      _context.Update(customer);
      return Task.CompletedTask;
    }

    public async Task<bool> SaveChanges()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<Customer>> OrderNotification()
    {
      var dateTime = DateTime.Now.AddHours(12);
      return await _context.Customer.Where(r => r.EntryDate.Date.Equals(dateTime.Date)).ToListAsync();
    }

    public async Task<List<Customer>> OrderNotificationOnTime()
    {
      var dateTime = DateTime.Now;
      return await _context.Customer.Where(r => r.EntryDate.Equals(dateTime)).ToListAsync();
    }
  }
}
