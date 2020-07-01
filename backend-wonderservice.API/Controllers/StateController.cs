
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.Controllers
{
    [Route("api/states")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> _logger;
        private readonly IMailService _mailService;
        private readonly IStateRepo _stateRepo;
        private readonly IMapper _mapper;

        public StateController(ILogger<StateController> logger, IMailService mailService, IStateRepo stateRepo,
            IMapper mapper)
        {
            _logger = logger;
            _mailService = mailService;
            _stateRepo = stateRepo;
            _mapper = mapper;
        }

        #region state

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            IEnumerable<StateLocalGovernmentVm> states = null;
            try
            {
                var state = await _stateRepo.GetState();
                states = _mapper.Map<IEnumerable<States>, IEnumerable<StateLocalGovernmentVm>>(state);
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(
                new
                {
                    body = states,
                    message = "get all state",
                    status = 200,
                    success = true
                });
        }

        #endregion

        #region Local Government
        [HttpGet("sub")]
        public async Task<ActionResult> Get(string localGovernment)
        {
            IEnumerable<StateLocalGovernmentVm> states = null;
            try
            {
                var state = await _stateRepo.GetId(localGovernment);
                if (state == default) return NotFound(new
                {
                    status = 404,
                    body = "",
                    message = "state not found",
                    success = false
                });
                var localGovt = await _stateRepo.GetLocalGovernment(state);
                states = _mapper.Map<IEnumerable<LocalGovernment>, IEnumerable<StateLocalGovernmentVm>>(localGovt);

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(
                new
                {
                    body = states,
                    message = "get all state",
                    status = 200,
                    success = true

                });
        }
        #endregion
    }
}