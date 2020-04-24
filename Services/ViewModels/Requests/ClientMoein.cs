using System;
using System.Collections.Generic;
using System.Text;

namespace Services.ViewModels.Requests
{
    public class ClientMoein
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ClientID { get; set; }
    }
    public class AcountingMoein
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ClientID { get; set; }
    }
    public class AcountingReviw
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        
    }
    public class ClientsAccpuntingReports
    {
        public int ClientID { get; set; }
        public bool Bed { get; set; }
        public bool Bes { get; set; }
        public double bedIN { get; set; }
        public double bedOut { get; set; }
        public double besIN { get; set; }
        public double besOut { get; set; }
    }
}
