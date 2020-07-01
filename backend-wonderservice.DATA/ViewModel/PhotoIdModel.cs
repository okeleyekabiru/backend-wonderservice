using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WonderService.Data.ViewModel
{
  public   class PhotoIdModel
    {
        [Required]
        public string Id { get; set; }
    }
}
