using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class ClientDto:BaseDto<ClientDto,Client>
    {
        public int User_ID { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public string CodeMeli { get; set; }
        public string CodeEgtesadi { get; set; }
        public decimal DiscountPercent { get; set; }
       
        public double MaxCreditValue { get; set; }
        public string ClientPartnerName { get; set; }
    }
}
