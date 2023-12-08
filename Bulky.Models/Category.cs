using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Bulky.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(15)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [Range(1, 100, ErrorMessage = $"Range 1-100")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
        public List<Product>? Products { get; set; }
    }
}
