using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WonderService.Data.ViewModel
{
  public  class ServicesVm
    {
        [Required]
        public string Body { get; set; }
        [Required]
        public string ServiceType { get; set; }
        public List<IFormFile> Photo { get; set; }

    }
}
