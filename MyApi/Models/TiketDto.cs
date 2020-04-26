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
        public string Title { get; set; }
        public bool Closed { get; set; }
        public short Level { get; set; }
        public short Department { get; set; }
        public bool IsAdminSide { get; set; }
        public int UserID { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DataModified { get; set; }
        public List<TiketContentDto> tiketContents { get; set; }

    }
    public class TiketContentDto : BaseDto<TiketContentDto, TiketContent>
    {
        public int TiketId { get; set; }
        public bool IsAdminSide { get; set; }
        public string Text { get; set; }
        public string FileURL { get; set; }
        public DateTime DataCreate { get; set; }
        public DateTime DataModified { get; set; }
    }
}
