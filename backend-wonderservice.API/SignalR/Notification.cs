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
    public class Notification : INotification
    {
        private readonly ICustomerOrder _customerOrder;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHUb> _hubContext;

        public Notification(ICustomerOrder customerOrder, IHubContext<NotificationHUb> hubContext, IMapper mapper)
        {
            _customerOrder = customerOrder;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task SendNotification()
        {



            var notification = await _customerOrder.OrderNotification();

            if (notification.Count > 0)
            {
                var model = _mapper.Map<List<Customer>, List<OrderModel>>(notification);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", model);
            }

            var onTimeNotification = await _customerOrder.OrderNotificationOnTime();
            if (onTimeNotification.Count > 0)
            {
                var model = _mapper.Map<List<Customer>, List<OrderModel>>(onTimeNotification);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", onTimeNotification, "notify");
            }

            Console.WriteLine("sending notification");
        }

    }
}
