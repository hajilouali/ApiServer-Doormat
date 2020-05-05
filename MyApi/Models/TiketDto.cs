using AutoMapper;
using Entities.Tikets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class TiketDto:BaseDto<TiketDto,Tiket>
    {
        public int id { get; set; }
        public string Title { get; set; }
        public bool Closed { get; set; }
        public short Level { get; set; }
        public string LevelString { get; set; }
        public short Department { get; set; }
        public string DepartmentString { get; set; }
        public bool IsAdminSide { get; set; }
        public int UserID { get; set; }
        public string User { get; set; }
        public DateTime DataCreate { get; set; }
        public string DataCreateString { get; set; }
        public DateTime DataModified { get; set; }
        public string DataModifiedstring { get; set; }
        public List<TiketContentDto> tiketContents { get; set; }
        public override void CustomMappings(IMappingExpression<Tiket, TiketDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.DataCreateString,
                    config => config.MapFrom(src => $"{src.DataCreate.ToPersianDateTime()}"));
            mappingExpression.ForMember(
                    dest => dest.DataModifiedstring,
                    config => config.MapFrom(src => $"{src.DataModified.ToPersianDateTime()}"));
            mappingExpression.ForMember(
                    dest => dest.User,
                    config => config.MapFrom(src => $"{src.User.FullName}"));
            mappingExpression.ForMember(
                    dest => dest.LevelString,
                    config => config.MapFrom(src => src.Level==1? "عادی" :
                    src.Level==2? "مهم" :
                    src.Level==3? "خیلی مهم" : ""));
            mappingExpression.ForMember(
                    dest => dest.DepartmentString,
                    config => config.MapFrom(src => src.Department==1? "واحد تولید" :
                    src.Department == 2 ? "واحد مالی و حسابداری" :
                    src.Department == 3 ? "سوالات عمومی" :
                    src.Department == 4 ? "انتقادات و پیشنهادات" :
                    src.Department == 5 ? "ارسال به مدیر" : ""));

        }
    }
    public class TiketContentDto : BaseDto<TiketContentDto, TiketContent>
    {
        public int id { get; set; }
        public int TiketId { get; set; }
        public bool IsAdminSide { get; set; }
        public string Text { get; set; }
        public string FileURL { get; set; }
        public int UserID { get; set; }
        public string User { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DataModified { get; set; }
        public override void CustomMappings(IMappingExpression<TiketContent, TiketContentDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.User,
                    config => config.MapFrom(src => $"{src.User.FullName}"));
        }
    }
}
