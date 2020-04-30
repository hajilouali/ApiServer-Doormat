using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebFramework.Api;

namespace MyApi.Models
{
    public class ProductAndServiceDto:BaseDto<ProductAndServiceDto,ProductAndService>
    {
        public int id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public UnitType UnitType { get; set; }
        public string UnitTypestring { get; set; }

        public int UnitPrice { get; set; }
        public string UnitPricestring { get; set; }
        public ProductType ProductType { get; set; }


        public override void CustomMappings(IMappingExpression<ProductAndService, ProductAndServiceDto> mappingExpression)
        {
            mappingExpression.ForMember(
                dest => dest.UnitTypestring,
                config => config.MapFrom(src => src.UnitType==UnitType.SquareMeters?"مترمربع":
                    src.UnitType == UnitType.Metr ? "متر" :
                    src.UnitType == UnitType.Unit ? "عدد" : ""));
            mappingExpression.ForMember(
                dest => dest.UnitPricestring,
                config => config.MapFrom(src => $"{src.UnitPrice.ToString("N0")} ریال"));

        }
    }
    
}
