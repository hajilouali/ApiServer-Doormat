using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class ManufactureHistoryDto:BaseDto<ManufactureHistoryDto, ManufactureHistory>
    {
        public int User_ID { get; set; }
        public string UserFullname { get; set; }
        public int Manufacture_ID { get; set; }
        public string DateTime { get; set; }
        public string ConditionManufactureTitle { get; set; }
        public ConditionManufacture ConditionManufacture { get; set; }
        public string Discription { get; set; }

        public override void CustomMappings(IMappingExpression<ManufactureHistory, ManufactureHistoryDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.DateTime,
                    config => config.MapFrom(src => $"{src.DateTime.ToPersianDateTime()}"));
            mappingExpression.ForMember(
                    dest => dest.UserFullname,
                    config => config.MapFrom(src => $"{src.User.FullName}"));
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
