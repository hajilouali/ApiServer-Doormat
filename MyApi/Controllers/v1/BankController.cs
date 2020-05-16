using AutoMapper.QueryableExtensions;
using Common;
using Common.Exceptions;
using Data.Repositories;
using Entities;
using Entities.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApi.Models;
using Services.Bessines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Accountants")]
    [ApiVersion("1")]
    public class BankController:BaseController
    {
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Product_Factor> _Product_Factor;
        private readonly IRepository<FactorAttachment> _FactorAttachment;
        private readonly IRepository<Manufacture> _Manufacture;
        private readonly IRepository<ProductAndService> _ProductAndService;
        private readonly IRepository<Client> _Client;
        private readonly IRepository<SanadHeading> _SanadHeading;
        private readonly IRepository<Sanad> _Sanad;
        private readonly IRepository<Bank> _Bank;
        private readonly IRepository<AccountingHeading> _AccountingHeading;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<ManufactureHistory> _ManufactureHistory;
        private readonly IRepository<ExpertHistory> _ExpertHistory;
        private readonly IRepository<SanadAttachment> _SanadAttachment;
        private readonly SiteSettings _siteSetting;

        public BankController(IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, IRepository<ExpertHistory> expertHistory, IRepository<SanadAttachment> sanadAttachment, IConfiguration configuration)
        {
            _Factor = factor;
            _Product_Factor = product_Factor;
            _FactorAttachment = factorAttachment;
            _Manufacture = manufacture;
            _ProductAndService = productAndService;
            _Client = client;
            _SanadHeading = sanadHeading;
            _Sanad = sanad;
            _Bank = bank;
            _AccountingHeading = accountingHeading;
            _hostingEnvironment = hostingEnvironment;
            _ManufactureHistory = manufactureHistory;
            _ExpertHistory = expertHistory;
            _SanadAttachment = sanadAttachment;
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        [HttpGet]
        public async Task<ActionResult<List<BankDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await _Bank.TableNoTracking.ProjectTo<BankDto>()
               .ToListAsync(cancellationToken);

            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ApiResult<BankDto>> Get(int id, CancellationToken cancellationToken)
        {
            var dto = await _Bank.TableNoTracking.ProjectTo<BankDto>()
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }
        [HttpPost]
        public async Task<ApiResult<BankDto>> Create(BankDto dto, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory,_ExpertHistory, _SanadAttachment);
            var respons = await usersProcess.AddToBank(dto.ToEntity());
            if (respons)
                return dto;
            throw new BadRequestException("مشکلی در فرایند ثبت بانک ایجاد شده است");
        }
        [HttpPut("{id:int}")]
        public async Task<ApiResult<BankDto>> Update(int id, BankDto dto, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory,_ExpertHistory, _SanadAttachment);
            var respons = await usersProcess.UpdateBank(id,dto.ToEntity());
            if (respons)
                return Ok();
            throw new BadRequestException("مشکلی در فرایند بروز رسانی بانک ایجاد شده است");
        }

    }
}
