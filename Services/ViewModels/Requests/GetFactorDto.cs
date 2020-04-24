using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.ViewModels.Requests
{
   public class GetFactorDto
    {
        public bool ISAllType { get; set; }
        public FactorType FactorType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ClientID { get; set; }
    }
}
