using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Repositories;
using Entities;
using Entities.Accounting;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using Services.Bessines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Accountants,Manufacturers")]
    [ApiVersion("1")]
    public class ManufactoreController:BaseController
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
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<ManufactureHistory> _ManufactureHistory;
        private readonly IRepository<ExpertHistory> _ExpertHistory;
        private readonly IRepository<SanadAttachment> _SanadAttachment;
        public ManufactoreController(IRepository<Manufacture> manufacture, IRepository<ManufactureHistory> manufactureHistory, UserManager<User> userManager, IRepository<Factor> factor = null, IRepository<Product_Factor> product_Factor = null, IRepository<FactorAttachment> factorAttachment = null, IRepository<ProductAndService> productAndService = null, IRepository<Client> client = null, IRepository<SanadHeading> sanadHeading = null, IRepository<Sanad> sanad = null, IRepository<Bank> bank = null, IRepository<AccountingHeading> accountingHeading = null, IHostingEnvironment hostingEnvironment = null, IRepository<ExpertHistory> expertHistory = null, IRepository<SanadAttachment> sanadAttachment = null)
        {
            _Manufacture = manufacture;
            _ManufactureHistory = manufactureHistory;
            _userManager = userManager;
            _Factor = factor;
            _Product_Factor = product_Factor;
            _FactorAttachment = factorAttachment;
            _ProductAndService = productAndService;
            _Client = client;
            _SanadHeading = sanadHeading;
            _Sanad = sanad;
            _Bank = bank;
            _AccountingHeading = accountingHeading;
            _hostingEnvironment = hostingEnvironment;
            _ExpertHistory = expertHistory;
            _SanadAttachment = sanadAttachment;
        }
        [HttpGet("[action]")]
        public async Task<ApiResult<List<ManufactureDto>>> GetManufacture()
        {
            var list = _Manufacture.TableNoTracking.Where(p=>p.ConditionManufacture!=ConditionManufacture.Install&& p.ConditionManufacture != ConditionManufacture.DeliverToClient&& p.ConditionManufacture != ConditionManufacture.DeliverToPartner).Include(p=>p.ManufactureHistories).ProjectTo<ManufactureDto>().ToList();
            if (list != null)
            {
                AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);

                foreach (var item in list)
                {
                    var result = await usersProcess.ClientAccountingStatus(item.FactorClientID);
                    item.ClientAccountingStatus = result;
                }
                return Ok(list);
            }
                
            return NotFound();
        }
        [HttpPost("[action]")]
        public async Task<ApiResult<List<ManufactureDto>>> GetManufacturebytime(ManufactureRequest dto)
        {
            if (dto.AllTime)
            {
                var list = _Manufacture.TableNoTracking.Include(p => p.ManufactureHistories).ProjectTo<ManufactureDto>().ToList();
                
                if (list != null)
                {
                    AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);

                    foreach (var item in list)
                    {
                        var result = await usersProcess.ClientAccountingStatus(item.FactorClientID);
                        item.ClientAccountingStatus = result;
                    }
                    return Ok(list);
                }
                    
            }
            else
            {
                var s = PersianDate.ToGeorgianDateTime(dto.StartTime);

                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(dto.EndTime);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);
                var list = _Manufacture.TableNoTracking.Where(p => p.InDateTime>=start&&p.InDateTime<=end).Include(p => p.ManufactureHistories).ProjectTo<ManufactureDto>().ToList();
                if (list != null)
                {
                    AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);

                    foreach (var item in list)
                    {
                        var result = await usersProcess.ClientAccountingStatus(item.FactorClientID);
                        item.ClientAccountingStatus = result;
                    }
                    return Ok(list);
                }
                    
            }
            
            return NotFound();
        }
        [HttpPost("[action]")]
        public async Task<ApiResult> AddHistory(ManufactureHistoryAdd ManufactureHistoryAdd)
        {
            try
            {
                var user =await _userManager.FindByNameAsync(User.Identity.Name);
                ManufactureHistory ss = new ManufactureHistory()
                {
                    Manufacture_ID = ManufactureHistoryAdd.Manufactury_ID,
                    ConditionManufacture = ManufactureHistoryAdd.ConditionManufacture,
                    Discription = ManufactureHistoryAdd.Discription,
                    DateTime = DateTime.Now,
                    User_ID = user.Id
                };
                _ManufactureHistory.Add(ss);
                var m = _Manufacture.GetById(ss.Manufacture_ID);
                m.ConditionManufacture = ss.ConditionManufacture;
                _Manufacture.Update(m);
                return Ok();
            }
            catch 
            {

                throw new BadRequestException("مشکلی در فرایند ثبت رویداد ایجاد شده است");
            }
        }
        [HttpGet("[action]/{id:int}")]
        public async Task<ApiResult<List<ManufactureHistoryDto>>> GetHistory(int id)
        {
            var list = _ManufactureHistory.TableNoTracking.Where(p => p.Manufacture_ID == id).OrderByDescending(p => p.DateTime)
                .ProjectTo<ManufactureHistoryDto>().ToList();
            if (list != null)
                return list;
            return NotFound();
        }
        [HttpPut("[action]/{id:int}")]
        public async Task<ApiResult<ManufactureHistoryDto>> UpdateHistory(int id ,ManufactureHistoryDto ManufactureHistoryDto)
        {
            var manufacture = _ManufactureHistory.GetById(id);
            if (manufacture == null)
                return NotFound();
            
            
            manufacture.User_ID= _userManager.Users.Where(p => p.UserName == User.Identity.Name).SingleOrDefault().Id;
            manufacture.Discription = ManufactureHistoryDto.Discription;
            manufacture.ConditionManufacture = ManufactureHistoryDto.ConditionManufacture;
            _ManufactureHistory.Update(manufacture);
            if (manufacture == _ManufactureHistory.Entities.Where(p=>p.Manufacture_ID== manufacture.Manufacture_ID).OrderBy(p=>p.DateTime).Last())
            {
                var m = _Manufacture.GetById(manufacture.Manufacture_ID);
                m.ConditionManufacture = manufacture.ConditionManufacture;
                _Manufacture.Update(m);
            }
            return Ok(ManufactureHistoryDto);
        }
        [HttpDelete("[action]/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult> DeleteHistory(int id)
        {
            var f = _ManufactureHistory.GetById(id);
            if (f == null)
                return NotFound();
            _ManufactureHistory.Delete(f);
            return Ok();
        }
    }
}
