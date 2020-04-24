using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class Product_FactorDto : BaseDto<Product_FactorDto, Product_Factor>
    {
        public int id { get; set; }
        public string ProductAndService { get; set; }
        public int ProductAndService_ID { get; set; }
        public int Factor_ID { get; set; }
        public decimal Width { get; set; }
        public decimal length { get; set; }
        public int Unit { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public string Metraj { get; set; }
        public string RowDiscription { get; set; }


        public override void CustomMappings(IMappingExpression<Product_Factor, Product_FactorDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.ProductAndService,
                    config => config.MapFrom(src => $"{src.ProductAndService.ProductName}"));
            
            
            mappingExpression.ForMember(
                    dest => dest.Metraj,
                    config => config.MapFrom(p => p.ProductAndService.UnitType.Equals(UnitType.Metr) ? $"{Math.Round(Convert.ToDecimal(p.Unit * p.length), 3)}" :
                    p.ProductAndService.UnitType.Equals(UnitType.SquareMeters) ? $"{Math.Round(Convert.ToDecimal(p.Unit * p.Width * p.length), 3)}" :
                    p.ProductAndService.UnitType.Equals(UnitType.Unit) ? $"{p.Unit}" : ""));
        }
    }
}
