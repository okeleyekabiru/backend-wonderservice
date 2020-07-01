using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_wonderservice.API.SignalR
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessage(string message);
        Task ReceiveMessage(ChatMessage message, string id);
    }
}
