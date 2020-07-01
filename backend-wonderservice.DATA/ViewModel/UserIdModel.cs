using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WonderService.Data.ViewModel
{
  public  class UserIdModel
    {
        [Required]
        public string Id { get; set; }
    }
}
