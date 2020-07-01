using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_wonderservice.API.SignalR
{
    public class ChatMessage
    {
        public string ClientUniqueId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string FromUserName { get; set; }
        public DateTime Date { get; set; }
    }
}

