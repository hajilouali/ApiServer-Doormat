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
using Services.ViewModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    
    [ApiVersion("1")]
    public class AccountingController : BaseController
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
        private readonly SiteSettings _siteSetting;
        public AccountingController(IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, UserManager<User> userManager, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, IRepository<ExpertHistory> expertHistory, IRepository<SanadAttachment> sanadAttachment, IConfiguration configuration)
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
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _ManufactureHistory = manufactureHistory;
            _ExpertHistory = expertHistory;
            _SanadAttachment = sanadAttachment;
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }
        [Authorize(Roles = "Admin,Accountants")]
        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetLastFactorID()
        {
            var result = _Factor.TableNoTracking.Max(p => p.Id);

            return result;
        }
        [Authorize(Roles = "Admin,Accountants,Manufacturers,Experts")]
        [HttpPost("[action]")]
        public async Task<ActionResult<List<FactorDto>>> GetFactors(GetFactorDto GetFactorDto, CancellationToken cancellationToken)
        {
            var Factors = await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().ToListAsync(cancellationToken);
            if (!string.IsNullOrEmpty(GetFactorDto.StartDate) && !string.IsNullOrEmpty(GetFactorDto.EndDate))
            {
                var s = PersianDate.ToGeorgianDateTime(GetFactorDto.StartDate);

                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(GetFactorDto.EndDate);

                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);

                Factors = Factors.Where(p => p.DateTimevalu >= start && p.DateTimevalu <= end).ToList();

            }
            if (GetFactorDto.ClientID != 0)
            {
                Factors = Factors.Where(p => p.Client_ID == GetFactorDto.ClientID).ToList();

            }
            if (!GetFactorDto.ISAllType)
            {

                Factors = Factors.Where(p => p.FactorType == GetFactorDto.FactorType).ToList();
            }
            return Factors;


        }
        [Authorize(Roles = "Admin,Accountants,Manufacturers,Experts")]
        [HttpGet("[action]/{id:int}")]
        public async Task<ApiResult<FactorDto>> GetFactor(int id, CancellationToken cancellationToken)
        {
            var dto = await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }
        [Authorize(Roles = "Admin,Accountants")]

        [HttpGet("[action]/{id:int}")]
        public async Task<ApiResult<List<Product_FactorDto>>> GetFactorRows(int id, CancellationToken cancellationToken)
        {
            var dto = await _Product_Factor.TableNoTracking.ProjectTo<Product_FactorDto>().Where(p => p.Factor_ID.Equals(id)).ToListAsync(cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }
        [Authorize(Roles = "Admin,Accountants")]

        [HttpPost("[action]")]
        public async Task<ActionResult<FactorDto>> AddFactor(AddFactorViewModel dto, CancellationToken cancellationToken, List<IFormFile> file = null)
        {
            dto.User_ID = _userManager.Users.Where(p => p.UserName == User.Identity.Name).SingleOrDefault().Id;
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respons = await usersProcess.AddFactor(dto, file);
            if (respons != 0)
            {
                var opject = await _Factor.Table.SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken);
                opject.DateTime = PersianDate.ToGeorgianDateTime(dto.ShamsiDate);
                _Factor.Update(opject);
                return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken);
            }

            throw new BadRequestException("مشکلی در فرایند ثبت فاکتور ایجاد شده است");
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<FactorDto>> AddPishFactor(AddFactorViewModel dto, CancellationToken cancellationToken, List<IFormFile> file = null)
        {
            dto.User_ID = _userManager.Users.Where(p => p.UserName == User.Identity.Name).SingleOrDefault().Id;
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respons = await usersProcess.AddPishFactor(dto, file);
            if (respons != 0)
            {
                var opject = await _Factor.Table.SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken);
                opject.DateTime = PersianDate.ToGeorgianDateTime(dto.ShamsiDate);
                _Factor.Update(opject);
                return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken);

            }
            throw new BadRequestException("مشکلی در فرایند ثبت پیش فاکتور ایجاد شده است");
        }
        [HttpGet("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<FactorDto>> CheangToFactor(int id, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = usersProcess.ChangeToFactor(id);
            if (respuns)
                return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
            throw new BadRequestException("مشکلی در فرایند تغییر وضعیت فاکتور ایجاد شده است");

        }
        [HttpGet("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<FactorDto>> CheangToPishFactor(int id, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = usersProcess.ChangeToPishFactor(id);
            if (respuns)
                return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
            throw new BadRequestException("مشکلی در فرایند تغییر وضعیت پیش فاکتور ایجاد شده است");

        }
        [HttpPost("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<FactorDto>> UpdateFactor(int id, AddFactorViewModel Dto, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var factor = _Factor.GetById(id);
            if (factor.FactorType == FactorType.Factor)
            {
                Dto.User_ID = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
                var respuns = await usersProcess.UpdateFactor(id, Dto);
                if (respuns)
                {
                    var opject = await _Factor.Table.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
                    opject.DateTime = PersianDate.ToGeorgianDateTime(Dto.ShamsiDate);
                    _Factor.Update(opject);
                    return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                }
            }

            throw new BadRequestException("مشکلی در فرایند بروز رسانی فاکتور ایجاد شده است");

        }
        [HttpPost("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<FactorDto>> UpdatePishFactor(int id, AddFactorViewModel Dto, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var factor = _Factor.GetById(id);
            if (factor.FactorType == FactorType.PishFactor)
            {
                Dto.User_ID = _userManager.FindByNameAsync(User.Identity.Name).Result.Id;
                var respuns = await usersProcess.UpdatePishFactor(id, Dto);
                if (respuns)
                {
                    var opject = await _Factor.Table.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
                    opject.DateTime = PersianDate.ToGeorgianDateTime(Dto.ShamsiDate);
                    _Factor.Update(opject);
                    return await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

                }
            }

            throw new BadRequestException("مشکلی در فرایند بروز رسانی فاکتور ایجاد شده است");

        }
        [HttpDelete("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult> DeleteFactor(int id, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.DeleteFactor(id);
            if (respuns)
                return Ok();
            throw new BadRequestException("مشکلی در فرایند حذف فاکتور ایجاد شده است");
        }
        [Authorize(Roles = "Admin,Accountants,Manufacturers,Experts")]
        [HttpPost("[action]/{id:int}")]
        public async Task<ApiResult> AddFactorAttachment(int id, IFormFile file)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.AddFactorAttachment(id, file);
            if (respuns)
                return Ok();
            throw new BadRequestException("مشکلی در فرایند ثبت پیوست فاکتور ایجاد شده است");
        }
        [Authorize(Roles = "Admin,Accountants,Manufacturers,Experts")]
        [HttpGet("[action]/{id:int}")]
        public async Task<ActionResult<List<FactorAttachment>>> GetFactorAttachment(int id, CancellationToken cancellationToken)
        {
            var resss = await _FactorAttachment.TableNoTracking.Where(p => p.Facor_ID == id).ToListAsync(cancellationToken);
            if (resss != null)
                return Ok(resss);
            throw new BadRequestException("مشکلی در فرایند ثبت پیوست فاکتور ایجاد شده است");
        }
        [HttpDelete("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult> RemoveFactorAttachment(int id)
        {
            try
            {
                _FactorAttachment.Delete(_FactorAttachment.GetById(id));
                return Ok();

            }
            catch (Exception)
            {

                throw new BadRequestException("مشکلی در فرایند ثبت پیوست فاکتور ایجاد شده است");
            }



        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult<List<SanadDto>>> ClientMoein(ClientMoein ClientMoein, CancellationToken cancellationToken)
        {
            int ch = _Client.GetById(ClientMoein.ClientID).AccountingHeading_ID;
            if (ClientMoein.ClientID == 0)
                return NotFound();

            if (!string.IsNullOrEmpty(ClientMoein.StartDate) && !string.IsNullOrEmpty(ClientMoein.EndDate))
            {
                var s = PersianDate.ToGeorgianDateTime(ClientMoein.StartDate);
                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(ClientMoein.EndDate);
                e = e.AddDays(1);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 00, minute: 00, second: 00);
                Int64 bedehkari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch && p.Bedehkari > 0 && p.SanadHeading.DateTime < s).Sum(p => p.Bedehkari);
                Int64 Bestankari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch && p.Bestankari > 0 && p.SanadHeading.DateTime < s).Sum(p => p.Bestankari);
                var result = Bestankari - bedehkari;
                var list = new List<SanadDto>();
                var toprow = new SanadDto() { Comment = "مانده از قبل", AccountingHeading_ID = 0, SanadHeading_ID = 0, Id = 0, Bedehkari = 0, Bestankari = 0 };
                if (result > 0)
                {
                    toprow.Bestankari = Convert.ToInt32(result);
                }
                if (result < 0)
                {
                    toprow.Bedehkari = Convert.ToInt32(Math.Abs(result));
                }
                list.Add(toprow);
                list.AddRange(await _Sanad.TableNoTracking.Where(
                    p => p.SanadHeading.DateTime >= start &&
                    p.SanadHeading.DateTime < end &&
                    p.AccountingHeading_ID == ch
                    ).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadDto>().ToListAsync(cancellationToken));
                return Ok(list);

            }
            if (string.IsNullOrEmpty(ClientMoein.StartDate) && string.IsNullOrEmpty(ClientMoein.EndDate))
            {
                var list = await _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadDto>().ToListAsync(cancellationToken); ;
                return Ok(list);

            }


            return NotFound();
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult<List<SanadDto>>> AccountingHeadingMoein(AcountingMoein ClientMoein, CancellationToken cancellationToken)
        {
            var ch = _AccountingHeading.GetById(ClientMoein.ClientID);
            if (ClientMoein == null)
                return NotFound();

            if (!string.IsNullOrEmpty(ClientMoein.StartDate) && !string.IsNullOrEmpty(ClientMoein.EndDate))
            {
                var s = PersianDate.ToGeorgianDateTime(ClientMoein.StartDate);
                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(ClientMoein.EndDate);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day + 1, hour: 00, minute: 00, second: 00);
                Int64 bedehkari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch.Id && p.Bedehkari > 0 && p.SanadHeading.DateTime < s).Sum(p => p.Bedehkari);
                Int64 Bestankari = _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch.Id && p.Bestankari > 0 && p.SanadHeading.DateTime < s).Sum(p => p.Bestankari);
                var result = Bestankari - bedehkari;
                var list = new List<SanadDto>();
                var toprow = new SanadDto() { Comment = "مانده از قبل", AccountingHeading_ID = 0, SanadHeading_ID = 0, Id = 0, Bedehkari = 0, Bestankari = 0 };
                if (result > 0)
                {
                    toprow.Bestankari = Convert.ToInt32(result);
                }
                if (result < 0)
                {
                    toprow.Bedehkari = Convert.ToInt32(Math.Abs(result));
                }
                list.Add(toprow);
                list.AddRange(await _Sanad.TableNoTracking.Where(
                    p => p.SanadHeading.DateTime >= start &&
                    p.SanadHeading.DateTime < end &&
                    p.AccountingHeading_ID == ch.Id
                    ).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadDto>().OrderBy(p => p.Id).ToListAsync(cancellationToken));
                return Ok(list);

            }
            if (string.IsNullOrEmpty(ClientMoein.StartDate) && string.IsNullOrEmpty(ClientMoein.EndDate))
            {
                var list = await _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == ch.Id).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadDto>().OrderBy(p => p.Id).ToListAsync(cancellationToken); ;
                return Ok(list);

            }


            return NotFound();
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<List<SanadHeadingDto>>> GetSanads(CancellationToken cancellationToken)
        {
            var time = Convert.ToInt32(DateTime.Now.ToPersianDateTime().ToString("dd"));
            var s = DateTime.Now.Subtract(TimeSpan.FromDays(time));
            var sanads = await _SanadHeading.TableNoTracking.Where(p => p.DateTime >= s).ProjectTo<SanadHeadingDto>().ToListAsync(cancellationToken);

            return sanads;

        }
        [HttpGet("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<SanadHeadingDto>> GetSanadById(int id, CancellationToken cancellationToken)
        {
            var sanads = await _SanadHeading.TableNoTracking.Include(p => p.Sanads).ProjectTo<SanadHeadingDto>().SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (sanads != null)
                return Ok(sanads);

            return NotFound();
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<SanadHeadingDto>> AddSanad(sanadViewModel sanadViewModel, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.addSanad(sanadViewModel);
            if (respuns != 0)
            {
                var ob = await _SanadHeading.Table.SingleOrDefaultAsync(p => p.Id.Equals(respuns), cancellationToken);
                ob.DateTime = PersianDate.ToGeorgianDateTime(sanadViewModel.ShamsiDate);
                _SanadHeading.Update(ob);
                return await _SanadHeading.TableNoTracking.ProjectTo<SanadHeadingDto>().SingleOrDefaultAsync(p => p.Id.Equals(respuns), cancellationToken);
            }
            throw new BadRequestException("مشکلی در فرایند صدور سند ایجاد شده است");
        }
        [HttpPost("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<SanadHeadingDto>> UpdateSanad(int id, sanadViewModel sanadViewModel, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.UpdateSanad(id, sanadViewModel);
            if (respuns != 0)
            {
                var ob = await _SanadHeading.Table.SingleOrDefaultAsync(p => p.Id.Equals(respuns), cancellationToken);
                ob.DateTime = PersianDate.ToGeorgianDateTime(sanadViewModel.ShamsiDate);
                _SanadHeading.Update(ob);
                return await _SanadHeading.TableNoTracking.ProjectTo<SanadHeadingDto>().SingleOrDefaultAsync(p => p.Id.Equals(respuns), cancellationToken);

            }
            throw new BadRequestException("مشکلی در فرایند ویرایش سند ایجاد شده است");
        }
        [HttpDelete("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult> DeletSanad(int id)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.DeletSanad(id);
            if (respuns)
                return Ok();
            throw new BadRequestException("مشکلی در فرایند حذف سند ایجاد شده است");
        }
        [HttpPost("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult> AddsanadAttachment(int id, IFormFile file)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respuns = await usersProcess.AddsanadAttachment(id, file);
            if (respuns)
                return Ok();
            throw new BadRequestException("مشکلی در فرایند ثبت پیوست سند ایجاد شده است");
        }
        [HttpGet("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<List<FactorAttachment>>> GetSanadAttachment(int id, CancellationToken cancellationToken)
        {
            var resss = await _SanadAttachment.TableNoTracking.Where(p => p.SanadID == id).ToListAsync(cancellationToken);
            if (resss != null)
                return Ok(resss);
            throw new BadRequestException("مشکلی در فرایند  ایجاد شده است");
        }
        [HttpDelete("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult> RemoveSanadAttachment(int id)
        {
            try
            {
                _SanadAttachment.Delete(_SanadAttachment.GetById(id));
                return Ok();

            }
            catch (Exception)
            {

                throw new BadRequestException("مشکلی در فرایند خذف پیوست سند ایجاد شده است");
            }



        }
        [HttpGet("[action]/{id:int}")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ClientAccountingStatus> GetClientStatus(int id)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var result = await usersProcess.ClientAccountingStatus(id);
            if (result != null)
                return result;
            throw new BadRequestException("اطلاعات طرف حساب یافت نشد ");
        }
        [HttpGet("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<List<AccountingHeading>>> GetHeadingSanads(CancellationToken cancellationToken)
        {
            var result = await _AccountingHeading.TableNoTracking.ToListAsync(cancellationToken);
            if (result != null)
                return result;
            throw new BadRequestException("اطلاعات طرف حساب یافت نشد ");
        }
        [HttpGet("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<List<AccountingHeading>>> GetHeadingTitles(CancellationToken cancellationToken)
        {
            var result = await _AccountingHeading.TableNoTracking.Where(p => p.Clients.Count == 0).ToListAsync(cancellationToken);
            if (result != null)
                return result;
            throw new BadRequestException("اطلاعات طرف حساب یافت نشد ");
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ApiResult<List<SanadAccountingDto>>> Accounting(AcountingReviw Dto, CancellationToken cancellationToken)
        {
            AccountingProgres usersProcess = new AccountingProgres(_siteSetting,_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);

            List<SanadAccountingDto> List = new List<SanadAccountingDto>();

            if (!string.IsNullOrEmpty(Dto.EndDate) && !string.IsNullOrEmpty(Dto.StartDate))
            {
                var s = PersianDate.ToGeorgianDateTime(Dto.StartDate);

                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(Dto.EndDate);
                e = e.AddDays(1);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 00, minute: 00, second: 01);

                var o = new SanadAccountingDto();
                o.Id = 1;
                o.AccountingHeading_ID = 0;
                o.AccountingHeading = "اسناد دریافتی";
                var mandeh = 0;
                o.SanadAccountingDtoS = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID.Equals(7) && p.SanadHeading.FactorID.Equals(0) && p.SanadHeading.DateTime >= start && p.SanadHeading.DateTime < end).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadAccountingDto>().ToList();
                foreach (var item in o.SanadAccountingDtoS)
                {
                    var abs = Math.Abs(item.Bedehkari);
                    mandeh += item.Bestankari - abs;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                }
                mandeh = 0;
                o.Bedehkari = 0;
                o.Bestankari = 0;
                foreach (var item in o.SanadAccountingDtoS.ToList())
                {
                    var mathnimber = Math.Abs(item.Bedehkari);
                    o.Bedehkari += mathnimber;
                    mathnimber = Math.Abs(item.Bestankari);
                    o.Bestankari += mathnimber;
                }
                
                o.Comment = "اسناد دریافتی";
                o.Mandeh = o.Bestankari - o.Bedehkari;
                o.SanadHeading_ID = 0;
                o.stringDatatime = "";
                if (o.Mandeh > 0)
                    o.TashKhis = "بست";
                if (o.Mandeh < 0)
                    o.TashKhis = "بده";
                if (o.Mandeh == 0)
                    o.TashKhis = "--";
                List.Add(o);

                var b = new SanadAccountingDto();
                b.Id = 2;
                b.AccountingHeading_ID = 0;
                b.AccountingHeading = "فروش";
                var olll = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID.Equals(7) && !p.SanadHeading.FactorID.Equals(0) && p.SanadHeading.DateTime >= start && p.SanadHeading.DateTime < end).OrderBy(p => p.SanadHeading.DateTime);
                b.SanadAccountingDtoS = olll.ProjectTo<SanadAccountingDto>().ToList();
                foreach (var item in b.SanadAccountingDtoS)
                {
                    mandeh += item.Bestankari - item.Bedehkari;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                }
                mandeh = 0;
                var Bedehkari = 0;
                var Bestankari = 0;
                foreach (var item in b.SanadAccountingDtoS)
                {
                    var abs = Math.Abs(item.Bedehkari);
                    b.Bedehkari += abs;
                    abs = Math.Abs(item.Bestankari);
                    b.Bestankari += abs;
                }
                 
                b.Comment = " فروش";
                b.Mandeh = b.Bestankari - b.Bedehkari;
                b.SanadHeading_ID = 0;
                b.stringDatatime = "";
                if (b.Mandeh > 0)
                    b.TashKhis = "بست";
                if (b.Mandeh < 0)
                    b.TashKhis = "بده";
                if (b.Mandeh == 0)
                    b.TashKhis = "--";
                List.Add(b);
                var cc = new SanadAccountingDto();
                cc.Id = 3;
                cc.AccountingHeading_ID = 0;
                cc.AccountingHeading = "بده کاران / بستانکاران";
                List<SanadAccountingDto> listss = new List<SanadAccountingDto>();
                var lisa = _Client.TableNoTracking.Include(p => p.AccountingHeading);
                foreach (var item in lisa)
                {
                    var sss = new SanadAccountingDto();

                    sss.Bedehkari = 0;
                    sss.Bestankari = 0;
                    sss.AccountingHeading = item.AccountingHeading.Title;
                    sss.AccountingHeading_ID = item.AccountingHeading.Id;
                    sss.Comment = item.AccountingHeading.Title;
                    foreach (var items in _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID == item.AccountingHeading_ID && p.SanadHeading.DateTime >= start && p.SanadHeading.DateTime < end))
                    {
                        var abs = Math.Abs(items.Bedehkari);
                        sss.Bedehkari += abs;
                        abs = Math.Abs(items.Bestankari);
                        sss.Bestankari += abs;
                    }
                    
                    sss.Mandeh = sss.Bestankari - sss.Bedehkari;
                    if (sss.Mandeh > 0)
                        sss.TashKhis = "بست";
                    if (sss.Mandeh < 0)
                        sss.TashKhis = "بده";
                    if (sss.Mandeh == 0)
                        sss.TashKhis = "--";
                    listss.Add(sss);
                }
                foreach (var item in listss.ToList())
                {
                    if (item.Bestankari == 0 && item.Bedehkari == 0)
                    {
                        listss.Remove(item);
                    }
                }
                cc.SanadAccountingDtoS = listss;
                foreach (var item in cc.SanadAccountingDtoS)
                {
                    mandeh += item.Bestankari - item.Bedehkari;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                    cc.Bedehkari += item.Bedehkari;
                    cc.Bestankari += item.Bestankari;
                }
                mandeh = 0;
                Bedehkari = 0;
                Bestankari = 0;
                //foreach (var item in cc.SanadAccountingDtoS)
                //{
                //    var abs = Math.Abs(item.Bedehkari);
                //    cc.Bedehkari += abs;
                //    abs = Math.Abs(item.Bestankari);
                //    cc.Bestankari += abs;
                //}
                 
                cc.Comment = " بده کاران / بستانکاران";
                cc.Mandeh = cc.Bestankari - cc.Bedehkari;
                cc.SanadHeading_ID = 0;
                cc.stringDatatime = "";
                if (cc.Mandeh > 0)
                    cc.TashKhis = "بست";
                if (cc.Mandeh < 0)
                    cc.TashKhis = "بده";
                if (cc.Mandeh == 0)
                    cc.TashKhis = "--";
                List.Add(cc);
                int index = 4;
                foreach (var item in _AccountingHeading.TableNoTracking.Include(p => p.Clients).Where(p => p.Id != 7 && p.Clients.Count == 0))
                {
                    var a = new SanadAccountingDto();
                    a.Id = index;
                    a.AccountingHeading_ID = item.Id;
                    a.AccountingHeading = item.Title;
                    a.SanadAccountingDtoS = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID == item.Id && p.SanadHeading.DateTime >= start && p.SanadHeading.DateTime < end).ProjectTo<SanadAccountingDto>().ToList();
                    foreach (var items in a.SanadAccountingDtoS)
                    {
                        mandeh += items.Bestankari - items.Bedehkari;
                        items.Mandeh = mandeh;
                        if (items.Mandeh > 0)
                            items.TashKhis = "بست";
                        if (items.Mandeh < 0)
                            items.TashKhis = "بده";
                        if (items.Mandeh == 0)
                            items.TashKhis = "--";
                        a.Bedehkari += items.Bedehkari;
                        a.Bestankari += items.Bestankari;
                    }
                    
                    
                    //foreach (var iye in a.SanadAccountingDtoS)
                    //{
                    //    var mathnumber = Math.Abs(iye.Bedehkari);
                    //    a.Bedehkari += mathnumber;
                    //    mathnumber = Math.Abs(iye.Bestankari);
                    //    a.Bestankari += mathnumber;
                    //}
                     
                    a.Comment = item.Title;
                    a.Mandeh = a.Bestankari - a.Bedehkari;
                    a.SanadHeading_ID = 0;
                    a.stringDatatime = "";
                    if (a.Mandeh > 0)
                        a.TashKhis = "بست";
                    if (a.Mandeh < 0)
                        a.TashKhis = "بده";
                    if (a.Mandeh == 0)
                        a.TashKhis = "--";
                    List.Add(a);
                    index++;
                }
                return List;
            }
            else
            {

                var o = new SanadAccountingDto();
                o.Id = 1;
                o.AccountingHeading_ID = 0;
                o.AccountingHeading = "اسناد دریافتی";
                var mandeh = 0;
                o.SanadAccountingDtoS = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID.Equals(7) && p.SanadHeading.FactorID.Equals(0)).OrderBy(p => p.SanadHeading.DateTime).ProjectTo<SanadAccountingDto>().ToList();
                foreach (var item in o.SanadAccountingDtoS)
                {
                    var abs = Math.Abs(item.Bedehkari);
                    mandeh += item.Bestankari - abs;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                }
                mandeh = 0;
                o.Bedehkari = 0;
                o.Bestankari = 0;
                foreach (var item in o.SanadAccountingDtoS)
                {
                    var matnumber = Math.Abs(item.Bedehkari);
                    o.Bedehkari += matnumber;
                    matnumber = Math.Abs(item.Bestankari);
                    o.Bestankari += matnumber;

                }
                
                o.Comment = "اسناد دریافتی";
                o.Mandeh = o.Bestankari - o.Bedehkari;
                o.SanadHeading_ID = 0;
                o.stringDatatime = "";
                if (o.Mandeh > 0)
                    o.TashKhis = "بست";
                if (o.Mandeh < 0)
                    o.TashKhis = "بده";
                if (o.Mandeh == 0)
                    o.TashKhis = "--";
                List.Add(o);

                var b = new SanadAccountingDto();
                b.Id = 2;
                b.AccountingHeading_ID = 0;
                b.AccountingHeading = "فروش";
                var olll = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID.Equals(7) && !p.SanadHeading.FactorID.Equals(0)).OrderBy(p => p.SanadHeading.DateTime);
                b.SanadAccountingDtoS = olll.ProjectTo<SanadAccountingDto>().ToList();
                foreach (var item in b.SanadAccountingDtoS)
                {
                    mandeh += item.Bestankari - item.Bedehkari;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                }
                mandeh = 0;
                var Bedehkari = 0;
                var Bestankari = 0;
                foreach (var item in b.SanadAccountingDtoS)
                {
                    var abs = Math.Abs(item.Bedehkari);
                    b.Bedehkari += abs;
                    abs = Math.Abs(item.Bestankari);
                    b.Bestankari += abs;
                }
                 
                b.Comment = " فروش";
                b.Mandeh = b.Bestankari - b.Bedehkari;
                b.SanadHeading_ID = 0;
                b.stringDatatime = "";
                if (b.Mandeh > 0)
                    b.TashKhis = "بست";
                if (b.Mandeh < 0)
                    b.TashKhis = "بده";
                if (b.Mandeh == 0)
                    b.TashKhis = "--";
                List.Add(b);
                var cc = new SanadAccountingDto();
                cc.Id = 3;
                cc.AccountingHeading_ID = 0;
                cc.AccountingHeading = "بده کاران / بستانکاران";
                List<SanadAccountingDto> listss = new List<SanadAccountingDto>();
                var lisa = _Client.TableNoTracking.Include(p => p.AccountingHeading);
                foreach (var item in lisa)
                {
                    var sss = new SanadAccountingDto();

                    sss.Bedehkari = 0;
                    sss.Bestankari = 0;
                    sss.AccountingHeading = item.AccountingHeading.Title;
                    sss.AccountingHeading_ID = item.AccountingHeading.Id;
                    sss.Comment = item.AccountingHeading.Title;
                    foreach (var items in _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID == item.AccountingHeading_ID))
                    {
                        var abs = Math.Abs(items.Bedehkari);
                        sss.Bedehkari += abs;
                        abs = Math.Abs(items.Bestankari);
                        sss.Bestankari += abs;
                    }
                    
                    sss.Mandeh = sss.Bestankari - sss.Bedehkari;
                    if (sss.Mandeh > 0)
                        sss.TashKhis = "بست";
                    if (sss.Mandeh < 0)
                        sss.TashKhis = "بده";
                    if (sss.Mandeh == 0)
                        sss.TashKhis = "--";
                    listss.Add(sss);
                }
                foreach (var item in listss.ToList())
                {
                    if (item.Bestankari == 0 && item.Bedehkari == 0)
                    {
                        listss.Remove(item);
                    }
                }
                cc.SanadAccountingDtoS = listss;
                foreach (var item in cc.SanadAccountingDtoS)
                {
                    mandeh += item.Bestankari - item.Bedehkari;
                    item.Mandeh = mandeh;
                    if (item.Mandeh > 0)
                        item.TashKhis = "بست";
                    if (item.Mandeh < 0)
                        item.TashKhis = "بده";
                    if (item.Mandeh == 0)
                        item.TashKhis = "--";
                }
                mandeh = 0;
                Bedehkari = 0;
                Bestankari = 0;
                foreach (var item in cc.SanadAccountingDtoS)
                {
                    var abs = Math.Abs(item.Bedehkari);
                    cc.Bedehkari += abs;
                    abs = Math.Abs(item.Bestankari);
                    cc.Bestankari += abs;
                }
                 
                cc.Comment = " بده کاران / بستانکاران";
                cc.Mandeh = cc.Bestankari - cc.Bedehkari;
                cc.SanadHeading_ID = 0;
                cc.stringDatatime = "";
                if (cc.Mandeh > 0)
                    cc.TashKhis = "بست";
                if (cc.Mandeh < 0)
                    cc.TashKhis = "بده";
                if (cc.Mandeh == 0)
                    cc.TashKhis = "--";
                List.Add(cc);
                int index = 4;
                foreach (var item in _AccountingHeading.TableNoTracking.Include(p => p.Clients).Where(p => p.Id != 7 && p.Clients.Count == 0))
                {
                    var a = new SanadAccountingDto();
                    a.Id = index;
                    a.AccountingHeading_ID = item.Id;
                    a.AccountingHeading = item.Title;
                    a.SanadAccountingDtoS = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID == item.Id).ProjectTo<SanadAccountingDto>().ToList();
                    foreach (var items in a.SanadAccountingDtoS)
                    {
                        mandeh += items.Bestankari - items.Bedehkari;
                        items.Mandeh = mandeh;
                        if (items.Mandeh > 0)
                            items.TashKhis = "بست";
                        if (items.Mandeh < 0)
                            items.TashKhis = "بده";
                        if (items.Mandeh == 0)
                            items.TashKhis = "--";
                    }
                    mandeh = 0;
                    a.Bedehkari = 0;
                    a.Bestankari = 0;
                    foreach (var itemss in a.SanadAccountingDtoS)
                    {
                        var mathnumber = Math.Abs(itemss.Bedehkari);
                        a.Bedehkari += mathnumber;
                        mathnumber = Math.Abs(itemss.Bestankari);
                        a.Bestankari += mathnumber;
                    }
                    
                    a.Comment = item.Title;
                    a.Mandeh = a.Bestankari - a.Bedehkari;
                    a.SanadHeading_ID = 0;
                    a.stringDatatime = "";
                    if (a.Mandeh > 0)
                        a.TashKhis = "بست";
                    if (a.Mandeh < 0)
                        a.TashKhis = "بده";
                    if (a.Mandeh == 0)
                        a.TashKhis = "--";
                    List.Add(a);
                    index++;
                }
                return List;
            }


            throw new BadRequestException("اطلاعات طرف حساب یافت نشد ");
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin,Accountants")]

        public async Task<ActionResult<List<ClientStatusDto>>> ClientsAccountingReport(ClientsAccpuntingReports Dto, CancellationToken cancellationToken)
        {
            List<ClientStatusDto> respons = new List<ClientStatusDto>();
            foreach (var item in _Client.TableNoTracking.Include(p => p.AccountingHeading).ToList())
            {
                var dto = new ClientStatusDto();
                dto.ClientID = item.Id;
                dto.ClientName = item.ClientName;
                double bedehkari = 0;
                double destankari = 0;
                foreach (var items in _Sanad.TableNoTracking.Where(p => p.AccountingHeading_ID == item.AccountingHeading_ID))
                {
                    bedehkari += items.Bedehkari;
                    destankari += items.Bestankari;
                }
                dto.Mandeh = destankari - bedehkari;
                if (dto.Mandeh > 0)
                    dto.Vaziat = "بستانکار";
                if (dto.Mandeh < 0)
                    dto.Vaziat = "بدهکار";
                if (dto.Mandeh == 0)
                    dto.Vaziat = "---";
                if (dto.Mandeh != 0)
                {
                    respons.Add(dto);
                }

            }
            if (Dto.ClientID != 0)
            {
                respons = respons.Where(p => p.ClientID == Dto.ClientID).ToList();
            }
            if (Dto.Bed)
            {
                var beddd =
                respons = respons.Where(p => Math.Abs(p.Mandeh) >= Dto.bedIN && Math.Abs(p.Mandeh) <= Dto.bedOut).ToList();
            }
            if (Dto.Bes)
            {
                respons = respons.Where(p => p.Mandeh >= Dto.besIN && p.Mandeh <= Dto.besOut).ToList();
            }

            return respons;
        }
        [HttpGet("[action]/{year}")]
        [Authorize(Roles = "Admin,Accountants")]
        public async Task<ActionResult<List<ShopReportDto>>> getyearsshop(int year)
        {
            List<ShopReportDto> list = new List<ShopReportDto>();

            for (int i = 1; i <= 12; i++)
            {
                var ass = string.Format(year + "/" + (i>9?i.ToString(""):string.Format("0"+i)) + "/01");
                var s = PersianDate.ToGeorgianDateTime(ass);
                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var enn = year + "/" + (i > 9 ? i.ToString("") : string.Format("0" + i)) + (i < 7 ? "/31" : i == 12 ? "/29" :"/30");
                var e = PersianDate.ToGeorgianDateTime(enn);
                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);

                var dto =new ShopReportDto();
                dto.id = i;
                switch (i)
                {
                    case 1:
                        {
                            dto.Name = "فروردین";
                        }
                        break;
                    case 2:
                        {
                            dto.Name = "اردیبهشت";
                        }
                        break;
                    case 3:
                        {
                            dto.Name = "خرداد";
                        }
                        break;
                    case 4:
                        {
                            dto.Name = "تیر";
                        }
                        break;
                    case 5:
                        {
                            dto.Name = "مرداد";
                        }
                        break;
                    case 6:
                        {
                            dto.Name = "شهریور";
                        }
                        break;
                    case 7:
                        {
                            dto.Name = "مهر";
                        }
                        break;
                    case 8:
                        {
                            dto.Name = "آبان";
                        }
                        break;
                    case 9:
                        {
                            dto.Name = "آذر";
                        }
                        break;
                    case 10:
                        {
                            dto.Name = "دی";
                        }
                        break;
                    case 11:
                        {
                            dto.Name = "بهمن";
                        }
                        break;
                    case 12:
                        {
                            dto.Name = "اسفند";
                        }
                        break;
                }
                var mandeh = 0;
                var olll = _Sanad.TableNoTracking.Include(p => p.SanadHeading).Where(p => p.AccountingHeading_ID.Equals(7) && !p.SanadHeading.FactorID.Equals(0) && p.SanadHeading.DateTime >= start && p.SanadHeading.DateTime <= end).OrderBy(p => p.SanadHeading.DateTime);
                
                foreach (var item in olll.ProjectTo<SanadAccountingDto>().ToList())
                {
                    mandeh += item.Bestankari - item.Bedehkari;
                    
                }
                dto.Value = mandeh;
                list.Add(dto);

            }
            return list;

        }
    }

}
