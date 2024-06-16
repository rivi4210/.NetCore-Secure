using AutoMapper;
using DTO;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<string> GetAllOrders()
        {
            return null;
            // Uncomment and implement actual logic if needed
            // return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<ActionResult<OrderReturnDTO>> CreateOrder([FromBody] OrderDTO orderDto)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDto);
            var createdOrder = await _orderService.Post(order);

            if (createdOrder != null)
            {
                var orderToReturn = _mapper.Map<Order, OrderReturnDTO>(createdOrder);
                return CreatedAtAction(nameof(GetAllOrders), new { id = orderToReturn.OrderId });
            }

            return NoContent();
        }
    }
}