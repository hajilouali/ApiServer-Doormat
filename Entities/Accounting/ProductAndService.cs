using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
   public class ProductAndService: BaseEntity
    {
        public ProductAndService()
        {
            Random _rdm = new Random();
            UnitType = UnitType.SquareMeters;
            ProductCode = _rdm.Next(1000, 9999).ToString();
            UnitPrice = 0;
            ProductType = ProductType.Product;
        }
        [Display(Name = "نام محصول")]
        public string ProductName { get; set; }
        [Display(Name = "کد محصول")]
        public string ProductCode { get; set; }
        [Display(Name = "واحد")]
        public UnitType UnitType { get; set; }
        [Display(Name = "قیمت واحد")]
        public int UnitPrice { get; set; }
        [Display(Name = "نوع ")]
        public ProductType ProductType { get; set; }


        public ICollection<Product_Factor> Product_Factor { get; set; }

    }
    public enum UnitType
    {
        [Display(Name ="مترمربع")]
        SquareMeters,
        [Display(Name = "متر")]
        Metr,
        [Display(Name = "عدد")]
        Unit,

    }
    public enum ProductType
    {
        [Display(Name = "محصول")]
        Product,
        [Display(Name = "سرویس")]
        Service
    }
}
