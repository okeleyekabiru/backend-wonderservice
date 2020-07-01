using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_wonderservice.API.SignalR;
using Hangfire;

namespace backend_wonderservice.API.BackgroundJob
{
    public class BackgroundJob: BackgroundJobServer
    {
        public BackgroundJob()
        {

            RecurringJob.AddOrUpdate<INotification>(recurringJobId: "notification", o => o.SendNotification(), "00 18 * * *");
            RecurringJob.AddOrUpdate<INotification>(recurringJobId: "notification", o => o.SendNotification(), "00 7 * * *");
        }
    }
}
