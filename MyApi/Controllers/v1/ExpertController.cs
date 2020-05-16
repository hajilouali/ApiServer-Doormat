using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Exceptions;
using Data.Repositories;
using Entities;
using Entities.Accounting;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApi.Models;
using Services.Bessines;
using Services.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Accountants,Experts")]
    [ApiVersion("1")]
    public class ExpertController : BaseController
    {
        private readonly IRepository<Expert> _Expert;
        private readonly IRepository<ExpertHistory> _ExpertHistory;
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
        private readonly IRepository<SanadAttachment> _SanadAttachment;
        private readonly SiteSettings _siteSetting;
        public ExpertController(IRepository<Expert> expert, IRepository<ExpertHistory> expertHistory, IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, UserManager<User> userManager, IRepository<SanadAttachment> sanadAttachment, IConfiguration configuration)
        {
            _Expert = expert;
            _ExpertHistory = expertHistory;
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
            _userManager = userManager;
            _SanadAttachment = sanadAttachment;
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }
        [HttpPost("[action]")]
        public async Task<ApiResult<List<ExpertDto>>> GetExpertbyTime(ManufactureRequest dto, CancellationToken CancellationToken)
        {


            if (dto.AllTime)
            {
                var list = _Expert.TableNoTracking.Include(p => p.ExpertHistories).ProjectTo<ExpertDto>().ToList();
                if (list != null)
                    return Ok(list);
            }
            else
            {
                var s = PersianDate.ToGeorgianDateTime(dto.StartTime);

                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(dto.EndTime);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);
                var list = _Expert.TableNoTracking.Where(p => p.DateTime >= start && p.DateTime <= end).Include(p => p.ExpertHistories).ProjectTo<ExpertDto>().ToList();
                return Ok(list);
            }
            return NotFound();
        }
        [HttpGet("[action]")]
        public async Task<ApiResult<List<ExpertDto>>> GetExpertobs(CancellationToken CancellationToken)
        {
            var list = _Expert.TableNoTracking.Where(p => p.ExpertCordition == ExpertCordition.PendingForVisit).Include(p => p.ExpertHistories).ProjectTo<ExpertDto>().ToList();
            if (list != null)
                return Ok(list);
            return NotFound();
        }
        [HttpGet("{id:int}")]
        public async Task<ApiResult<ExpertDto>> Get(int id, CancellationToken CancellationToken)
        {
            var list = await _Expert.TableNoTracking.Where(p => p.Id == id).Include(p => p.ExpertHistories).ProjectTo<ExpertDto>().SingleOrDefaultAsync(CancellationToken);
            if (list != null)
                return Ok(list);
            return NotFound();
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> CreateExpert(int id)
        {
            var user = _Client.GetById(id);
            if (user != null)
            {
                var expert = new Expert()
                {
                    Client_ID = id,

                };
                _Expert.Add(expert);
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند ثبت بازدید ایجاد شده است");

        }
        [HttpGet("[action]/{id:int}/{ClientID:int}")]
        public async Task<ActionResult> UpdateExpert(int id, int ClientID)
        {
            var user = _Expert.GetById(id);
            if (user != null)
            {
                user.Client_ID = ClientID;
                user.DateTime = DateTime.Now;
                _Expert.Update(user);
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند بروز رسانی بازدید ایجاد شده است");

        }
        [HttpDelete("[action]/{id:int}")]
        public async Task<ActionResult> DeleteExpert(int id)
        {
            var user = _Expert.GetById(id);
            if (user != null)
            {
                List<ExpertHistory> list = new List<ExpertHistory>();
                foreach (var item in _ExpertHistory.Entities.Where(p => p.Expert_ID == user.Id))
                {
                    list.Add(item);
                }
                _ExpertHistory.DeleteRange(list);

                _Expert.Delete(user);
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند حذف بازدید ایجاد شده است");

        }
        [HttpPost("[action]/{id:int}")]
        public async Task<ApiResult<FactorDto>> ChangeExpertToFactor(int id, AddFactorViewModel model, List<IFormFile> file = null)
        {
            var idd = 0;
            var Expert = _Expert.GetById(id);
            if (Expert != null && Expert.ExpertCordition == ExpertCordition.PendingForVisit)
            {
                model.Client_ID = Expert.Client_ID;
                var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                model.User_ID = user.Id;
                var userrolls = _userManager.GetRolesAsync(user).Result;

                if (userrolls.Where(p => p.Equals("Admin") || p.Equals("Accountants")).Any())
                {
                    AccountingProgres accounting = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
                    idd = await accounting.AddPishFactor(model, file);
                }
                else
                {
                    AExpertProgres AccountingProgres = new AExpertProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory);

                    idd = await AccountingProgres.AddPishFactorFromPartner(model, file);
                }            
                var mo = _Factor.GetById(idd);
                if (mo != null)
                {
                    _ExpertHistory.Add(new ExpertHistory()
                    {
                        DateTime = DateTime.Now,
                        ExpertCordition = ExpertCordition.Issued,
                        Expert_ID = Expert.Id,
                        User_ID = _userManager.Users.Where(p => p.UserName == User.Identity.Name).SingleOrDefault().Id,
                        Facor_ID = idd,

                    });
                    Expert.ExpertCordition = ExpertCordition.Issued;
                    _Expert.Update(Expert);
                    var s = Mapper.Map<FactorDto>(mo);
                    return Ok(s);
                }



            }
            throw new BadRequestException("مشکلی در فرایند ثبت فاکتور ایجاد شده است");
        }


    }
}
