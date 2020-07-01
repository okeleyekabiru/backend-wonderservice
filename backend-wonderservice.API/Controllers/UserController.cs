using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Extensions;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plugins.JwtHandler;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;
        private readonly IMailService _mailService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IJwtSecurity _jwtSecurity;

        public UserController(IUser userRepo, IMailService mailService, ILogger<UserController> logger, IMapper mapper, IJwtSecurity jwtSecurity)
        {
            _userRepo = userRepo;
            _mailService = mailService;
            _logger = logger;
            _mapper = mapper;
            _jwtSecurity = jwtSecurity;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserModel user)
        {
            ReturnResult result;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ValidationState);
                }
                result = await _userRepo.Register(user);
                if (!result.Succeeded) return StatusCode(statusCode: StatusCodes.Status500InternalServerError, result.Error);

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(new { Success = true });
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel user)
        {
            JwtModel result;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.ValidationState);
                }

                result = await _userRepo.Login(user);
                if (result.Error != null)
                {
                    return BadRequest(new { error = result.Error });
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return Ok(new
            {
                status = StatusCodes.Status200OK,
                message = "user successfully login ",
                token = result.Token,
                body = new { user.Email }

            });
        }
        [AllowAnonymous]
        [HttpPost("confirmation")]
        public async Task<ActionResult> Confirmation(string token)
        {
            ReturnResult model;

            try
            {

                model = await _userRepo.ConfirmEmail(token);
                if (!model.Succeeded) return BadRequest(new { Error = model.Error });
            }
            catch (Exception e)
            {

                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(String.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(500, "Internal Server");
            }

            return Ok(new { success = "Email Successful Verified" });
        }
        [HttpGet("id")]
        public async Task<ActionResult> GetUser(UserIdModel id)
        {
            UserViewModel newModel;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ValidationState);

            }
            try
            {
                var model = await _userRepo.Get(id.Id);
                newModel = _mapper.Map<User, UserViewModel>(model);
                if (model == null) return NotFound(new { Error = "User cannot be found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(String.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(500, "Internal Server");
            }

            return Ok(newModel);

        }
        [HttpGet("users")]
        public ActionResult GetAllUser(int pageNumber, int pageSize)
        {
            IEnumerable<UserViewModel> model;
            try
            {
                var users = _userRepo.GetAll(pageNumber, pageSize);
                if (users == null)
                    return NoContent();
                var metadata = new
                {
                    users.TotalCount,
                    users.PageSize,
                    users.CurrentPage,
                    users.TotalPages,
                    users.HasNext,
                    users.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                model = _mapper.Map<List<User>, List<UserViewModel>>(users);

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(String.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(500, "Internal Server");
            }

            return Ok(model);
        }
        [HttpDelete("id")]
        public async Task<ActionResult> Delete(UserIdModel id)
        {
            ReturnResult result;
            try
            {
                var user = await _userRepo.Get(id.Id);
                result = await _userRepo.Delete(user);
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);


            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(String.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(500, "Internal Server");
            }

            return Ok(new { result.Succeeded });
        }
        [HttpPut("update")]
        public async Task<ActionResult> Update(UserViewModel userModel)
        {
            ReturnResult result;
            try
            {
                result = await userModel.Update(_userRepo, _mailService, _jwtSecurity);
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(String.Empty, e.InnerException?.ToString() ?? e.Message, "error");
                return StatusCode(500, "Internal Server");
            }

            return Ok(new { result.Succeeded });
        }
    }

}