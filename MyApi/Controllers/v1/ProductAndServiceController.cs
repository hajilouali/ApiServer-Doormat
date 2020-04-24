using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    public class ProductAndServiceController : CrudController<ProductAndServiceDto, ProductAndService>
    {
        private readonly IRepository<Client> _clientrepository;
        private readonly IRepository<AccountingHeading> _accountingHeadingrepository;
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Expert> _Expert;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public ProductAndServiceController(IRepository<ProductAndService> repository, IRepository<Client> clientrepository, IRepository<AccountingHeading> accountingHeadingrepository, IRepository<Factor> factor, IRepository<Expert> expert, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager) : base(repository)
        {
            _clientrepository = clientrepository;
            _accountingHeadingrepository = accountingHeadingrepository;
            _Factor = factor;
            _Expert = expert;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
    }
}
