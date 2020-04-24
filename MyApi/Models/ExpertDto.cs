using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class ExpertHistoryDto : BaseDto<ExpertHistoryDto, ExpertHistory>
    {
        public ExpertCordition ExpertCordition { get; set; }
        public string ExpertCorditionTitle { get; set; }
        public string DateTime { get; set; }
        public int User_ID { get; set; }
        public string UserName { get; set; }
        public int Expert_ID { get; set; }
        public int Facor_ID { get; set; }
        public string LastFactorStatus { get; set; }
        public override void CustomMappings(IMappingExpression<ExpertHistory, ExpertHistoryDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.DateTime,
                    config => config.MapFrom(src => $"{src.DateTime.ToPersianDigitalDateTimeString()}"));
            mappingExpression.ForMember(
                    dest => dest.UserName,
                    config => config.MapFrom(src => $"{src.User.UserName}"));
            mappingExpression.ForMember(
                  dest => dest.ExpertCorditionTitle,
                   config => config.MapFrom(src => src.ExpertCordition.Equals(ExpertCordition.PendingForVisit) ? "در انتظار بازدید" :

                  src.ExpertCordition.Equals(ExpertCordition.Issued) ? "بازدید شده" : ""
                  ));
            mappingExpression.ForMember(
                  dest => dest.LastFactorStatus,
                   config => config.MapFrom(src => src.Factor.FactorType.Equals(FactorType.Factor) ? "فاکتور شده" :

                  src.Factor.FactorType.Equals(FactorType.PishFactor) ? "پیش فاکتور شده" : "پیش فاکتور همکار"
                  ));
        }
    }
    public class ExpertDto:BaseDto<ExpertDto,Expert>
    {
        public string DateTime { get; set; }
        public ExpertCordition ExpertCordition { get; set; }
        public string ExpertCorditionTitle { get; set; }
        public int Client_ID { get; set; }
        public string ClientName { get; set; }
        public string Clientaddress { get; set; }
        public string Clientphone { get; set; }
        public List<ExpertHistoryDto> ExpertHistores { get; set; }

        public override void CustomMappings(IMappingExpression<Expert, ExpertDto> mappingExpression)
        {
            mappingExpression.ForMember(
                   dest => dest.ExpertHistores,
                   config => config.MapFrom(src => src.ExpertHistories));
            mappingExpression.ForMember(
                   dest => dest.Clientaddress,
                   config => config.MapFrom(src => src.Client.ClientAddress.ToString()));
            mappingExpression.ForMember(
                   dest => dest.Clientphone,
                   config => config.MapFrom(src => src.Client.ClientPhone.ToString()));
            mappingExpression.ForMember(
                    dest => dest.DateTime,
                    config => config.MapFrom(src => $"{src.DateTime.ToPersianDigitalDateTimeString()}"));
            mappingExpression.ForMember(
                    dest => dest.ClientName,
                    config => config.MapFrom(src => $"{src.Client.ClientName}"));
            mappingExpression.ForMember(
                  dest => dest.ExpertCorditionTitle,
                   config => config.MapFrom(src => src.ExpertCordition.Equals(ExpertCordition.PendingForVisit) ? "در انتظار بازدید" :                  
                  src.ExpertCordition.Equals(ExpertCordition.Issued) ? "بازدید شده" : ""
                  ));
        }
    }
}
