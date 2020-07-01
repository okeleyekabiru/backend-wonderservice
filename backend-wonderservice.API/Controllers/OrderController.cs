using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.API.SignalR;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WonderService.Data.Security;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.API.Controllers
{

    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICustomerOrder _customerOrder;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IServiceType _serviceType;
        private readonly IStateRepo _stateRepo;
        private readonly IHubContext<NotificationHUb> _hubContext;

        public OrderController(ICustomerOrder customerOrder, ILogger<OrderController> logger, IMapper mapper,
            IMailService mailService, IServiceType serviceType, IStateRepo stateRepo,
            IHubContext<NotificationHUb> hubContext)
        {
            _customerOrder = customerOrder;
            _logger = logger;
            _mapper = mapper;
            _mailService = mailService;
            _serviceType = serviceType;
            _stateRepo = stateRepo;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult> Post(OrderModel order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.ValidationState);
            try
            {
                order.AppointmentDateEnd = order.AppointmentDate.AddHours(3);
                var serviceId = await _serviceType.GetServiceId(order.ServiceType);
                var localGovtId = await _stateRepo.GetLocalGovernmentId(order.LocalGovernment);
                var stateId = await _stateRepo.GetId(order.States);
                if (stateId.Equals(default)) return NotFound();
                if (localGovtId.Equals(default)) return NotFound();
                if (serviceId.Equals(default)) return NotFound(new {Error = "Service type cannot be found"});
                var mapOrderToCustomerOrder = _mapper.Map<OrderModel, Customer>(order);
                mapOrderToCustomerOrder.ServicesTypeId = serviceId;
                mapOrderToCustomerOrder.StateId = stateId;
                mapOrderToCustomerOrder.LocalGovernmentId = localGovtId;
                var result = await _customerOrder.Post(mapOrderToCustomerOrder);
                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            var orderList = new List<OrderModel> {order};
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", orderList, "booking");
            return CreatedAtAction("Post", new {Success = true});
        }

        [Authorize(Roles = Role.Admin, AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            List<OrderModel> orderModel;
            try
            {
                var orders = await _customerOrder.GetAllOrder();
                if (orders.Count < 1) return NoContent();

                orderModel = _mapper.Map<List<Customer>, List<OrderModel>>(orders);
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(orderModel);
        }

        [HttpGet("id")]
        public async Task<ActionResult> Get(string id)
        {
            Customer orderModel;
            OrderModel newModel;
            try
            {
                orderModel = await _customerOrder.GetOrder(id);
                if (orderModel == null) return NoContent();
                newModel = _mapper.Map<Customer, OrderModel>(orderModel);
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
            Customer orderModel;
            try
            {
                orderModel = await _customerOrder.GetOrder(id);
                if (orderModel == null) return NoContent();

                await _customerOrder.Delete(orderModel);
                if (!await _customerOrder.SaveChanges())
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(orderModel);
        }

        [HttpPut]
        public async Task<ActionResult> Update(OrderModelUpdate order)
        {
            Customer orderModel;
            try
            {
                orderModel = await _customerOrder.GetOrder(order.Id);
                if (orderModel == null) return NoContent();
                var serviceId = _serviceType.GetServicesType(order.ServiceType);
                if (serviceId.Equals(default)) return BadRequest(new {Error = "Service type cannot be found"});
                orderModel.Email = order.Email ?? orderModel.Email;
                orderModel.FirstName = order.FirstName ?? orderModel.FirstName;
                orderModel.LastName = order.LastName ?? orderModel.LastName;
                orderModel.Address = order.Address ?? orderModel.Address;
                orderModel.PhoneNumber = order.PhoneNumber ?? orderModel.PhoneNumber;
                orderModel.ServicesType = serviceId;
                await _customerOrder.Update(orderModel);
                if (!await _customerOrder.SaveChanges())
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException?.ToString() ?? e.Message);
                _mailService.SendMail(string.Empty, e.InnerException?.ToString() ?? e.Message, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

            return Ok(orderModel);
        }
    }
}