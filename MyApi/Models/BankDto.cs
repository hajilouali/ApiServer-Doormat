using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class BankDto:BaseDto<BankDto,Bank>
    {
        public string BankTitle { get; set; }
        public int AccountingHeading_ID { get; set; }
    }
}
