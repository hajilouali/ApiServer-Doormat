using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Models
{
    public class ManufactureHistoryAdd
    {
        public int Manufactury_ID { get; set; }
        public string Discription { get; set; }
        public ConditionManufacture ConditionManufacture { get; set; }
    }
}
