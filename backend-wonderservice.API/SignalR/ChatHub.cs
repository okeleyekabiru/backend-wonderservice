using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace backend_wonderservice.API.SignalR
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(ChatMessage message)
        {

            // invoke this ReceiveMessage method in the client
            // Broadcast to all clients
            // await Clients.Others.SendAsync(
            //     "ReceiveMessage",
            //   message
            // );
            await Clients.All.SendAsync("ReceiveMessage", message);

        }
    }
}
