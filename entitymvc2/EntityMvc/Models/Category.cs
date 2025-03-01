using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EntityMvc.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
} 