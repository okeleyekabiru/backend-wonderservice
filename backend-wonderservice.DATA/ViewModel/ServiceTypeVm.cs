using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WonderService.Data.ViewModel
{
  public  class ServiceTypeVm
    {
        public string Id { get; set; }
        [Required]
        public string ServiceType { get; set; }
    }
}
