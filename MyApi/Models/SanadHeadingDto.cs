using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class SanadHeadingDto:BaseDto<SanadHeadingDto,SanadHeading>
    {
        public int id { get; set; }
        public int FactorID { get; set; }
        public string StringDateTime { get; set; }
        public string Discription { get; set; }
        public List<SanadDto> Sanads { get; set; }
        public override void CustomMappings(IMappingExpression<SanadHeading, SanadHeadingDto> mappingExpression)
        {
            //mappingExpression.ForMember(
            //        dest => dest.Sanads,
            //        config => config.MapFrom(src =>  src.Sanads.ToList() ));
            mappingExpression.ForMember(
                   dest => dest.StringDateTime,
                   config => config.MapFrom(src => $"{src.DateTime.ToPersianDateTime().ToPersianDigitalDateString()}"));
        }
    }
    public class SanadDto : BaseDto<SanadDto, Sanad>
    {
        public string stringDatatime { get; set; }
        public string AccountingHeading { get; set; }
        public int AccountingHeading_ID { get; set; }
        public int SanadHeading_ID { get; set; }
        public int Bedehkari { get; set; }
        public int Bestankari { get; set; }
        public string Comment { get; set; }
        public override void CustomMappings(IMappingExpression<Sanad, SanadDto> mappingExpression)
        {
            
            mappingExpression.ForMember(
                   dest => dest.AccountingHeading,
                   config => config.MapFrom(src => src.AccountingHeading.Title));
            
            mappingExpression.ForMember(
                   dest => dest.stringDatatime,
                   config => config.MapFrom(src => src.SanadHeading.DateTime.ToPersianDigitalDateTimeString()));
        }
    }
    public class SanadAccountingDto : BaseDto<SanadAccountingDto, Sanad>
    {
        public SanadAccountingDto()
        {
            Mandeh = 0;
            SanadAccountingDtoS = new List<SanadAccountingDto>();
            AccountingHeading_ID = 0;
            SanadHeading_ID = 0;
            Bedehkari = 0;
            Bestankari = 0;
            Mandeh = 0;

        }
        public DateTime Datatime { get; set; }
        public string stringDatatime { get; set; }
        public string AccountingHeading { get; set; }
        public int AccountingHeading_ID { get; set; }
        public int SanadHeading_ID { get; set; }
        public int Bedehkari { get; set; }
        public int Bestankari { get; set; }
        public int Mandeh { get; set; }
        public string TashKhis { get; set; }
        public string Comment { get; set; }
        public List<SanadAccountingDto> SanadAccountingDtoS { get; set; }

        public override void CustomMappings(IMappingExpression<Sanad, SanadAccountingDto> mappingExpression)
        {
            mappingExpression.ForMember(
                   dest => dest.Mandeh,
                   config => config.MapFrom(src =>src.Bestankari- src.Bedehkari));

            mappingExpression.ForMember(
                   dest => dest.Datatime,
                   config => config.MapFrom(src => src.SanadHeading.DateTime));

            mappingExpression.ForMember(
                   dest => dest.TashKhis,
                   config => config.MapFrom(src => src.Bestankari - src.Bedehkari==0?"--": src.Bestankari - src.Bedehkari>0?"بست":"بده"));

            mappingExpression.ForMember(
                   dest => dest.AccountingHeading,
                   config => config.MapFrom(src => src.AccountingHeading.Title));

            mappingExpression.ForMember(
                   dest => dest.stringDatatime,
                   config => config.MapFrom(src => src.SanadHeading.DateTime.ToPersianDigitalDateTimeString()));
        }
    }
    public class ClientStatusDto 
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public double Mandeh { get; set; }
        
        public string Vaziat { get; set; }
    }
}
