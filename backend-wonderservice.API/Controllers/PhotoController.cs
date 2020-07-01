using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Extensions;
using backend_wonderservice.DATA.Infrastructure.Cloudinary;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WonderService.Data.Security;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.Controllers
{
    [Route("api/photo")]
    [ApiController]
    [Authorize(Roles = Role.Admin, AuthenticationSchemes = "Bearer")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhoto _photoRepo;
        private readonly IMailService _mailService;
        private readonly ILogger<PhotoController> _logger;
        private readonly IMapper _mapper;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly ICustomerOrder _orderContext;
        private readonly IServiceType _serviceType;

        public PhotoController(IPhoto photoRepo, IMailService mailService, ILogger<PhotoController> logger, IMapper mapper, IPhotoAccessor photoAccessor, ICustomerOrder orderContext, IServiceType serviceType)
        {
            _photoRepo = photoRepo;
            _mailService = mailService;
            _logger = logger;
            _mapper = mapper;
            _photoAccessor = photoAccessor;
            _orderContext = orderContext;
            _serviceType = serviceType;
        }
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm]PhotoUploadModel photo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ValidationState);
                }
                var result = await photo.Upload(_photoRepo, _photoAccessor, await _serviceType.GetServiceId(photo.Category), true);
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Created(HttpContext.Request.Host.Value, new { Success = true });
        }
        [HttpGet]
        public async Task<ActionResult> GetPhoto(PhotoIdModel Id)
        {
            Photo photo;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }
            try
            {
                photo = await _photoRepo.Get(Id.Id);
                if (photo == null)
                {
                    return NotFound(new { Error = "photo not found" });
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(photo);
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(PhotoIdModel id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);
            }
            try
            {
                var photo = await _photoRepo.Get(id.Id);
                var result = await photo.Delete(_photoRepo, _photoAccessor);
                if (!result.Succeeded)
                    return StatusCode(statusCode: StatusCodes.Status500InternalServerError, result.Error);

            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(new { Success = true });
        }

    }
}