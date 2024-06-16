using Entities;

namespace DTO
{
    public class OrderDTO
    {
        public virtual ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
}
