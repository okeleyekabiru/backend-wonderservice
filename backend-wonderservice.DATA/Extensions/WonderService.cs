using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Infrastructure.Cloudinary;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Http;
using Plugins.JwtHandler;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Extensions
{
    public static class WonderService
    {
        public static async Task<ReturnResult> Upload(this List<IFormFile> model, IPhoto photo, IPhotoAccessor photoAccessor, Guid serviceId, bool multiple)
        {

            List<PhotoUpLoadResult> result = null;
            List<Photo> photos = new List<Photo>();
            result = photoAccessor.AddPhotos(model);
            if (result != null)
            {
                foreach (var item in result)
                {
                    var upload = new Photo();
                    upload.PublicId = item.PublicId;
                    upload.Url = item.Url;
                    upload.TimeUpload = DateTime.Now;
                    // remember to add service id
                    upload.ServicesId = serviceId;
                    photos.Add(upload);
                }

                await photo.AddRange(photos);
                return new ReturnResult
                {
                    Succeeded = true
                };

            }

            ;

            return new ReturnResult
            {
                Error = "Error uploading image"
            };
        }
        public static ReturnResult Delete(this ICollection<Photo> photos, IPhotoAccessor photoAccessor)
        {
            var result = new ReturnResult();
            var response = new List<string>();
            foreach (var photo in photos)
            {
                var ok = photoAccessor.DeletePhoto(photo.PublicId);
                if (ok == null)
                {
                    result.Error = "failed to delete from remote server";
                    return result;
                }
                response.Add(ok);
            }

            if (response.Count > 0) result.Succeeded = true;
            return result;
        }
        public static async Task<ReturnResult> Upload(this PhotoUploadModel model, IPhoto photo, IPhotoAccessor photoAccessor, Guid serviceId, bool multiple)
        {
            List<PhotoUpLoadResult> result = null;
            List<Photo> photos = new List<Photo>();
            result = photoAccessor.AddPhotos(model.Photo);
            if (result != null)
            {
                foreach (var item in result)
                {
                    var upload = new Photo();
                    upload.PublicId = item.PublicId;
                    upload.Url = item.Url;
                    upload.TimeUpload = DateTime.Now;
                    // remember to add service id
                    upload.ServicesId = serviceId;
                    photos.Add(upload);
                }

                await photo.AddRange(photos);
                return await photo.SaveChanges();
            }

            ;

            return new ReturnResult
            {
                Error = "Error uploading image"
            };
        }


        public static async Task<ReturnResult> Delete(this Photo photo, IPhoto photoRepo, IPhotoAccessor photoAccessor
            )
        {

            var delete = photoAccessor.DeletePhoto(photo.PublicId);
            if (string.IsNullOrEmpty(delete))
            {
                return new ReturnResult
                {
                    Error = "An error occured when delete from remote server"
                };
            }

            await photoRepo.Delete(photo);
            return await photoRepo.SaveChanges();



        }

        public static async Task<ReturnResult> Update(this UserViewModel userViewModel, IUser userRepo, IMailService service, IJwtSecurity jwtSecurity)
        {
            var user = await userRepo.Get(userViewModel.Id);
            if (userViewModel.Email != null)
            {
                user.EmailConfirmed = false;
                var token = jwtSecurity.CreateTokenForEmail(user);
                service.VerifyEmail(userViewModel.Email, token.Token);
            }
            user.FirstName = userViewModel.FirstName ?? user.FirstName;
            user.LastName = userViewModel.LastName ?? user.LastName;
            user.Email = userViewModel.Email ?? user.Email;
            user.PhoneNumber = userViewModel.PhoneNumber ?? user.PhoneNumber;
            var result = await userRepo.Update(user);
            if (result.Succeeded) return new ReturnResult { Succeeded = true };

            return new ReturnResult { Error = result.Error };
        }

    }
}
