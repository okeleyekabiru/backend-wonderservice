using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WonderService.Data.ViewModel
{
   public class OrderModelUpdate
    {[Required]
        public string Id { get; set; }
     
        public string Address { get; set; }
      
        [EmailAddress(ErrorMessage = "invalid email")]
        public string Email { get; set; }
   
        public string PhoneNumber { get; set; }
       
        public string FirstName { get; set; }
     
        public string LastName { get; set; }
      
        public string ServiceType { get; set; }
    }
}
