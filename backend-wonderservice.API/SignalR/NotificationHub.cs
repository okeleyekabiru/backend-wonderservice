using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.SignalR;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.SignalR
{
    public class NotificationHUb : Hub
    {
        private readonly ICustomerOrder _customer;
        private readonly IMapper _mapper;

        public NotificationHUb(ICustomerOrder customer, IMapper mapper)
        {
            _customer = customer;
            _mapper = mapper;
        }

        public async Task SendNotification()
        {
            var notification = await _customer.OrderNotification();
            if (notification.Count > 0)
            {
                var model = _mapper.Map<List<Customer>, List<OrderModel>>(notification);
                await Clients.All.SendAsync("ReceiveMessage", model);

            }
        }
    }
}
