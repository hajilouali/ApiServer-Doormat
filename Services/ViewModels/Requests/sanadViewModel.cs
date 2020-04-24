using System;
using System.Collections.Generic;
using System.Text;

namespace Services.ViewModels.Requests
{
    public class sanadViewModel
    {
        public string ShamsiDate { get; set; }
        public string Discription { get; set; }
        public List<SanadViewModel> Sanads { get; set; }
    }
    public class SanadViewModel
    {
        public int AccountingHeading_ID { get; set; }
        public int Bedehkari { get; set; }
        public int Bestankari { get; set; }
        public string Comment { get; set; }
    }
}
