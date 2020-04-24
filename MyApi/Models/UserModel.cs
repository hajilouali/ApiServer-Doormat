using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public IList<string> RollName { get; set; }
        public decimal Discount { get; set; }
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }

    }
}
