using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Repositories;
using Entities;
using Entities.Tikets;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using Services.ViewModels.Requests;
using Tools;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,Accountants,Manufacturers")]
    [ApiVersion("1")]
    public class ManageTiketsController : BaseController
    {
        private readonly IRepository<Tiket> _Tiket;
        private readonly IRepository<TiketContent> _TiketContent;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<User> _userManager;

        public ManageTiketsController(IRepository<Tiket> tiket, IRepository<TiketContent> tiketContent, IHostingEnvironment hostingEnvironment, UserManager<User> userManager)
        {
            _Tiket = tiket;
            _TiketContent = tiketContent;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<List<TiketDto>>> GetTiketListbydate(GetTiket dto, CancellationToken cancellationToken)
        {
            var s = PersianDate.ToGeorgianDateTime(dto.StartDate);

            var start = new DateTime(year: s.Year, month: s.Month, day: s.Day, hour: 00, minute: 00, second: 00);
            var e = PersianDate.ToGeorgianDateTime(dto.EndDate);
            var end = new DateTime(year: e.Year, month: e.Month, day: e.Day, hour: 23, minute: 59, second: 59);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var rols = await _userManager.GetRolesAsync(user);

            if (rols.Where(p => p.Contains("Admin")).Any())
            {
                var list =  await _Tiket.TableNoTracking.Where(p=>p.DataModified>= start && p.DataModified<= end).OrderBy(p => p.Closed == false)
                                .ThenBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);
                //list = list.Skip(10 * page).Take(10).ToList();
                return list;
            }

            if (rols.Where(p => p.Contains("Accountants")).Any())
            {
                var list = await _Tiket.TableNoTracking.Where(p => p.DataModified >= start && p.DataModified <= end && ( p.Department.Equals(2) || p.Department.Equals(3))).OrderBy(p => p.Closed == false)
                                .ThenBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);
                //list = list.Skip(10 * page).Take(10).ToList();
                return list;
            }

            if (rols.Where(p => p.Contains("Manufacturers")).Any())
            {
                var list = await _Tiket.TableNoTracking.Where(p => p.DataModified >= start && p.DataModified <= end && p.Department.Equals(1)).OrderBy(p => p.Closed == false)
                                .ThenBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);
                //list = list.Skip(10 * page).Take(10).ToList();
                return list;
            }
            return new List<TiketDto>();

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<TiketDto>>> GetTiketList(CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var rols = await _userManager.GetRolesAsync(user);
            //var list = await _Tiket.TableNoTracking.Where(p=>p.Closed==true)
            //    .OrderBy(p => p.DataModified)


            //    .Include(p => p.tiketContents)
            //    .ProjectTo<TiketDto>().ToListAsync(cancellationToken);

            if (rols.Where(p => p.Contains("Admin")).Any())
            {
                return await _Tiket.TableNoTracking.Where(p => p.Closed == false)
                                .OrderBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);

            }

            if (rols.Where(p => p.Contains("Accountants")).Any())
            {
                return await _Tiket.TableNoTracking.Where(p => p.Closed == false && (p.Department.Equals(2) || p.Department.Equals(3)))
                                .OrderBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);
            }

            if (rols.Where(p => p.Contains("Manufacturers")).Any())
            {
                return await _Tiket.TableNoTracking.Where(p => p.Closed == false && p.Department.Equals(1))
                                .OrderBy(p => p.DataModified)
                                .Include(p => p.tiketContents)
                                .ProjectTo<TiketDto>().ToListAsync(cancellationToken);
            }



            return new List<TiketDto>();
        }
        [HttpPost("[action]/{id}")]
        public async Task<ActionResult<TiketContentDto>> AddTiketContent(int id, TiketContentViewModel dto, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket = _Tiket.GetById(id);
            if (Tiket != null)
            {
                var model = new TiketContent()
                {
                    TiketId = Tiket.Id,
                    Text = dto.Text,
                    IsAdminSide = true,
                    FileURL = "",
                    UserID = user.Id
                };
                if (dto.File != null)
                {
                    if (dto.File.Length > 0)
                    {
                        var check = CheckContentdocument.isdocoment(dto.File);
                        if (check == true)
                        {
                            bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id));

                            if (!exists)
                                System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id));
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id);
                            int CountFile = System.IO.Directory.GetFiles(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id)).Length;
                            string FName = Tiket.Id.ToString() + CountFile.ToString() + Path.GetExtension(dto.File.FileName);
                            var filePath = Path.Combine(uploads, FName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await dto.File.CopyToAsync(fileStream);
                            }
                            model.FileURL = FName;
                        }

                    }
                }
                Tiket.DataModified = DateTime.Now;
                Tiket.Closed = true;
                Tiket.IsAdminSide = true;
                await _Tiket.UpdateAsync(Tiket, cancellationToken);
                await _TiketContent.AddAsync(model, cancellationToken);


                return await _TiketContent.TableNoTracking.Where(p => p.Id == model.Id).ProjectTo<TiketContentDto>().FirstOrDefaultAsync();
            }
            throw new BadRequestException("مشکلی در فرایند ثبت پیغام به وجود آمده");
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<List<TiketContentDto>>> GetTiketContent(int id, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket = await _Tiket.TableNoTracking.Where(p =>  p.Id == id).Include(p=>p.tiketContents).ProjectTo<TiketDto>().FirstOrDefaultAsync(cancellationToken);
            if (Tiket != null)
            {

                return Ok(Tiket);
            }
            else
            {
                throw new BadRequestException("مشکلی در فرایند مشاهده اطلاعات تیکت ایجاد شده است");
            }
        }
        [HttpPost("[action]/{id:int}")]
        public async Task<ApiResult<TiketContentDto>> AddTiketContentAttachment(int id, IFormFile file,CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket = _Tiket.GetById(id);

            var model = new TiketContent()
            {
                TiketId = Tiket.Id,
                Text = "",
                IsAdminSide = true,
                FileURL = "",
                UserID = user.Id
            };
            if (file != null)
            {
                if (file.Length > 0)
                {
                    var check = CheckContentdocument.isdocoment(file);
                    if (check == true)
                    {
                        bool exists = System.IO.Directory.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id));

                        if (!exists)
                            System.IO.Directory.CreateDirectory(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id));
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id);
                        int CountFile = System.IO.Directory.GetFiles(Path.Combine(_hostingEnvironment.WebRootPath, "uploads/Tiket/" + Tiket.Id)).Length;
                        string FName = Tiket.Id.ToString() + CountFile.ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploads, FName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        model.FileURL = FName;
                    }

                }
                Tiket.DataModified = DateTime.Now;
                Tiket.Closed = true;
                Tiket.IsAdminSide = true;
                await _Tiket.UpdateAsync(Tiket, cancellationToken);
                await _TiketContent.AddAsync(model, cancellationToken);


                return await _TiketContent.TableNoTracking.Where(p => p.Id == model.Id).ProjectTo<TiketContentDto>().FirstOrDefaultAsync();
            }
            

            throw new BadRequestException("مشکلی در فرایند ثبت پیغام به وجود آمده");
        }
    }
}