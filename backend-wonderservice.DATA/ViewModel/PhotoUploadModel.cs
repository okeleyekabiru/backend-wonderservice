using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WonderService.Data.ViewModel
{
  public  class PhotoUploadModel
    {
        [Required]
        public List< IFormFile> Photo { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
