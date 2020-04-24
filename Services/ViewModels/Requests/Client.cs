using System;
using System.Collections.Generic;
using System.Text;

namespace Services.ViewModels.Requests
{
    public class ClientViewModel
    {
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public int CodeMeli { get; set; }
        public int CodeEgtesadi { get; set; }
        public decimal DiscountPercent { get; set; }
        public int AccountingHeading_ID { get; set; }
        public int User_ID { get; set; }
        public double MaxCreditValue { get; set; }
        public string ClientPartnerName { get; set; }
    }
}
