using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WonderService.Data.ViewModel
{
   public class ServicesUpdateVm
    {
        [Required]
        public string Body { get; set; }
        [Required]
        public string Id { get; set; }
        public string ServiceType { get; set; }
    }
}
