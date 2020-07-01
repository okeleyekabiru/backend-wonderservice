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
    [Route("api/service")]
    [ApiController]
    [Authorize(Roles = Role.Admin, AuthenticationSchemes = "Bearer")]

    public class RenderedServiceController : ControllerBase
    {
        private readonly IServiceUpload _serviceContext;
        private readonly IMailService _mailService;
        private readonly ILogger<RenderedServiceController> _logger;
        private readonly ICustomerOrder _orderContext;
        private readonly IPhoto _photoRepo;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IServiceType _serviceType;
        private readonly IMapper _mapper;

        public RenderedServiceController(IServiceUpload serviceContext, IMailService mailService, ILogger<RenderedServiceController> logger, ICustomerOrder orderContext, IPhoto photoRepo, IPhotoAccessor photoAccessor, IServiceType serviceType, IMapper mapper)
        {
            _serviceContext = serviceContext;
            _mailService = mailService;
            _logger = logger;
            _orderContext = orderContext;
            _photoRepo = photoRepo;
            _photoAccessor = photoAccessor;
            _serviceType = serviceType;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm]ServicesVm services)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
            ReturnResult result;
            try
            {
                var serviceId = await _serviceType.GetServiceId(services.ServiceType);
                if (serviceId.Equals(default)) return NotFound(new { Error = " Service Type Not found" });

                var service = new Services
                {
                    Body = services.Body,
                    ServiceTypeId = serviceId
                };
                await _serviceContext.Post(service);
                if (services.Photo != null)
                {
                    result = await services.Photo.Upload(_photoRepo, _photoAccessor, service.Id, true);
                    if (result.Error != null)
                        return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");

                }

                if (!await _serviceContext.SaveChanges()) return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");

            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(new { Succeded = true });
        }
        [HttpGet]
        public async Task<ActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Error = "Id must be provided" });
            ServiceReturnVm newModel;
            try
            {
                var model = await _serviceContext.Get(id);
                if (model == null) return NotFound();

                newModel = _mapper.Map<Services, ServiceReturnVm>(model);
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(newModel);
        }
        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult> Get()
        {

            List<ServiceReturnVm> newModel;
            try
            {
                var model = await _serviceContext.GetAll();
                if (model.Count <= 0) return NoContent();

                newModel = _mapper.Map<List<Services>, List<ServiceReturnVm>>(model);
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(newModel);
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
        {

            if (string.IsNullOrEmpty(id)) return BadRequest(new { Error = "Id must be provided" });
            try
            {
                var model = await _serviceContext.Get(id);
                if (model == null) return NotFound();

                await _serviceContext.Delete(model);
                if (!await _serviceContext.SaveChanges()) return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                model.Photo.Delete(_photoAccessor);
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(new { Succeded = true });
        }

        [HttpPut]
        public async Task<ActionResult> Update(ServicesUpdateVm services)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
            try
            {
                var serviceType = await _serviceType.GetServiceId(services.ServiceType);
                if (serviceType.Equals(default)) return NotFound(new { Error = "service type not found" });



                var service = await _serviceContext.Get(services.Id);
                service.Body = services.Body ?? service.Body;
                service.ServiceTypeId = serviceType;
                await _serviceContext.Update(service);
                if (!await _serviceContext.SaveChanges()) return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(new { Succeded = true });
        }
    }

}