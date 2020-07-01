using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WonderService.Data.Security;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.Controllers
{

    [Route("api/category")]
    [ApiController]
    [Authorize(Roles = Role.Admin, AuthenticationSchemes = "Bearer")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly ILogger<ServiceTypeController> _logger;
        private readonly IServiceType _serviceType;
        private readonly IMapper _mapper;

        public ServiceTypeController(IMailService mailService, ILogger<ServiceTypeController> logger, IServiceType serviceType, IMapper mapper)
        {
            _mailService = mailService;
            _logger = logger;
            _serviceType = serviceType;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult> Post(ServiceTypeVm serviceType)
        {
            var service = await _serviceType.GetServiceId(serviceType.ServiceType);
            if (!service.Equals(default)) return BadRequest(new { Error = "Service Type Already exist" });
            ReturnResult result;
            if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
            try
            {
                var model = _mapper.Map<ServiceTypeVm, ServicesTypes>(serviceType);
                await _serviceType.Post(model);
                result = await _serviceType.SaveChanges();
                if (result.Error != null) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(result.Succeeded);
        }

        [HttpGet]
        public ActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Error = "Id is required" });
            ServicesTypes model;
            try
            {
                model = _serviceType.Get(id);
                if (model == null) return NotFound(new { Error = "service type not found" });
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(model);
        }
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult> Get()
        {


            List<StateLocalGovernmentVm> mapModel;
            try
            {
                var model = await _serviceType.GetAll();
                if (model.Count <= 0) return NoContent();
                mapModel = _mapper.Map<List<ServicesTypes>, List<StateLocalGovernmentVm>>(model);
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(mapModel);
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
        {


            if (string.IsNullOrEmpty(id)) return BadRequest(new { Error = "Id is required" });
            ServicesTypes model;
            try
            {
                model = _serviceType.Get(id);
                if (model == null) return BadRequest(new { Error = "service type not found" });
                await _serviceType.Delete(model);
                var result = await _serviceType.SaveChanges();
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);

            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(model);
        }
        [HttpPut]
        public async Task<ActionResult> Update(ServiceTypeVm service)
        {


            if (!ModelState.IsValid && string.IsNullOrEmpty(service.Id)) return BadRequest(new { Error = "Id is required", body = ModelState.ValidationState });
            ServicesTypes model;
            try
            {
                model = _serviceType.Get(service.Id);
                if (model == null) return BadRequest(new { Error = "service type not found" });
                model.ServiceType = service.ServiceType ?? model.ServiceType;
                await _serviceType.Update(model);
                var result = await _serviceType.SaveChanges();
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);

            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(model);
        }
    }

}