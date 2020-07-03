using System;
using System.Collections.Generic;
using System.Text;
using backend_wonderservice.DATA.Infrastructure;
using backend_wonderservice.DATA.Models;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IMailService
    {
       
        void SendMail(string email, string message, string subject);
        string ErrorMessage(string message);
        void VerifyEmail(string email, string message);
        string OrderDetails(Customer customer, string servicesType);
       
    }
}
