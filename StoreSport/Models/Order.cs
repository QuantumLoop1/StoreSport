using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StoreSport.Models
{
    public class Order
    {
        [BindNever]
        public int OrderId { get; set; }

        [BindNever]
        public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();

        [Required(ErrorMessage = "Пожалуйста, введите имя")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пожалуйста, введите адрес")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пожалуйста, введите город")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пожалуйста, введите регион")]
        public string? State { get; set; }

        public string? Zip { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите страну")]
        public string Country { get; set; } = string.Empty;

        public bool GiftWrap { get; set; }
    }
}
