using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Bessines
{
    public class CompanyProgres
    {
        private readonly IRepository<Expert> _Expert;
        private readonly IRepository<ExpertHistory> _ExpertHistory;
        private readonly IRepository<Manufacture> _Manufacture;
        private readonly IRepository<ManufactureHistory> _ManufactureHistory;
        private readonly UserManager<User> _userManager;

        public CompanyProgres(IRepository<Expert> expert, IRepository<ExpertHistory> expertHistory, IRepository<Manufacture> manufacture, IRepository<ManufactureHistory> manufactureHistory, UserManager<User> userManager)
        {
            _Expert = expert;
            _ExpertHistory = expertHistory;
            _Manufacture = manufacture;
            _ManufactureHistory = manufactureHistory;
            _userManager = userManager;
        }

        
    }
}
