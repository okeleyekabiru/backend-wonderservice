using System;
using System.Collections.Generic;
using System.Text;

namespace WonderService.Data.ViewModel
{
  public  class PhotoVm
 {
     public Guid Id { get; set; }
     public string Url { get; set; }
     public DateTime? TimeUpload { get; set; }
     public DateTime? UpdatedTime { get; set; }
    }
}
