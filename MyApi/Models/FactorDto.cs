using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class FactorDto:BaseDto<FactorDto,Factor>
    {
        public int id { get; set; }
        public string DateTime { get; set; }
        public DateTime DateTimevalu { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Taxes { get; set; }
        public decimal Discount { get; set; }
        public decimal FactorPrice { get; set; }
        public int User_ID { get; set; }
        public string User_Name { get; set; }
        public string UserName { get; set; }
        public int Client_ID { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public string ClientMeli { get; set; }
        public string ClientEgtesadi { get; set; }

        public string FactorCodeView { get; set; }
        public string Discription { get; set; }
        public FactorType FactorType { get; set; }
        public List<Product_FactorDto> Product_Factor { get; set; }
        public List<ManufactureDto> Manufacture { get; set; }

        public override void CustomMappings(IMappingExpression<Factor, FactorDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.DateTime,
                    config => config.MapFrom(src => $"{src.DateTime.ToPersianDateTime().ToPersianDigitalDateString()}"));
            mappingExpression.ForMember(
                    dest => dest.User_Name,
                    config => config.MapFrom(src => $"{src.Client.ClientName}"));
            mappingExpression.ForMember(
                    dest => dest.UserName,
                    config => config.MapFrom(src => $"{src.User.FullName}"));
            mappingExpression.ForMember(
                    dest => dest.DateTimevalu,
                    config => config.MapFrom(src => src.DateTime));




            mappingExpression.ForMember(
                dest => dest.ClientAddress,
                config => config.MapFrom(src => $"{src.Client.ClientAddress}"));
            mappingExpression.ForMember(
                dest => dest.ClientPhone,
                config => config.MapFrom(src => $"{src.Client.ClientPhone}"));
            mappingExpression.ForMember(
                dest => dest.ClientMeli,
                config => config.MapFrom(src => $"{src.Client.CodeMeli}"));
            mappingExpression.ForMember(
                dest => dest.ClientEgtesadi,
                config => config.MapFrom(src => $"{src.Client.CodeEgtesadi}"));
        }
    }
}
