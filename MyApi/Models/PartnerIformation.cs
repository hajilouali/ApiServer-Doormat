using Services.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Models
{
    public class PartnerFactorCunt 
    {
        public int Inyear { get; set; }
        public int InMonth { get; set; }
        public int All { get; set; }
    }
    
    public class PartnerIformation
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public ClientAccountingStatus ClientAccountingStatus { get; set; }
        public PartnerFactorCunt PartnerFactorCunt { get; set; }
        public List<ManufactureDto> ManufactureDto { get; set; }
        public List<FactorDto> FactorPernding { get; set; }

    }
}
