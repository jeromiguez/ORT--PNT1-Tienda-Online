﻿using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CompanyManager.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(30, ErrorMessage = ErrorViewModel.MaxCharacters)]
        [MinLength(3, ErrorMessage = ErrorViewModel.MinCharacters)]
        [Required(ErrorMessage = ErrorViewModel.RequiredField)]
        public string Name { get; set; }
        
        public DateTime? DeletedAt { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = ErrorViewModel.MaxCharacters)]
        public string? Description { get; set; }

        [Display(Name = "Imagen")]
        public string? Image { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = ErrorViewModel.RequiredField)]
        public float Price { get; set; }
        
        [Display(Name = "Descuento")]
        [Range(0, 99, ErrorMessage = ErrorViewModel.PorcentRange)]
        public int Discount { get; set; }

        [Display(Name = "Stock")]
        [Required(ErrorMessage = ErrorViewModel.RequiredField)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorViewModel.StockErrorRange)]
        public int Stock { get; set; }

        [Display(Name = "Unidades Vendidas")]
        public int SoldItems { get; set; } = 0;

        public float CalculateDiscount() {
            return Price - (Price * Discount / 100);
        }
    }

}
