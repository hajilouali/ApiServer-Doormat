﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Entities.Tikets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using Microsoft.EntityFrameworkCore;
using Common.Exceptions;
using Services.ViewModels.Requests;
using Tools;
using System.IO;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = "Admin,CellPartner")]
    [ApiVersion("1")]
    public class TiketController : BaseController
    {
        private readonly IRepository<Tiket> _Tiket;
        private readonly IRepository<TiketContent> _TiketContent;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<User> _userManager;

        public TiketController(IRepository<Tiket> tiket, IRepository<TiketContent> tiketContent, IHostingEnvironment hostingEnvironment, UserManager<User> userManager)
        {
            _Tiket = tiket;
            _TiketContent = tiketContent;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }
        [HttpGet("[action]/{page:int}")]
        public async Task<ActionResult<List<Tiket>>> GetTiketList(int page,CancellationToken cancellationToken)
        {
            page = page - 1;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var list = await _Tiket.TableNoTracking.Where(p => p.UserID == user.Id).OrderBy(p => p.Closed).ThenBy(p => p.DataModified).ToListAsync(cancellationToken);
            list = list.Skip(10 * page).Take(10).ToList();
            //var ticketsFromRepo = (await _Tiket
            //        .GetManyAsyncPaging(p => p.UserID == user.Id, s => s.OrderBy(x => x.Closed).ThenByDescending(x => x.DataModified), "",
            //    10, 0, page));
            return list;
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<Tiket>> GetTiket(int id, CancellationToken cancellationToken)
        {
            
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket =await _Tiket.TableNoTracking.Where(p => p.UserID == user.Id&&p.Id==id).FirstOrDefaultAsync(cancellationToken);
            if (Tiket!=null)
            {
                return Ok(Tiket);
            }
            else
            {
                throw new BadRequestException("مشکلی در فرایند مشاهده تیکت ایجاد شده است");
            }
        }
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<List<TiketContent>>> GetTiketContent(int id, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket = await _Tiket.TableNoTracking.Where(p => p.UserID == user.Id && p.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (Tiket != null)
            {
                
                var model =await _TiketContent.TableNoTracking.Where(p => p.TiketId == Tiket.Id).ToListAsync(cancellationToken);
                return Ok(model);
            }
            else
            {
                throw new BadRequestException("مشکلی در فرایند مشاهده اطلاعات تیکت ایجاد شده است");
            }
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<Tiket>> AddTiket(AddNewTiket dto, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var oldtiket = await _Tiket.TableNoTracking.Where(p => p.Title == dto.Title && p.UserID == user.Id).FirstOrDefaultAsync(cancellationToken);
            if (oldtiket==null)
            {
                var tiket = new Tiket()
                {
                    UserID = user.Id,
                    Closed = false,
                    IsAdminSide = false,
                    Department=dto.Department,
                    Level=dto.Level,
                    Title=dto.Title
                };
                await _Tiket.AddAsync(tiket, cancellationToken);
                var respons =await _Tiket.TableNoTracking.FirstOrDefaultAsync(p => p.Id == tiket.Id, cancellationToken);
                return respons;

            }
            else
            {
                throw new BadRequestException("این تیکت قبلا ثبت شده است.");
            }
        }
        [HttpPost("[action]/{id}")]
        public async Task<ActionResult<TiketContent>> AddTiketContent(int id,TiketContentViewModel dto, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var Tiket = await _Tiket.TableNoTracking.Where(p => p.UserID == user.Id && p.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (Tiket!=null)
            {
                var model = new TiketContent()
                {
                    TiketId = Tiket.Id,
                    Text = dto.Text,
                    IsAdminSide = false,
                    FileURL = "",
                };
                if (dto.File.Length>0)
                {
                    var check = CheckContentdocument.isdocoment(dto.File);
                    if (check==true)
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
                _TiketContent.Add(model);
                return model;
            }
            throw new BadRequestException("مشکلی در فرایند ثبت پیغام به وجود آمده");
        }
    }
}