using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Models
{
    public class ManufactureRequest
    {
        public bool AllTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
