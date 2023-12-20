using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Talabat.Core.Entities.Order_Aggregate;


namespace Talabat.Apis.DTOs
{
    public class OrderDto
    {
       
        [Required]
        public string BasketId { get; set; }
        [Required]
        public int DeliveryMethodId { get; set; }
        public AddressDto shipToAddress { get; set; }
    }
}
