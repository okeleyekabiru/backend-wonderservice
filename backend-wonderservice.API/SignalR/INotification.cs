using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_wonderservice.API.SignalR
{
    public interface INotification
    {
        Task SendNotification();
    }
}
