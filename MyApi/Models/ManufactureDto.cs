using AutoMapper;
using Entities;
using Services.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class ManufactureDto:BaseDto<ManufactureDto, Manufacture>
    {
        public int Factor_ID { get; set; }
        public int FactorClientID { get; set; }
        public string FactorClientName { get; set; }
        public string FactorClientaddress { get; set; }
        public string FactorClientPhone { get; set; }
        public ClientAccountingStatus ClientAccountingStatus { get; set; }
        public string InDateTime { get; set; }
        public ConditionManufacture ConditionManufacture { get; set; }
        public string ConditionManufactureTitle { get; set; }
        public List<ManufactureHistoryDto> ManufactureHistories { get; set; }
        public override void CustomMappings(IMappingExpression<Manufacture, ManufactureDto> mappingExpression)
        {
            mappingExpression.ForMember(
                   dest => dest.InDateTime,
                   config => config.MapFrom(src => $"{src.InDateTime.ToPersianDateTime()}"));
            mappingExpression.ForMember(
                   dest => dest.FactorClientName,
                   config => config.MapFrom(src => $"{src.Factor.Client.ClientName}"));
            mappingExpression.ForMember(
                   dest => dest.FactorClientaddress,
                   config => config.MapFrom(src => $"{src.Factor.Client.ClientAddress}"));
            mappingExpression.ForMember(
                   dest => dest.FactorClientPhone,
                   config => config.MapFrom(src => $"{src.Factor.Client.ClientPhone}"));
            mappingExpression.ForMember(
                   dest => dest.FactorClientID,
                   config => config.MapFrom(src => src.Factor.Client_ID));
            mappingExpression.ForMember(
                   dest => dest.ConditionManufactureTitle,
                   config => config.MapFrom(src => src.ConditionManufacture.Equals(ConditionManufacture.PendingForConstruction) ? "در انتظار ساخت" :
                   src.ConditionManufacture.Equals(ConditionManufacture.Cut) ? "برش خورده" :
                   src.ConditionManufacture.Equals(ConditionManufacture.Built) ? "ساخته شده" :
                   src.ConditionManufacture.Equals(ConditionManufacture.DeliverToClient) ? "تحویل به مشتری" :
                   src.ConditionManufacture.Equals(ConditionManufacture.DeliverToPartner) ? "تحویل به همکار" :
                   src.ConditionManufacture.Equals(ConditionManufacture.Install) ? "نصب شده" : ""
                   ));
        }
    }
}
