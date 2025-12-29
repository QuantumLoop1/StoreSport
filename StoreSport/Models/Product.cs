using System.ComponentModel.DataAnnotations;

namespace StoreSport.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название товара")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пожалуйста, введите описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Пожалуйста, введите положительную цену")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите категорию")]
        public string Category { get; set; } = string.Empty;
    }
}
