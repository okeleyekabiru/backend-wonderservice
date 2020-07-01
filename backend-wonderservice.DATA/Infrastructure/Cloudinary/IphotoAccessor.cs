using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace backend_wonderservice.DATA.Infrastructure.Cloudinary
{

    public interface IPhotoAccessor
    {
        PhotoUpLoadResult AddPhoto(IFormFile file);
        string DeletePhoto(string publicId);
        List<PhotoUpLoadResult> AddPhotos(List<IFormFile> file);
    }
}
