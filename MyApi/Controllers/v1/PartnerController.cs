using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
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
using MyApi.Models;
using Services.Bessines;
using Services.ViewModels.Requests;
using Tools;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,CellPartner")]
    [ApiVersion("1")]
    public class PartnerController : BaseController
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

        public PartnerController(IRepository<Factor> factor, IRepository<Product_Factor> product_Factor, IRepository<FactorAttachment> factorAttachment, IRepository<Manufacture> manufacture, IRepository<ProductAndService> productAndService, IRepository<Client> client, IRepository<SanadHeading> sanadHeading, IRepository<Sanad> sanad, IRepository<Bank> bank, IRepository<AccountingHeading> accountingHeading, UserManager<User> userManager, IHostingEnvironment hostingEnvironment, IRepository<ManufactureHistory> manufactureHistory, IRepository<ExpertHistory> expertHistory, IRepository<SanadAttachment> sanadAttachment)
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
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<PartnerIformation>> GetCurentUserInformation()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var client =  _Client.Entities.Where(p => p.User_ID==user.Id).FirstOrDefault();
            PersianCalendar PersianCalendar1 = new PersianCalendar();
            AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            

            var s = PersianDate.ToPersianDateString(DateTime.Now);
            PersianCalendar pc = new PersianCalendar();
            var startyear = new DateTime(pc.GetYear(DateTime.Now),01, 01, 00,00,01);
            var startmonth = new DateTime(pc.GetYear(DateTime.Now), pc.GetMonth(DateTime.Now), 01, 00, 00, 01);
            PartnerIformation model = new PartnerIformation();
            model.ClientAccountingStatus = await usersProcess.ClientAccountingStatus(client.Id);
            model.UserID = user.Id;
            model.UserName = user.UserName;
            model.UserFullName = user.FullName;
            var list = await _Factor.TableNoTracking.Where(p => p.Client_ID == client.Id && p.FactorType == FactorType.PendingToAccept).Include(p => p.Product_Factor).ProjectTo<FactorDto>().ToListAsync();
            model.FactorPernding = list;
            model.ManufactureDto = await _Manufacture.TableNoTracking.Include(p => p.ManufactureHistories).Where(p => p.Factor.Client_ID == client.Id && (p.ConditionManufacture == ConditionManufacture.PendingForConstruction ||
            p.ConditionManufacture == ConditionManufacture.Cut ||
            p.ConditionManufacture == ConditionManufacture.Built
            )).ProjectTo<ManufactureDto>().ToListAsync();
            model.PartnerFactorCunt = new PartnerFactorCunt()
            {
                All = await _Factor.TableNoTracking.Where(p => p.Client_ID == client.Id).CountAsync(),
                    InMonth = await _Factor.TableNoTracking.Where(p => p.Client_ID == client.Id && p.DateTime >= startmonth).CountAsync(),
                    Inyear = await _Factor.TableNoTracking.Where(p => p.Client_ID == client.Id && p.DateTime >= startyear).CountAsync()
                };
            return model;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<List<FactorDto>>> GetFactorList(GetFactorDto GetFactorDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var client = _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefault();
            var Factors = await _Factor.TableNoTracking.Where(p=>p.Client_ID==client.Id).Include(p => p.Product_Factor ).Include(p=>p.Manufacture).ProjectTo<FactorDto>().ToListAsync(cancellationToken);
            if (!string.IsNullOrEmpty(GetFactorDto.StartDate) && !string.IsNullOrEmpty(GetFactorDto.EndDate))
            {
                var s = PersianDate.ToGeorgianDateTime(GetFactorDto.StartDate);

                var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
                var e = PersianDate.ToGeorgianDateTime(GetFactorDto.EndDate);

                var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);

                Factors = Factors.Where(p => p.DateTimevalu >= start && p.DateTimevalu <= end).ToList();

            }
            
            if (!GetFactorDto.ISAllType)
            {

                Factors = Factors.Where(p => p.FactorType == GetFactorDto.FactorType).ToList();
            }
            return Ok(Factors);
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<FactorDto>> GetFactor(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var client = _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefault();
            var factor =await _Factor.TableNoTracking.Where(p => p.Id == id).Include(p => p.Product_Factor).Include(p => p.Manufacture).ThenInclude(p=>p.ManufactureHistories).ProjectTo<FactorDto>().FirstOrDefaultAsync(cancellationToken);
            if (factor.Client_ID== client.Id)
            {
                return factor;
            }
            throw new BadRequestException("مشکلی در فرایند مشاهده فاکتور ایجاد شده است");
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<List<FactorAttachment>>> GetFactorAttachment(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var client = _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefault();
            var factor = await _Factor.TableNoTracking.Where(p => p.Id == id).Include(p => p.Product_Factor).ProjectTo<FactorDto>().FirstOrDefaultAsync(cancellationToken);
            if (factor.Client_ID == client.Id)
            {
                return _FactorAttachment.TableNoTracking.Where(p=>p.Facor_ID==factor.id).ToList();
            }
            throw new BadRequestException("مشکلی در فرایند مشاهده فاکتور ایجاد شده است");
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> DeleteFactorAttachment(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var dto =await _FactorAttachment.GetByIdAsync(cancellationToken, id);
            var client = _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefault();
            var factor = await _Factor.TableNoTracking.Where(p => p.Id == dto.Facor_ID).Include(p => p.Product_Factor).ProjectTo<FactorDto>().FirstOrDefaultAsync(cancellationToken);
            if (factor.Client_ID == client.Id)
            {
                await _FactorAttachment.DeleteAsync(dto, cancellationToken);
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند مشاهده فاکتور ایجاد شده است");
        }
        [HttpPost("[action]/{id}")]
        public async Task<ActionResult<FactorAttachment>> AddAttacmentToFactor(int id,
            CancellationToken cancellationToken)
        {
            var file = Request.Form.Files[0];
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
           
            var client = _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefault();
            var factor = await _Factor.TableNoTracking.Where(p => p.Id == id).Include(p => p.Product_Factor).ProjectTo<FactorDto>().FirstOrDefaultAsync(cancellationToken);
            if (factor.Client_ID == client.Id)
            {
                if (CheckContentImage.IsImage(file))
                {
                    bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + id));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + id));
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + id);
                    int CountFile = System.IO.Directory.GetFiles(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Factors/" + id)).Length;
                    string FName = id.ToString() + CountFile.ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, FName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream, cancellationToken);
                        var modeladd = new FactorAttachment()
                        {
                            Facor_ID = id,
                            FileName = FName,
                            Discription = "",
                        };
                        _FactorAttachment.Add(modeladd);
                        return Ok(modeladd);
                    }
                }

                
            }
            
           throw new BadRequestException("مشکلی در فرایند مشاهده فاکتور ایجاد شده است");
        }
        [HttpPost("[action]/{id}")]
        public async Task<ActionResult<FactorAttachment>> AddAttacmentDiscription(int id, attacgmodel dto ,
            CancellationToken cancellationToken)
        {
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var client =await _Client.Entities.Where(p => p.User_ID == user.Id).FirstOrDefaultAsync(cancellationToken);
            var attachment =await _FactorAttachment.GetByIdAsync(cancellationToken, id);
            var factor = await _Factor.TableNoTracking.Where(p => p.Id == attachment.Facor_ID).Include(p => p.Product_Factor).ProjectTo<FactorDto>().FirstOrDefaultAsync(cancellationToken);
            
            if (factor.Client_ID == client.Id)
            {
                attachment.Discription = dto.Discription;

               await _FactorAttachment.UpdateAsync(attachment, cancellationToken);
                return Ok(attachment);


            }

            throw new BadRequestException("مشکلی در فرایند مشاهده فاکتور ایجاد شده است");
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductAndServiceDto>>> GetProducts(CancellationToken cancellationToken)
        {
            var dto =await _ProductAndService.TableNoTracking
                .Where(p => p.ProductType == ProductType.Product && !string.IsNullOrEmpty(p.ProductCode)).ProjectTo<ProductAndServiceDto>()
                .ToListAsync(cancellationToken);
            return dto;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<FactorDto>> AddNewFactor(AddFactorPartnerViewModel dto,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            dto.UserID = user.Id;
            AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
            var respons = await usersProcess.AddCellsPartnerFactor(dto);
            if (respons != 0)
            {
                 
                var opject = await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken); ;

                return opject;

            }


            throw new BadRequestException("مشکلی در فرایند ثیت فاکتور ایجاد شده است");
        }
        [HttpPost("[action]/{id}")]
        public async Task<ActionResult<FactorDto>> UpdateFactor(int id ,AddFactorPartnerViewModel dto,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            dto.UserID = user.Id;
            var factor = await _Factor.TableNoTracking.Include(p => p.Client).Where(p=>p.Id==id)
                .FirstOrDefaultAsync(cancellationToken);
            if (factor.Client.User_ID==user.Id)
            {
                AccountingProgres usersProcess = new AccountingProgres(_Factor, _Product_Factor, _FactorAttachment, _Manufacture, _ProductAndService, _Client, _SanadHeading, _Sanad, _Bank, _AccountingHeading, _hostingEnvironment, _ManufactureHistory, _ExpertHistory, _SanadAttachment);
                var respons = await usersProcess.UpdatePartnerFactor(id,dto);
                if (respons)
                {

                    var opject = await _Factor.TableNoTracking.Include(p => p.Product_Factor).ProjectTo<FactorDto>().SingleOrDefaultAsync(p => p.Id.Equals(respons), cancellationToken); ;

                    return opject;

                }
            }
            


            throw new BadRequestException("مشکلی در فرایند بروزرسانی فاکتور ایجاد شده است");
        }

        [HttpPost("[action]")]

        public async Task<ApiResult<List<SanadDto>>> AccountingHeadingMoein(AcountingMoein ClientMoein, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var client =
                await _Client.TableNoTracking.FirstOrDefaultAsync(p => p.User_ID == user.Id, cancellationToken);
            var ch = _AccountingHeading.GetById(client.AccountingHeading_ID);
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

        #region PartnerAccoutingController
        [HttpPost("[action]")]
        [Authorize]
        public async Task<ApiResult> ChangePassword([FromForm]Passwordchange dto ,CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var respons =await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (respons.Succeeded)
            {
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند تغییر رمز عبور پیش آمده");
        }
        #endregion
    }
}