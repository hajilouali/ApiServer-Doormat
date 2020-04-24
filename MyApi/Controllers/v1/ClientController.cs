using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize(Roles = "Admin,Accountants,Experts")]
    [ApiVersion("1")]
    public class ClientController : BaseController
    {
        private readonly IRepository<Client> _clientrepository;
        private readonly IRepository<AccountingHeading> _accountingHeadingrepository;
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Expert> _Expert;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public ClientController(IRepository<Client> repository, IRepository<Client> clientrepository, IRepository<AccountingHeading> accountingHeadingrepository, IRepository<Factor> factor, IRepository<Expert> expert, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager) 
        {
            _clientrepository = clientrepository;
            _accountingHeadingrepository = accountingHeadingrepository;
            _Factor = factor;
            _Expert = expert;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpPost]
        public  async Task<ApiResult<ClientDto>> Create(ClientDto dto, CancellationToken cancellationToken)
        {
            dto.User_ID = _userManager.Users.Where(p => p.UserName == User.Identity.Name).SingleOrDefault().Id;
            UsersProcess usersProcess = new UsersProcess(_signInManager, _userManager, _roleManager, _clientrepository, _accountingHeadingrepository, _Factor, _Expert);
            var respons = await usersProcess.addClient(dto.ToEntity());
            if (respons)
                return dto;
            throw new BadRequestException("مشکلی در فرایند ثبت طرف حساب ایجاد شده است");
        }
        [HttpDelete("{id:int}")]
        public async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            UsersProcess usersProcess = new UsersProcess(_signInManager, _userManager, _roleManager, _clientrepository, _accountingHeadingrepository, _Factor, _Expert);
            var respuns = await usersProcess.DeletClient(id);
            if (respuns)
              return  Ok();
            throw new BadRequestException("مشکلی در فرایند حذف طرف حساب ایجاد شده است");
        }
        [HttpGet]
        public async Task<ActionResult<List<ClientDto>>> Get(CancellationToken cancellationToken)
        {
            var list = await _clientrepository.TableNoTracking.ProjectTo<ClientDto>()
               .ToListAsync(cancellationToken);

            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ApiResult<ClientDto>> Get(int id, CancellationToken cancellationToken)
        {
            var dto = await _clientrepository.TableNoTracking.ProjectTo<ClientDto>()
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }
        [HttpPut("{id:int}")]
        public async Task<ApiResult<ClientDto>> Update(int id, ClientDto dto, CancellationToken cancellationToken)
        {
            UsersProcess usersProcess = new UsersProcess(_signInManager, _userManager, _roleManager, _clientrepository, _accountingHeadingrepository, _Factor, _Expert);
            var respons = await usersProcess.UpdateClient(id, dto.ToEntity());
            if (respons)
               return Ok();
            throw new BadRequestException("مشکلی در فرایند بروز رسانی طرف حساب ایجاد شده است");
        }
    }
}
